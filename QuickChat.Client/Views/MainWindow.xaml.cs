using QuickChat.Client.Models;
using QuickChat.Client.Services;
using QuickChat.Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuickChat.Client
{
    public partial class MainWindow : Window
    {
        private readonly ChatService _chatService = new();
        private readonly ApiService _apiService = new();
        private readonly MessageCacheService _cacheService = new();

        private readonly Guid _userId;
        private readonly string _username;

        private Guid _currentChatId;
        private readonly Dictionary<Guid, ObservableCollection<MessageItem>> _chatMessages = new();
        public ObservableCollection<ChatItem> Chats { get; set; } = new();

        public MainWindow(Guid userId, string username)
        {
            InitializeComponent();

            _userId = userId;
            _username = username;

            ChatsList.ItemsSource = Chats;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _chatService.Connect(_username);

            _chatService.MessageReceived += (chatId, message, senderName, senderId) =>
            {
                var id = Guid.Parse(chatId);
                var msg = new MessageItem
                {
                    Text = message,
                    Sender = senderName,
                    SenderId = senderId,
                    IsMine = senderId == _userId
                };

                if (!_chatMessages.ContainsKey(id))
                    _chatMessages[id] = new ObservableCollection<MessageItem>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatMessages[id].Add(msg);
                    _cacheService.SaveMessages(id, _chatMessages[id].ToList());

                    if (id == _currentChatId)
                        ShowMessagesForCurrentChat();
                });
            };

            _chatService.NewChatCreated += async (chatId, otherUsername) =>
            {
                Console.WriteLine($"🆕 SignalR: Новый чат {chatId} с {otherUsername}");

                var chat = new ChatItem
                {
                    Id = Guid.Parse(chatId),
                    OtherUser = otherUsername
                };

                Chats.Add(chat);

                var history = await _apiService.GetChatMessagesAsync(chat.Id, _userId);
                _chatMessages[chat.Id] = new ObservableCollection<MessageItem>(history);
                _cacheService.SaveMessages(chat.Id, history);
            };

            await RefreshChatListAsync();

            // ✅ Вызовем обновление онлайн-статусов только один раз
            _ = StartUpdatingStatuses();
        }


        private async Task RefreshChatListAsync()
        {
            Chats.Clear();

            var chatsFromServer = await _apiService.GetUserChatsAsync(_userId);

            foreach (var chat in chatsFromServer)
            {
                // Назначаем цвет, если отсутствует
                if (chat.UserColor == null)
                {
                    chat.UserColor = new SolidColorBrush(GetColorFromString(chat.OtherUser));
                }

                // Назначаем имя, если не пришло
                if (string.IsNullOrWhiteSpace(chat.DisplayName))
                {
                    chat.DisplayName = chat.OtherUser;
                }

                Chats.Add(chat);

                var history = await _apiService.GetChatMessagesAsync(chat.Id, _userId);
                _chatMessages[chat.Id] = new ObservableCollection<MessageItem>(
                    history.Select(m => new MessageItem
                    {
                        Text = m.Text,
                        Sender = m.Sender,
                        SenderId = m.SenderId,
                        IsMine = m.SenderId == _userId
                    })
                );

                _cacheService.SaveMessages(chat.Id, history);
                await _chatService.JoinChatGroup(chat.Id.ToString());
            }

            if (Chats.Any())
            {
                _currentChatId = Chats.First().Id;
                ChatsList.SelectedItem = Chats.First();
                ShowMessagesForCurrentChat();
            }
        }


        private void ChatsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatsList.SelectedItem is ChatItem selected)
            {
                _currentChatId = selected.Id;

                if (_chatMessages.TryGetValue(_currentChatId, out var messages))
                {
                    MessagesList.ItemsSource = messages;
                }
                else
                {
                    MessagesList.ItemsSource = null;
                }
            }
        }


        private Color GetColorFromString(string input)
        {
            int hash = input.GetHashCode();
            byte r = (byte)((hash >> 16) & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)(hash & 0xFF);
            return Color.FromRgb(r, g, b);
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T childOfType)
                    return childOfType;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void ShowMessagesForCurrentChat()
        {
            if (_chatMessages.TryGetValue(_currentChatId, out var messages))
            {
                MessagesList.ItemsSource = messages;

                // Прокрутка вниз — отложена, чтобы сработало после визуализации
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var scrollViewer = FindVisualChild<ScrollViewer>(MessagesList);
                    scrollViewer?.ScrollToEnd();
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private async Task StartUpdatingStatuses()
        {
            while (true)
            {
                try
                {
                    var statuses = await _apiService.GetUserStatusesAsync();

                    foreach (var chat in Chats)
                    {
                        var status = statuses.FirstOrDefault(s => s.Username == chat.OtherUser);
                        if (status != null)
                            chat.IsOnline = status.IsOnline;
                    }
                }
                catch
                {
                    // игнорируем ошибки
                }

                await Task.Delay(30000); // обновление каждые 30 секунд
            }
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {

            var text = MessageInputBox.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                var sent = await _apiService.SendMessageToApiAsync(_currentChatId, text, _username);

                if (sent)
                {
                    MessageInputBox.Clear();
                }
                else
                {
                    MessageBox.Show("Ошибка при отправке сообщения.");
                }
            }
        }
        private void MessageInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true; // чтобы не добавлял новую строку
                SendButton_Click(null, null);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.IsOpen = true;
        }

        private async void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await _chatService.Disconnect();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private async void NewChat_Click(object sender, RoutedEventArgs e)
        {
            var selectWindow = new SelectUserWindow();
            if (selectWindow.ShowDialog() == true)
            {
                var targetUserId = selectWindow.SelectedUserId;
                var targetUserName = selectWindow.SelectedUserName;

                var chatId = await _apiService.CreatePrivateChatAsync(_userId, targetUserId);
                if (chatId.HasValue)
                {
                    var chat = new ChatItem
                    {
                        Id = chatId.Value,
                        OtherUser = targetUserName
                    };

                    Chats.Add(chat);
                    await _chatService.JoinChatGroup(chat.Id.ToString());

                    var history = await _apiService.GetChatMessagesAsync(chat.Id, _userId);
                    _chatMessages[chat.Id] = new ObservableCollection<MessageItem>(history);
                    _cacheService.SaveMessages(chat.Id, history);

                    _currentChatId = chat.Id;
                    ChatsList.SelectedItem = chat;
                    ShowMessagesForCurrentChat();
                }
                else
                {
                    MessageBox.Show("❌ Не удалось создать чат.");
                }
            }
        }
    }
}
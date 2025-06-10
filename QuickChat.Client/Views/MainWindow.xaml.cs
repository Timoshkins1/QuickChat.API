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
        }

        private async Task RefreshChatListAsync()
        {
            Chats.Clear();
            var chatsFromServer = await _apiService.GetUserChatsAsync(_userId);

            foreach (var chat in chatsFromServer)
            {
                Chats.Add(chat);
                var history = await _apiService.GetChatMessagesAsync(chat.Id, _userId);
                _chatMessages[chat.Id] = new ObservableCollection<MessageItem>(history);
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
                ShowMessagesForCurrentChat();
            }
        }

        private void ShowMessagesForCurrentChat()
        {
            if (_chatMessages.TryGetValue(_currentChatId, out var messages))
            {
                MessagesList.ItemsSource = messages;
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
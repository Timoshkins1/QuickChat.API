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

            _chatService.MessageReceived += async (chatId, message, senderName) =>
            {
                var id = Guid.Parse(chatId);

                // Если чата ещё нет — загружаем и добавляем его
                if (!Chats.Any(c => c.Id == id))
                {
                    var chatsFromServer = await _apiService.GetUserChatsAsync(_userId);
                    var newChat = chatsFromServer.FirstOrDefault(c => c.Id == id);

                    if (newChat != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Chats.Add(newChat);
                            _chatMessages[newChat.Id] = new ObservableCollection<MessageItem>();
                        });
                    }
                }

                // Добавляем сообщение
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!_chatMessages.ContainsKey(id))
                        _chatMessages[id] = new ObservableCollection<MessageItem>();

                    _chatMessages[id].Add(new MessageItem
                    {
                        Text = message,
                        Sender = senderName,
                        IsMine = senderName == _username
                    });

                    if (id == _currentChatId)
                        ShowMessagesForCurrentChat();
                });
            };

            await RefreshChatListAsync();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageInputBox.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                await _chatService.SendMessage(_currentChatId, text);
                await _apiService.SendMessageToApiAsync(_currentChatId, text, _username);

                MessageInputBox.Clear();
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
            if (!_chatMessages.ContainsKey(_currentChatId))
                _chatMessages[_currentChatId] = new ObservableCollection<MessageItem>();

            MessagesList.ItemsSource = _chatMessages[_currentChatId];
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
                    _chatMessages[chat.Id] = new ObservableCollection<MessageItem>();

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

        private async Task RefreshChatListAsync()
        {
            Chats.Clear();

            var chatsFromServer = await _apiService.GetUserChatsAsync(_userId);
            foreach (var chat in chatsFromServer)
                Chats.Add(chat);

            if (Chats.Any())
            {
                if (!_chatMessages.ContainsKey(Chats.First().Id))
                    _chatMessages[Chats.First().Id] = new ObservableCollection<MessageItem>();

                _currentChatId = Chats.First().Id;
                ChatsList.SelectedItem = Chats.First();
                ShowMessagesForCurrentChat();
            }
        }
    }
}

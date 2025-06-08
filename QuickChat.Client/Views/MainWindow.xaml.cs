using QuickChat.Client.Models;
using QuickChat.Client.Services;
using QuickChat.Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            _chatService.MessageReceived += (chatId, message, sender) =>
            {
                var id = Guid.Parse(chatId);

                if (!_chatMessages.ContainsKey(id))
                    _chatMessages[id] = new ObservableCollection<MessageItem>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatMessages[id].Add(new MessageItem
                    {
                        Text = message,
                        Sender = sender,
                        IsMine = sender == _username
                    });

                    if (id == _currentChatId)
                        ShowMessagesForCurrentChat();
                });
            };

            var chatsFromServer = await _apiService.GetUserChatsAsync(_userId);
            foreach (var chat in chatsFromServer)
                Chats.Add(chat);

            if (Chats.Any())
            {
                _currentChatId = Chats.First().Id;
                ChatsList.SelectedItem = Chats.First();
                ShowMessagesForCurrentChat();
            }
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

                var chat = await _apiService.CreatePrivateChatAsync(_userId, targetUserId);
                if (chat != null)
                {
                    chat.OtherUser = targetUserName;
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
    }
}

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
        private readonly Dictionary<Guid, ObservableCollection<MessageItem>> _chatMessages = new();
        public ObservableCollection<ChatItem> Chats { get; set; } = new();
        private Guid _currentChatId;
        private string _username = "User1"; // ← заменить на логин при авторизации

        public MainWindow()
        {
            InitializeComponent();
            ChatsList.ItemsSource = Chats;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _chatService.Connect("User1"); // ← имя пользователя
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
                        IsMine = sender == _username
                    });

                    if (id == _currentChatId)
                        ShowMessagesForCurrentChat();
                });
            };

            var serverChats = await _apiService.GetChatsAsync();

            foreach (var chat in serverChats)
            {
                Chats.Add(chat); // ✅ chat уже содержит Id и Name
            }


            if (Chats.Any())
            {
                _currentChatId = Chats.First().Id;
                ShowMessagesForCurrentChat();
                ChatsList.SelectedItem = Chats.First();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = MessageBox.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                await _chatService.SendMessage(_currentChatId, text);
                await _apiService.SendMessageToApiAsync(_currentChatId, text);
                MessageBox.Clear();
            }
        }

        private void ShowMessagesForCurrentChat()
        {
            if (!_chatMessages.ContainsKey(_currentChatId))
                _chatMessages[_currentChatId] = new ObservableCollection<MessageItem>();

            MessagesList.ItemsSource = _chatMessages[_currentChatId];
        }

        private void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterChatWindow();
            if (regWindow.ShowDialog() == true)
            {
                var newChatId = regWindow.CreatedChatId;
                var newChatName = regWindow.CreatedChatName;

                Chats.Add(new ChatItem { Id = newChatId, Name = newChatName });
            }
        }

        private void JoinChat_Click(object sender, RoutedEventArgs e)
        {
            var joinWindow = new JoinChatWindow();
            if (joinWindow.ShowDialog() == true)
            {
                var chatId = joinWindow.JoinedChatId;
                var chatName = joinWindow.JoinedChatName;

                Chats.Add(new ChatItem { Id = chatId, Name = chatName });
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
    }
}

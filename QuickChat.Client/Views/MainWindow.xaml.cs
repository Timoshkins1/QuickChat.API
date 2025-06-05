using QuickChat.Client.Models;
using QuickChat.Client.Services;
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

        public ObservableCollection<ChatItem> Chats { get; set; } = new();
        private readonly Dictionary<Guid, ObservableCollection<MessageItem>> _chatMessages = new();
        private Guid _currentChatId;

        public MainWindow()
        {
            InitializeComponent();

            ChatsList.ItemsSource = Chats;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _chatService.Connect();

            _chatService.MessageReceived += (chatId, message) =>
            {
                var id = Guid.Parse(chatId);

                if (!_chatMessages.ContainsKey(id))
                    _chatMessages[id] = new ObservableCollection<MessageItem>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatMessages[id].Add(new MessageItem
                    {
                        Text = message,
                        IsMine = false
                    });

                    if (id == _currentChatId)
                        ShowMessagesForCurrentChat();
                });
            };

            // Загрузить чаты с API
            var serverChats = await _apiService.GetChatsAsync();
            foreach (var name in serverChats)
            {
                var id = Guid.NewGuid(); // ← можно позже заменить на ID с сервера
                Chats.Add(new ChatItem { Name = name, Id = id });
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

                if (!_chatMessages.ContainsKey(_currentChatId))
                    _chatMessages[_currentChatId] = new ObservableCollection<MessageItem>();

                _chatMessages[_currentChatId].Add(new MessageItem
                {
                    Text = text,
                    IsMine = true
                });

                ShowMessagesForCurrentChat();
                MessageBox.Clear();
            }
        }

        private void ShowMessagesForCurrentChat()
        {
            if (!_chatMessages.ContainsKey(_currentChatId))
                _chatMessages[_currentChatId] = new ObservableCollection<MessageItem>();

            MessagesList.ItemsSource = _chatMessages[_currentChatId];
        }

        private async void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox("Введите имя нового чата:", "Новый чат");
            if (!string.IsNullOrWhiteSpace(input))
            {
                await _apiService.CreateChatAsync(input);
                var chat = new ChatItem { Name = input, Id = Guid.NewGuid() };
                Chats.Add(chat);
                _currentChatId = chat.Id;
                ShowMessagesForCurrentChat();
                ChatsList.SelectedItem = chat;
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

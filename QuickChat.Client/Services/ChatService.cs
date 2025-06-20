﻿using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace QuickChat.Client.Services
{
    public class ChatService
    {
        private HubConnection? _connection;
        public event Action<string, string, string, Guid>? MessageReceived;

        public event Action<string, string>? NewChatCreated;
        public async Task Disconnect()
        {
            if (_connection != null)
            {
                try
                {
                    await _connection.StopAsync();
                    await _connection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    // Логирование ошибки, если необходимо
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
                finally
                {
                    _connection = null;
                }
            }
        }
        public async Task Connect(string username)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"http://26.207.53.154:5111/chatHub?username={username}") // передаём username в query
                .Build();

            _connection.On<string, string, string, Guid>("ReceiveMessage", (chatId, text, senderName, senderId) =>
            {
                MessageReceived?.Invoke(chatId, text, senderName, senderId);
            });

            _connection.On<string, string>("NewChatCreated", (chatId, otherUsername) =>
            {
                NewChatCreated?.Invoke(chatId, otherUsername);
            });

            await _connection.StartAsync();
            await _connection.InvokeAsync("JoinAllUserChats", username);
        }

        public async Task SendMessage(string chatId, string text, string senderName, Guid senderId)
        {
            if (_connection != null)
            {
                await _connection.InvokeAsync("SendMessage", chatId, text, senderName, senderId);
            }
        }

        public async Task JoinChatGroup(string chatId)
        {
            if (_connection != null)
            {
                await _connection.InvokeAsync("JoinChatGroup", chatId);
            }
        }
    }
}

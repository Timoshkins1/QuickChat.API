﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace QuickChat.API.Hubs
{
    public class MessagesHub : Hub
    {
        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        }

        public async Task SendMessage(string chatId, string message, string senderName, string senderId)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chatId, message, senderName, senderId);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using QuickChat.Client.Models;

namespace QuickChat.Client.Services
{
    public class MessageCacheService
    {
        private readonly string _cacheDir;

        // Используем ConcurrentDictionary для блокировки на уровне chatId
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> Locks = new();

        public MessageCacheService()
        {
            _cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickChatCache");
            if (!Directory.Exists(_cacheDir))
                Directory.CreateDirectory(_cacheDir);
        }

        private string GetFilePath(Guid chatId) => Path.Combine(_cacheDir, $"{chatId}.json");

        public List<MessageItem> LoadMessages(Guid chatId)
        {
            var path = GetFilePath(chatId);
            if (!File.Exists(path)) return new List<MessageItem>();

            var semaphore = Locks.GetOrAdd(chatId, _ => new SemaphoreSlim(1, 1));
            semaphore.Wait();
            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<MessageItem>>(json) ?? new List<MessageItem>();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void SaveMessages(Guid chatId, List<MessageItem> messages)
        {
            var path = GetFilePath(chatId);
            var semaphore = Locks.GetOrAdd(chatId, _ => new SemaphoreSlim(1, 1));
            semaphore.Wait();
            try
            {
                var json = JsonSerializer.Serialize(messages);
                File.WriteAllText(path, json);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}

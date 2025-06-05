using System;
using System.Collections.Generic;

namespace QuickChat.API.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }      // Логин
        public string PasswordHash { get; set; }  // Хеш пароля
        public string Name { get; set; }          // Отображаемое имя (DisplayName)

        public bool IsOnline { get; set; }
        public DateTime LastOnline { get; set; }

        // Навигационные свойства
        public List<UserChat> UserChats { get; set; }
        public List<Message> Messages { get; set; }
    }
}

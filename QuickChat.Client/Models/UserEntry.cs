using System;
using System.ComponentModel;

namespace QuickChat.Client.Models
{
    public class UserEntry : INotifyPropertyChanged
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }

        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOnline)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

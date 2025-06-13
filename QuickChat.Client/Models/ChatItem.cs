using System;
using System.ComponentModel;
using System.Windows.Media;

namespace QuickChat.Client.Models
{
    public class ChatItem : INotifyPropertyChanged
    {
        private bool _isOnline;
        private string _displayName;

        public Guid Id { get; set; }
        public string OtherUser { get; set; }

        public Brush UserColor { get; set; }

        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                _isOnline = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

namespace QuickChat.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }

        public bool IsOnline { get; set; }
        public DateTime LastOnline { get; set; }

        public List<UserChat> UserChats { get; set; }
        public List<Message> Messages { get; set; }
    }
}

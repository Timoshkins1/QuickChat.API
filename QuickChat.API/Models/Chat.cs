using QuickChat.API.Models;

public class Chat
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; } // храним хеш пароля
    public bool IsGroup { get; set; } = true;

    public List<UserChat> UserChats { get; set; }
    public List<Message> Messages { get; set; }
}

using QuickChat.API.Models;

public class Chat
{
    public Guid Id { get; set; }
    public bool IsGroup { get; set; } = false;

    public List<UserChat> UserChats { get; set; }
    public List<Message> Messages { get; set; }
}

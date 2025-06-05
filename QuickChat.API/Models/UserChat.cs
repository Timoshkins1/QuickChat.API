using System.ComponentModel.DataAnnotations.Schema;
using QuickChat.API.Models;

public class UserChat
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User User { get; set; }

    [ForeignKey(nameof(Chat))]
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
}
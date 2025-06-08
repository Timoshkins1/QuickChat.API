namespace QuickChat.API.DTO
{
    public class CreatePrivateChatRequest
    {
        public Guid InitiatorId { get; set; }
        public Guid RecipientId { get; set; }
    }
}

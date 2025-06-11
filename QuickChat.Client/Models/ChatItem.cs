public class ChatItem
{
    public Guid Id { get; set; }
    public string OtherUser { get; set; } // Логин (оставляем для внутреннего использования)
    public string DisplayName { get; set; } // Добавляем новое свойство для отображаемого имени

    public string UserColor => GenerateColorFromGuid(Id);

    private static string GenerateColorFromGuid(Guid id)
    {
        var hash = id.GetHashCode();
        var random = new Random(hash);
        return $"#{random.Next(0x99, 0xEE):X2}{random.Next(0x99, 0xEE):X2}{random.Next(0x99, 0xEE):X2}";
    }
}
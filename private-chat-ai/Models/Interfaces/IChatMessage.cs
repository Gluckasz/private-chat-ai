namespace PrivateChatAI.Models.Interfaces
{
    public interface IChatMessage
    {
        Guid Id { get; }
        string Content { get; set; }
        bool IsUser { get; set; }
        DateTime Timestamp { get; set; }
    }
}

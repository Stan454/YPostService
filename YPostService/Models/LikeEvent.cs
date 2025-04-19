public class LikeEvent
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsLike { get; set; } 
}

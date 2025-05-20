using System.ComponentModel.DataAnnotations;

namespace YPostService.Models
{
    public class Post
    {
        public Guid PostId { get; set; } = Guid.NewGuid(); // Auto-generate ID

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Post content must be at least 5 characters long.")]
        [MaxLength(280, ErrorMessage = "Post content cannot exceed 280 characters.")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = true;
        public int LikeCount { get; set; }
    }
}
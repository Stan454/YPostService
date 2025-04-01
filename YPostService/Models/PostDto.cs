using System.ComponentModel.DataAnnotations;

namespace YPostService.Models.Dtos
{
    public class PostDto
    {
        public Guid PostId { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
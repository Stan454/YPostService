﻿using System.ComponentModel.DataAnnotations;

namespace YPostService.Models.Dtos
{
    public class PostDto
    {
        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
        public string Username { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; } = true;
        public int LikeCount { get; set; }
    }
}
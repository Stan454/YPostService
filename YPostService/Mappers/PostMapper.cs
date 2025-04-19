using YPostService.Models;
using YPostService.Models.Dtos;

namespace YPostService.Mappers
{
    public static class PostMapper
    {
        public static PostDto ToPostDto(this Post post)
        {
            return new PostDto
            {
                PostId = post.PostId,
                UserId = post.UserId,
                Username = post.Username,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                IsPublic = post.IsPublic,
                LikeCount = post.LikeCount
            };
        }
    }
}
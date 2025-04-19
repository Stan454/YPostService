using YPostService.Models;
using YPostService.Models.Dtos;

namespace YPostService.Logic
{
    public interface IPostLogic
    {
        Task<Post> GetPostByIdAsync(Guid id);
        Task<List<PostDto>> GetPublicPostsAsync();
        Task<Post> SendPostAsync(Post post);
        Task IncrementLikeCountAsync(Guid postId);
        Task DecrementLikeCountAsync(Guid postId);
    }
}
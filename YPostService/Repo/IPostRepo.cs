using Microsoft.Extensions.Hosting;
using YPostService.Models.Dtos;
using YPostService.Models;

namespace YPostService.Repo
{
    public interface IPostRepo
    {
        Task<List<PostDto>> GetPublicPosts();
        Task<Post> AddPostAsync(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
    }
}
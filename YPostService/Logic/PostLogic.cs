using Microsoft.AspNetCore.Http.HttpResults;
using YPostService.Logic;
using YPostService.Models.Dtos;
using YPostService.Repo;
using YPostService.Models;

public class PostLogic : IPostLogic
{
    private readonly IPostRepo _postRepo;

    public PostLogic(IPostRepo postRepo)
    {
        _postRepo = postRepo;
    }

    public async Task<Post> GetPostByIdAsync(Guid postId)
    {
        var post = await _postRepo.GetPostByIdAsync(postId);
        return post;
    }

    public async Task<List<PostDto>> GetPublicPostsAsync()
    {
        var allPosts = await _postRepo.GetPublicPosts();
        return allPosts;
    }

    public async Task<Post> SendPostAsync(Post post)
    {
        post.PostId = Guid.NewGuid();
        post.CreatedAt = DateTime.UtcNow;

        return await _postRepo.AddPostAsync(post);
    }

}
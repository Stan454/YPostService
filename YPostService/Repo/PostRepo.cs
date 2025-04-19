using Microsoft.EntityFrameworkCore;
using YPostService.Mappers;
using YPostService.Models;
using YPostService.Models.Dtos;
using YPostService.Repo;

public class PostRepo : IPostRepo
{
    private readonly PostDbContext _context;

    public PostRepo(PostDbContext context)
    {
        _context = context;
    }

    public async Task<List<PostDto>> GetPublicPosts()
    {
        return await _context.Posts
            .Where(post => post.IsPublic)
            .Select(post => post.ToPostDto())
            .ToListAsync();
    }

    public async Task<Post> AddPostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post> GetPostByIdAsync(Guid postId)
    {
        return await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task IncrementLikeCountAsync(Guid postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        if (post != null)
        {
            post.LikeCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecrementLikeCountAsync(Guid postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        if (post != null && post.LikeCount > 0)
        {
            post.LikeCount--;
            await _context.SaveChangesAsync();
        }
    }
}

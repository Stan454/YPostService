using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using YPostService.Repo;
using YPostService.Mappers;
using YPostService.Models;
using YPostService.Models.Dtos;

public class PostRepo : IPostRepo
{
    // Simulating an in-memory data source with a static list of posts
    private static readonly List<Post> _posts = new List<Post>
{
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("d290f1ee-6c54-4b01-90e6-d701748f0851"),
        Content = "Hello world!",
        CreatedAt = DateTime.UtcNow,
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("e1d2f3a4-b5c6-7d8e-9f01-234567890abc"),
        Content = "This is another post but its not public",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = false
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("f1e2d3c4-b5a6-7d8e-9f01-34567890abcd"),
        Content = "This is another public post",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("a1b2c3d4-e5f6-7d8e-9f01-4567890abcde"),
        Content = "This is the 3th public message!",
        CreatedAt = DateTime.UtcNow,
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("b1c2d3e4-f5a6-7d8e-9f01-567890abcdef"),
        Content = "This is the 4th public message!",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("c1d2e3f4-a5b6-7d8e-9f01-67890abcdef1"),
        Content = "This is the 5th public message!",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("d1e2f3a4-b5c6-7d8e-9f01-7890abcdef12"),
        Content = "This is the 6th public message",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("e1f2a3b4-c5d6-7d8e-9f01-890abcdef123"),
        Content = "his is the 7th public message",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("f1a2b3c4-d5e6-7d8e-9f01-90abcdef1234"),
        Content = "This is the 8th public message!",
        CreatedAt = DateTime.UtcNow,
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("a1b2c3d4-e5f6-7d8e-9f01-01abcdef1234"),
        Content = "This is the 9th public message!",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    },
    new Post
    {
        PostId = Guid.NewGuid(),
        UserId = Guid.Parse("b1c2d3e4-f5a6-7d8e-9f01-12abcdef1234"),
        Content = "This is the 10th public message!",
        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
        IsPublic = true
    }
};


    public async Task<List<PostDto>> GetPublicPosts()
    {
        var publicPosts = _posts.Where(post => post.IsPublic).Select(s => s.ToPostDto())
            .ToList();
        return await Task.FromResult(publicPosts);
    }

    public async Task<Post> AddPostAsync(Post post)
    {
        _posts.Add(post);
        return await Task.FromResult(post); // Simulating async DB operation
    }

    public async Task<Post> GetPostByIdAsync(Guid postId)
    {
        var post = _posts.FirstOrDefault(p => p.PostId == postId);
        return await Task.FromResult(post);  // Simulate async DB call
    }
}
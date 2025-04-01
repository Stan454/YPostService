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
            UserId = "user1",
            Content = "Hello world!",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user2",
            Content = "This is another post but its not public",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = false
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user3",
            Content = "This is another public post",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user4",
            Content = "This is the 3th public message!",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user5",
            Content = "This is the 4th public message!",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user6",
            Content = "This is the 5th public message!",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user7",
            Content = "This is the 6th public message",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user8",
            Content = "his is the 7th public message",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user9",
            Content = "This is the 8th public message!",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user10",
            Content = "This is the 9th public message!",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            IsPublic = true
        },
        new Post
        {
            PostId = Guid.NewGuid(),
            UserId = "user11",
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
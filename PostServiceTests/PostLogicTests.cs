using Moq;
using Xunit;
using YPostService.Logic;
using YPostService.Repo;
using YPostService.Models;
using YPostService.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PostLogicTests
{
    private readonly IPostRepo _postRepo;
    private readonly PostLogic _postLogic;

    public PostLogicTests()
    {
        // Initialize the PostRepo with sample data
        _postRepo = new PostRepo();  // Use actual PostRepo instead of a mock
        _postLogic = new PostLogic(_postRepo);
    }

    //[Fact]
    //public async Task GetPostByIdAsync_ShouldReturnPostDto_WhenPostExists()
    //{
    //    // Arrange
    //    var postId = Guid.NewGuid(); // Replace with an actual GUID from the _posts list
    //    var expectedPost = new PostDto
    //    {
    //        PostId = postId,
    //        UserId = "user1",
    //        Content = "Hello world!",
    //        CreatedAt = DateTime.UtcNow,
    //        IsPublic = true
    //    };

    //    // Act
    //    var result = await _postLogic.GetPostByIdAsync(postId);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal(expectedPost.PostId, result.PostId);
    //    Assert.Equal(expectedPost.Content, result.Content);
    //    Assert.Equal(expectedPost.UserId, result.UserId);
    //    Assert.Equal(expectedPost.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(1)); // Allow small margin for CreatedAt comparison
    //    Assert.Equal(expectedPost.IsPublic, result.IsPublic);
    //}

    [Fact]
    public async Task GetPublicPostsAsync_ShouldReturnListOfPostDtos()
    {
        // Arrange
        var expectedPosts = new List<PostDto>
        {
            new PostDto { PostId = Guid.NewGuid(), UserId = Guid.Parse("b1c2d3e4-f5a6-7d8e-9f01-12abcdef1234"), Content = "Public Post 1", CreatedAt = DateTime.UtcNow, IsPublic = true },
            new PostDto { PostId = Guid.NewGuid(), UserId = Guid.Parse("a1b2c3d4-e5f6-7d8e-9f01-01abcdef1234"), Content = "Public Post 2", CreatedAt = DateTime.UtcNow.AddMinutes(-1), IsPublic = true }
        };

        // Act
        var result = await _postLogic.GetPublicPostsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count > 0); // Should return public posts
        Assert.Contains(result, p => p.IsPublic == true);  // Ensure public posts exist
    }

    [Fact]
    public async Task SendPostAsync_ShouldReturnPostDtoWithNewId()
    {
        // Arrange
        var newPost = new Post
        {
            Content = "New content",
            IsPublic = true
        };

        var expectedPostDto = new PostDto
        {
            PostId = Guid.NewGuid(),  // PostId will be generated within SendPostAsync
            UserId = newPost.UserId,
            Content = newPost.Content,
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        };

        // Act
        var result = await _postLogic.SendPostAsync(newPost);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPost.Content, result.Content);
        Assert.NotEqual(Guid.Empty, result.PostId);  // Ensure PostId is generated
        Assert.True(result.CreatedAt <= DateTime.UtcNow); // Ensure CreatedAt is set
        Assert.Equal(newPost.UserId, result.UserId);
        Assert.Equal(newPost.IsPublic, result.IsPublic);
    }
}

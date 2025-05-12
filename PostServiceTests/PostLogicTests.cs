using Moq;
using YPostService.Models.Dtos;
using YPostService.Models;
using YPostService.Repo;

public class PostLogicTests
{
    private readonly Mock<IPostRepo> _postRepoMock;
    private readonly PostLogic _postLogic;

    public PostLogicTests()
    {
        _postRepoMock = new Mock<IPostRepo>();
        _postLogic = new PostLogic(_postRepoMock.Object);
    }

    [Fact]
    public async Task GetPostByIdAsync_ShouldReturnPostDto_WhenPostExists()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var expectedPost = new Post
        {
            PostId = postId,
            UserId = Guid.NewGuid(),
            Content = "Hello world!",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        };

        _postRepoMock.Setup(repo => repo.GetPostByIdAsync(postId))
                     .ReturnsAsync(expectedPost);

        // Act
        var result = await _postLogic.GetPostByIdAsync(postId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPost.PostId, result.PostId);
        Assert.Equal(expectedPost.Content, result.Content);
        Assert.Equal(expectedPost.UserId, result.UserId);
        Assert.Equal(expectedPost.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(expectedPost.IsPublic, result.IsPublic);
    }

    [Fact]
    public async Task GetPublicPostsAsync_ShouldReturnListOfPostDtos()
    {
        // Arrange
        var expectedPosts = new List<PostDto>
        {
            new PostDto { PostId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "Public Post 1", CreatedAt = DateTime.UtcNow, IsPublic = true },
            new PostDto { PostId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "Public Post 2", CreatedAt = DateTime.UtcNow.AddMinutes(-1), IsPublic = true }
        };

        _postRepoMock.Setup(repo => repo.GetPublicPosts())
                     .ReturnsAsync(expectedPosts);

        // Act
        var result = await _postLogic.GetPublicPostsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPosts.Count, result.Count);
        Assert.All(result, post => Assert.True(post.IsPublic));
    }

    [Fact]
    public async Task SendPostAsync_ShouldReturnPostDtoWithNewId()
    {
        // Arrange
        var newPost = new Post
        {
            Content = "New content",
            IsPublic = true,
            UserId = Guid.NewGuid()
        };

        var savedPost = new Post
        {
            PostId = Guid.NewGuid(),
            Content = newPost.Content,
            IsPublic = newPost.IsPublic,
            UserId = newPost.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _postRepoMock.Setup(repo => repo.AddPostAsync(It.IsAny<Post>()))
                     .ReturnsAsync(savedPost);

        // Act
        var result = await _postLogic.SendPostAsync(newPost);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedPost.PostId, result.PostId);
        Assert.Equal(savedPost.Content, result.Content);
        Assert.Equal(savedPost.UserId, result.UserId);
        Assert.Equal(savedPost.CreatedAt, result.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(savedPost.IsPublic, result.IsPublic);
    }
}

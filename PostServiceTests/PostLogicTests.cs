using Moq;
using YPostService.Models.Dtos;
using YPostService.Models;
using YPostService.Repo;
using Microsoft.AspNetCore.Mvc;
using YPostService.Logic;
using System.ComponentModel.DataAnnotations;

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

    [Fact]
    public async Task SendPost_RemovesScriptTagsFromContent()
    {
        // Arrange
        var mockLogic = new Mock<IPostLogic>();
        var post = new Post
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Content = "<script>alert('xss')</script>Safe content"
        };
        mockLogic.Setup(l => l.SendPostAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post p) => p);

        var controller = new PostController(mockLogic.Object);

        // Act
        var result = await controller.SendPost(post) as CreatedAtActionResult;
        var createdPost = result?.Value as Post;

        // Assert
        Assert.NotNull(createdPost);
        Assert.DoesNotContain("<script>", createdPost.Content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Safe content", createdPost.Content);
    }

    [Theory]
    [InlineData("<script>alert(1)</script>", "")]
    [InlineData("<img src='x' onerror='alert(1)'>", "")]
    [InlineData("<a href=\"javascript:alert(1)\">link</a>", "")]
    [InlineData("<div style=\"background-image: url(javascript:alert(1))\">test</div>", "")]
    public void HtmlSanitizer_RemovesDangerousContent(string input, string expected)
    {
        var sanitizer = new Ganss.Xss.HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        var sanitized = sanitizer.Sanitize(input);
        Assert.Equal(expected, sanitized);
    }

    [Fact]
    public void Post_ModelValidation_ValidModel_Passes()
    {
        // Arrange
        var post = new Post
        {
            UserId = Guid.NewGuid(),
            Username = "validuser",
            Content = "This is a valid post content.",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        };

        var context = new ValidationContext(post, null, null);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(post, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Post_ModelValidation_InvalidModel_Fails()
    {
        // Arrange
        var post = new Post
        {
            UserId = Guid.Empty, // Invalid
            Username = new string('a', 51), // te lang
            Content = "123", // Te kort
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        };

        var context = new ValidationContext(post, null, null);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(post, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.ErrorMessage.Contains("Username cannot exceed 50 characters"));
        Assert.Contains(results, r => r.ErrorMessage.Contains("Post content must be at least 5 characters long"));
    }

    [Fact]
    public void Post_ModelValidation_ContentTooLong_Fails()
    {
        // Arrange
        var post = new Post
        {
            UserId = Guid.NewGuid(),
            Username = "validuser",
            Content = new string('a', 281),
            CreatedAt = DateTime.UtcNow,
            IsPublic = true
        };

        var context = new ValidationContext(post, null, null);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(post, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.ErrorMessage.Contains("Post content cannot exceed 280 characters"));
    }

}

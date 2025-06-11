using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using YPostService.Logic;
using YPostService.Models;
using Ganss.Xss;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostLogic _postLogic;

    public PostController(IPostLogic postLogic)
    {
        _postLogic = postLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicPosts()
    {
        var posts = await _postLogic.GetPublicPostsAsync();

        if (posts == null || !posts.Any())
        {
            return NotFound(new { Message = "No public posts found" });
        }
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> SendPost([FromBody] Post post)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();

        var originalContent = post.Content;
        var sanitizedContent = sanitizer.Sanitize(originalContent);

        if (originalContent != sanitizedContent)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            Log.Warning("Suspicious input detected from IP {IP}, user {Username}. Original: {Original}, Sanitized: {Sanitized}",
                ip ?? "Unknown IP",
                post.Username ?? "Unknown User",
                originalContent,
                sanitizedContent);
        }
        post.Content = sanitizedContent;
        var createdPost = await _postLogic.SendPostAsync(post);
        return CreatedAtAction(nameof(GetPostById), new { id = createdPost.PostId }, createdPost);
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById([FromRoute] Guid id)
    {
        var post = await _postLogic.GetPostByIdAsync(id);

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpGet("/tests")]
    public IActionResult Testifcve(string username)
    {
        using (var connection = new SqlConnection("YourConnectionStringHere"))
        {
            var command = new SqlCommand("SELECT * FROM Users WHERE Username = '" + username + "'", connection);
            connection.Open();
            var reader = command.ExecuteReader();

            // Dummy logic
            if (reader.Read())
            {
                return Ok(reader["Username"].ToString());
            }
        }

        return NotFound();
    }

}
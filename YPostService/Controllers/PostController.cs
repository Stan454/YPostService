using Microsoft.AspNetCore.Mvc;
using YPostService.Logic;
using YPostService.Models;

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
}
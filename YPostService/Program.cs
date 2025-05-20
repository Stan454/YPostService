using Microsoft.EntityFrameworkCore;
using YPostService.Logic;
using YPostService.Models;
using YPostService.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<IPostLogic, PostLogic>();
builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://localhost:7099")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Configure DB context
var env = builder.Environment;

if (env.IsEnvironment("CI"))
{
    // Use in-memory database for CI builds
    builder.Services.AddDbContext<PostDbContext>(options =>
        options.UseInMemoryDatabase("CI_TestDB"));
}
else
{
    // Use PostgreSQL in other environments
    var connectionString = env.IsDevelopment()
        ? builder.Configuration.GetConnectionString("DevPostDb")
        : builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<PostDbContext>(options =>
        options.UseNpgsql(connectionString));
}

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Running in environment: {env}", env.EnvironmentName);
logger.LogInformation("Using database provider: {provider}", env.IsEnvironment("CI") ? "InMemory" : "PostgreSQL");


// Apply migrations or seed data (conditionally)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostDbContext>();

    if (env.IsEnvironment("CI"))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        dbContext.Posts.Add(new Post
        {
            PostId = Guid.NewGuid(), // Optional, since it auto-generates
            UserId = Guid.NewGuid(), // Simulate a valid user ID
            Username = "CIUser",
            Content = "This is a test post for CI pipeline.",
            CreatedAt = DateTime.UtcNow,
            IsPublic = true,
            LikeCount = 0
        });
        dbContext.SaveChanges();
    }
    else
    {
        // Ensure database is migrated in non-CI environments
        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.EntityFrameworkCore;
using YPostService.Logic;
using YPostService.Repo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<IPostLogic, PostLogic>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("https://localhost:7099") 
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddDbContext<PostDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
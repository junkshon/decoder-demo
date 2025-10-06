using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerPro;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TaskTrackerProDb"));

builder.Services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Avoid reference loops in JSON caused by EF navigation properties
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TaskTrackerPro API", Version = "v1" });
});

var app = builder.Build();

// Seed demo data
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DemoData.SeedAsync(ctx);
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Minimal health check
app.MapGet("/health", () => Results.Ok(new { status = "ok", timeUtc = DateTime.UtcNow }));

app.Run();

/// <summary>
/// Useful request DTOs used by controllers (kept here to remain within 3 files total).
/// </summary>
namespace TaskTrackerPro
{
    public record PaginationQuery(int Page = 1, int PageSize = 25);

    public record CreateUserDto(string Name, string Email);
    public record UpdateUserDto(string Name, string Email);

    public record CreateProjectDto(string Name, string? Description, Guid? OwnerId, DateOnly StartDate, DateOnly? EndDate);
    public record UpdateProjectDto(string Name, string? Description, Guid? OwnerId, DateOnly StartDate, DateOnly? EndDate);

    public record CreateTaskDto(
        string Title,
        string? Description,
        DateOnly? DueDate,
        TaskPriority Priority,
        TaskStatus Status,
        Guid ProjectId,
        Guid? AssignedToId);

    public record UpdateTaskDto(
        string Title,
        string? Description,
        DateOnly? DueDate,
        TaskPriority Priority,
        TaskStatus Status,
        Guid ProjectId,
        Guid? AssignedToId);

    public record TaskFilterQuery(
        TaskStatus? Status,
        TaskPriority? Priority,
        Guid? ProjectId,
        Guid? AssignedToId,
        DateOnly? DueBefore,
        DateOnly? DueAfter,
        int Page = 1,
        int PageSize = 25);

    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

    public record AnalyticsSummaryItem(
        Guid? UserId,
        string? UserName,
        int TotalTasks,
        int CompletedTasks,
        int OverdueTasks,
        int ActiveTasks);

    public record AnalyticsSummary(IEnumerable<AnalyticsSummaryItem> PerUser, int TotalProjects, int TotalUsers, int TotalTasks);
}

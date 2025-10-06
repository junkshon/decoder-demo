using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskTrackerPro
{
    // ---------- Services ----------
    public interface IUserService
    {
        Task<PagedResult<User>> GetAsync(int page, int pageSize);
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(CreateUserDto dto);
        Task<User?> UpdateAsync(Guid id, UpdateUserDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public interface IProjectService
    {
        Task<PagedResult<Project>> GetAsync(int page, int pageSize);
        Task<Project?> GetByIdAsync(Guid id);
        Task<Project> CreateAsync(CreateProjectDto dto);
        Task<Project?> UpdateAsync(Guid id, UpdateProjectDto dto);
        Task<bool> DeleteAsync(Guid id);
    }

    public interface ITaskService
    {
        Task<PagedResult<TaskItem>> GetAsync(TaskFilterQuery q);
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<TaskItem> CreateAsync(CreateTaskDto dto);
        Task<TaskItem?> UpdateAsync(Guid id, UpdateTaskDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<AnalyticsSummary> AnalyticsAsync();
    }

    public class UserService(AppDbContext ctx) : IUserService
    {
        private readonly AppDbContext _ctx = ctx;

        public async Task<PagedResult<User>> GetAsync(int page, int pageSize)
        {
            var query = _ctx.Users.AsNoTracking().OrderBy(u => u.Name);
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new(items, page, pageSize, total);
        }

        public Task<User?> GetByIdAsync(Guid id) =>
            _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> CreateAsync(CreateUserDto dto)
        {
            var entity = new User { Name = dto.Name, Email = dto.Email };
            _ctx.Users.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<User?> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var entity = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null) return null;
            entity.Name = dto.Name;
            entity.Email = dto.Email;
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null) return false;
            _ctx.Users.Remove(entity);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }

    public class ProjectService(AppDbContext ctx) : IProjectService
    {
        private readonly AppDbContext _ctx = ctx;

        public async Task<PagedResult<Project>> GetAsync(int page, int pageSize)
        {
            var query = _ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .OrderBy(p => p.Name);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new(items, page, pageSize, total);
        }

        public Task<Project?> GetByIdAsync(Guid id) =>
            _ctx.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Project> CreateAsync(CreateProjectDto dto)
        {
            var entity = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                OwnerId = dto.OwnerId
            };
            _ctx.Projects.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Project?> UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var entity = await _ctx.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.OwnerId = dto.OwnerId;

            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _ctx.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return false;
            _ctx.Projects.Remove(entity);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }

    public class TaskService(AppDbContext ctx) : ITaskService
    {
        private readonly AppDbContext _ctx = ctx;

        public async Task<PagedResult<TaskItem>> GetAsync(TaskFilterQuery q)
        {
            var query = _ctx.Tasks
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            if (q.Status.HasValue) query = query.Where(t => t.Status == q.Status);
            if (q.Priority.HasValue) query = query.Where(t => t.Priority == q.Priority);
            if (q.ProjectId.HasValue) query = query.Where(t => t.ProjectId == q.ProjectId);
            if (q.AssignedToId.HasValue) query = query.Where(t => t.AssignedToId == q.AssignedToId);
            if (q.DueBefore.HasValue) query = query.Where(t => t.DueDate != null && t.DueDate <= q.DueBefore);
            if (q.DueAfter.HasValue) query = query.Where(t => t.DueDate != null && t.DueDate >= q.DueAfter);

            query = query.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate);

            var total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync();
            return new(items, q.Page, q.PageSize, total);
        }

        public Task<TaskItem?> GetByIdAsync(Guid id) =>
            _ctx.Tasks
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TaskItem> CreateAsync(CreateTaskDto dto)
        {
            var entity = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Status = dto.Status,
                ProjectId = dto.ProjectId,
                AssignedToId = dto.AssignedToId
            };
            _ctx.Tasks.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<TaskItem?> UpdateAsync(Guid id, UpdateTaskDto dto)
        {
            var entity = await _ctx.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.DueDate = dto.DueDate;
            entity.Priority = dto.Priority;
            entity.Status = dto.Status;
            entity.ProjectId = dto.ProjectId;
            entity.AssignedToId = dto.AssignedToId;

            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _ctx.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return false;
            _ctx.Tasks.Remove(entity);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<AnalyticsSummary> AnalyticsAsync()
        {
            var now = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var users = await _ctx.Users.AsNoTracking().ToListAsync();
            var tasks = await _ctx.Tasks.AsNoTracking().ToListAsync();
            var projects = await _ctx.Projects.AsNoTracking().ToListAsync();

            var perUser = users
                .Select(u =>
                {
                    var userTasks = tasks.Where(t => t.AssignedToId == u.Id).ToList();
                    var completed = userTasks.Count(t => t.Status == TaskStatus.Completed);
                    var overdue = userTasks.Count(t => t.DueDate.HasValue
                                                      && t.DueDate.Value < now
                                                      && t.Status != TaskStatus.Completed
                                                      && t.Status != TaskStatus.Cancelled);
                    var active = userTasks.Count - completed;
                    return new AnalyticsSummaryItem(u.Id, u.Name, userTasks.Count, completed, overdue, active);
                })
                .ToList();

            // Unassigned bucket
            var unassignedTasks = tasks.Where(t => t.AssignedToId == null).ToList();
            if (unassignedTasks.Count > 0)
            {
                var completed = unassignedTasks.Count(t => t.Status == TaskStatus.Completed);
                var overdue = unassignedTasks.Count(t => t.DueDate.HasValue
                                                         && t.DueDate.Value < now
                                                         && t.Status != TaskStatus.Completed
                                                         && t.Status != TaskStatus.Cancelled);
                var active = unassignedTasks.Count - completed;
                perUser.Add(new AnalyticsSummaryItem(null, "Unassigned", unassignedTasks.Count, completed, overdue, active));
            }

            return new AnalyticsSummary(perUser, projects.Count, users.Count, tasks.Count);
        }
    }

    // ---------- Controllers ----------

    [ApiController]
    [Route("api/users")]
    public class UsersController(IUserService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<User>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
            => Ok(await service.GetAsync(page, pageSize));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await service.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            var user = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<User>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/projects")]
    public class ProjectsController(IProjectService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<Project>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
            => Ok(await service.GetAsync(page, pageSize));

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Project>> Get(Guid id)
        {
            var project = await service.GetByIdAsync(id);
            return project is null ? NotFound() : Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> Create([FromBody] CreateProjectDto dto)
        {
            var project = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Project>> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            var updated = await service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/tasks")]
    public class TasksController(ITaskService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskItem>>> List(
            [FromQuery] TaskStatus? status,
            [FromQuery] TaskPriority? priority,
            [FromQuery] Guid? projectId,
            [FromQuery] Guid? assignedToId,
            [FromQuery] DateOnly? dueBefore,
            [FromQuery] DateOnly? dueAfter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            var result = await service.GetAsync(new TaskFilterQuery(status, priority, projectId, assignedToId, dueBefore, dueAfter, page, pageSize));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TaskItem>> Get(Guid id)
        {
            var task = await service.GetByIdAsync(id);
            return task is null ? NotFound() : Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create([FromBody] CreateTaskDto dto)
        {
            var task = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TaskItem>> Update(Guid id, [FromBody] UpdateTaskDto dto)
        {
            var updated = await service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController(ITaskService taskService) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<ActionResult<AnalyticsSummary>> Summary()
            => Ok(await taskService.AnalyticsAsync());
    }
}

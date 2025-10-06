package com.example.tasktrackerpro;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.time.LocalDate;
import java.util.*;

@RestController
@RequestMapping("/api")
public class Controllers {

    private final UserRepository userRepo;
    private final ProjectRepository projectRepo;
    private final TaskRepository taskRepo;

    public Controllers(UserRepository u, ProjectRepository p, TaskRepository t) {
        this.userRepo = u; this.projectRepo = p; this.taskRepo = t;
    }

    // ---------- USERS ----------
    @GetMapping("/users")
    public List<User> listUsers() { return userRepo.findAll(); }

    @PostMapping("/users")
    public User createUser(@RequestBody User user) { return userRepo.save(user); }

    @GetMapping("/users/{id}")
    public ResponseEntity<User> getUser(@PathVariable String id) {
        return userRepo.findById(id).map(ResponseEntity::ok).orElse(ResponseEntity.notFound().build());
    }

    @DeleteMapping("/users/{id}")
    public ResponseEntity<Void> deleteUser(@PathVariable String id) {
        userRepo.deleteById(id); return ResponseEntity.noContent().build();
    }

    // ---------- PROJECTS ----------
    @GetMapping("/projects")
    public List<Project> listProjects() { return projectRepo.findAll(); }

    @PostMapping("/projects")
    public Project createProject(@RequestBody Project project) { return projectRepo.save(project); }

    @GetMapping("/projects/{id}")
    public ResponseEntity<Project> getProject(@PathVariable String id) {
        return projectRepo.findById(id).map(ResponseEntity::ok).orElse(ResponseEntity.notFound().build());
    }

    // ---------- TASKS ----------
    @GetMapping("/tasks")
    public List<Task> listTasks(@RequestParam(required = false) TaskStatus status) {
        if (status == null) return taskRepo.findAll();
        return taskRepo.findAll().stream().filter(t -> t.getStatus() == status).toList();
    }

    @GetMapping("/tasks/{id}")
    public ResponseEntity<Task> getTask(@PathVariable String id) {
        return taskRepo.findById(id).map(ResponseEntity::ok).orElse(ResponseEntity.notFound().build());
    }

    @PostMapping("/tasks")
    public Task createTask(@RequestBody Task task) { return taskRepo.save(task); }

    @DeleteMapping("/tasks/{id}")
    public ResponseEntity<Void> deleteTask(@PathVariable String id) {
        taskRepo.deleteById(id); return ResponseEntity.noContent().build();
    }

    // ---------- ANALYTICS ----------
    @GetMapping("/analytics/summary")
    public Map<String, Object> analytics() {
        var tasks = taskRepo.findAll();
        var users = userRepo.findAll();
        var projects = projectRepo.findAll();
        var now = LocalDate.now();

        List<Map<String, Object>> perUser = new ArrayList<>();

        for (var u : users) {
            var userTasks = tasks.stream().filter(t -> t.getAssignedTo() != null && t.getAssignedTo().getId().equals(u.getId())).toList();
            var completed = userTasks.stream().filter(t -> t.getStatus() == TaskStatus.COMPLETED).count();
            var overdue = userTasks.stream()
                    .filter(t -> t.getDueDate() != null && t.getDueDate().isBefore(now)
                            && t.getStatus() != TaskStatus.COMPLETED && t.getStatus() != TaskStatus.CANCELLED)
                    .count();

            var userSummary = Map.of(
                    "userId", u.getId(),
                    "userName", u.getName(),
                    "totalTasks", userTasks.size(),
                    "completedTasks", completed,
                    "overdueTasks", overdue,
                    "activeTasks", userTasks.size() - completed
            );
            perUser.add(userSummary);
        }

        var summary = new LinkedHashMap<String, Object>();
        summary.put("perUser", perUser);
        summary.put("totalUsers", users.size());
        summary.put("totalProjects", projects.size());
        summary.put("totalTasks", tasks.size());

        return summary;
    }
}

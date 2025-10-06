package com.example.tasktrackerpro;

import jakarta.persistence.*;
import java.time.LocalDate;
import java.util.List;

@Entity
class User {
    @Id @GeneratedValue(strategy = GenerationType.UUID)
    private String id;
    private String name;
    private String email;

    @OneToMany(mappedBy = "owner", cascade = CascadeType.ALL)
    private List<Project> ownedProjects;

    @OneToMany(mappedBy = "assignedTo", cascade = CascadeType.ALL)
    private List<Task> assignedTasks;

    public User() {}
    public User(String name, String email) { this.name = name; this.email = email; }

    // Getters and setters omitted for brevity
    public String getId() { return id; }
    public String getName() { return name; }
    public String getEmail() { return email; }
}

@Entity
class Project {
    @Id @GeneratedValue(strategy = GenerationType.UUID)
    private String id;
    private String name;
    private String description;
    private LocalDate startDate;
    private LocalDate endDate;

    @ManyToOne
    private User owner;

    @OneToMany(mappedBy = "project", cascade = CascadeType.ALL)
    private List<Task> tasks;

    public Project() {}
    public Project(String name, String desc, LocalDate start, LocalDate end, User owner) {
        this.name = name; this.description = desc;
        this.startDate = start; this.endDate = end; this.owner = owner;
    }

    // Getters
    public String getId() { return id; }
    public String getName() { return name; }
    public String getDescription() { return description; }
}

@Entity
class Task {
    @Id @GeneratedValue(strategy = GenerationType.UUID)
    private String id;
    private String title;
    private String description;
    private LocalDate dueDate;

    @Enumerated(EnumType.STRING)
    private TaskStatus status;

    @Enumerated(EnumType.STRING)
    private TaskPriority priority;

    @ManyToOne
    private Project project;

    @ManyToOne
    private User assignedTo;

    public Task() {}
    public Task(String title, String desc, LocalDate dueDate, TaskStatus status,
                TaskPriority priority, Project project, User assignedTo) {
        this.title = title; this.description = desc; this.dueDate = dueDate;
        this.status = status; this.priority = priority; this.project = project; this.assignedTo = assignedTo;
    }

    public String getId() { return id; }
    public String getTitle() { return title; }
    public TaskStatus getStatus() { return status; }
}

enum TaskStatus { NEW, IN_PROGRESS, BLOCKED, COMPLETED, CANCELLED }
enum TaskPriority { LOW, MEDIUM, HIGH, CRITICAL }

interface UserRepository extends org.springframework.data.jpa.repository.JpaRepository<User, String> {}
interface ProjectRepository extends org.springframework.data.jpa.repository.JpaRepository<Project, String> {}
interface TaskRepository extends org.springframework.data.jpa.repository.JpaRepository<Task, String> {}

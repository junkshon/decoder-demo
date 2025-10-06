import express from "express";
import { state, TaskStatus } from "./models.js";

export const router = express.Router();

// ---------- USERS ----------
router.get("/users", (req, res) => {
  res.json(state.users);
});

router.get("/users/:id", (req, res) => {
  const user = state.users.find((u) => u.id === req.params.id);
  if (!user) return res.status(404).json({ error: "User not found" });
  res.json(user);
});

router.post("/users", (req, res) => {
  const user = req.body;
  user.id = user.id || crypto.randomUUID();
  state.users.push(user);
  res.status(201).json(user);
});

// ---------- PROJECTS ----------
router.get("/projects", (req, res) => {
  res.json(state.projects);
});

router.get("/projects/:id", (req, res) => {
  const project = state.projects.find((p) => p.id === req.params.id);
  if (!project) return res.status(404).json({ error: "Project not found" });
  res.json(project);
});

// ---------- TASKS ----------
router.get("/tasks", (req, res) => {
  const { status } = req.query;
  let tasks = state.tasks;
  if (status && Object.values(TaskStatus).includes(status))
    tasks = tasks.filter((t) => t.status === status);
  res.json(tasks);
});

router.get("/tasks/:id", (req, res) => {
  const task = state.tasks.find((t) => t.id === req.params.id);
  if (!task) return res.status(404).json({ error: "Task not found" });
  res.json(task);
});

// ---------- ANALYTICS ----------
router.get("/analytics/summary", (req, res) => {
  const now = new Date();
  const users = state.users;
  const projects = state.projects;
  const tasks = state.tasks;

  const perUser = users.map((u) => {
    const userTasks = tasks.filter((t) => t.assignedToId === u.id);
    const completed = userTasks.filter((t) => t.status === "COMPLETED").length;
    const overdue = userTasks.filter(
      (t) =>
        t.dueDate &&
        new Date(t.dueDate) < now &&
        !["COMPLETED", "CANCELLED"].includes(t.status)
    ).length;

    return {
      userId: u.id,
      userName: u.name,
      totalTasks: userTasks.length,
      completedTasks: completed,
      overdueTasks: overdue,
      activeTasks: userTasks.length - completed,
    };
  });

  // Unassigned
  const unassigned = tasks.filter((t) => !t.assignedToId);
  if (unassigned.length > 0) {
    const completed = unassigned.filter((t) => t.status === "COMPLETED").length;
    const overdue = unassigned.filter(
      (t) =>
        t.dueDate &&
        new Date(t.dueDate) < now &&
        !["COMPLETED", "CANCELLED"].includes(t.status)
    ).length;

    perUser.push({
      userId: null,
      userName: "Unassigned",
      totalTasks: unassigned.length,
      completedTasks: completed,
      overdueTasks: overdue,
      activeTasks: unassigned.length - completed,
    });
  }

  res.json({
    perUser,
    totalUsers: users.length,
    totalProjects: projects.length,
    totalTasks: tasks.length,
  });
});

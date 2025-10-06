import { randomUUID } from "crypto";

// ----- ENUMS -----
export const TaskStatus = {
  NEW: "NEW",
  IN_PROGRESS: "IN_PROGRESS",
  BLOCKED: "BLOCKED",
  COMPLETED: "COMPLETED",
  CANCELLED: "CANCELLED",
};

export const TaskPriority = {
  LOW: "LOW",
  MEDIUM: "MEDIUM",
  HIGH: "HIGH",
  CRITICAL: "CRITICAL",
};

// ----- MODELS -----
export let state = {
  users: [],
  projects: [],
  tasks: [],
};

export function seedDemoData() {
  if (state.users.length > 0) return;

  const today = new Date();

  const alice = { id: randomUUID(), name: "Alice Ahmed", email: "alice@example.com" };
  const ben = { id: randomUUID(), name: "Ben Brown", email: "ben@example.com" };
  const chloe = { id: randomUUID(), name: "Chloe Chen", email: "chloe@example.com" };

  const proj1 = {
    id: randomUUID(),
    name: "Customer Portal",
    description: "Build a secure customer portal MVP.",
    startDate: new Date(today.getTime() - 21 * 86400000),
    ownerId: alice.id,
  };

  const proj2 = {
    id: randomUUID(),
    name: "Mobile App Rewrite",
    description: "Rewrite legacy app using Node.js backend.",
    startDate: new Date(today.getTime() - 35 * 86400000),
    endDate: new Date(today.getTime() + 30 * 86400000),
    ownerId: ben.id,
  };

  const tasks = [
    {
      id: randomUUID(),
      title: "Design auth flow",
      description: "Define user login journeys",
      dueDate: new Date(today.getTime() + 7 * 86400000),
      status: TaskStatus.IN_PROGRESS,
      priority: TaskPriority.HIGH,
      projectId: proj1.id,
      assignedToId: alice.id,
    },
    {
      id: randomUUID(),
      title: "Implement login API",
      description: "JWT-based login using Express",
      dueDate: new Date(today.getTime() + 3 * 86400000),
      status: TaskStatus.BLOCKED,
      priority: TaskPriority.CRITICAL,
      projectId: proj1.id,
      assignedToId: ben.id,
    },
    {
      id: randomUUID(),
      title: "Add telemetry",
      description: "Include winston logging",
      dueDate: new Date(today.getTime() - 2 * 86400000),
      status: TaskStatus.COMPLETED,
      priority: TaskPriority.MEDIUM,
      projectId: proj1.id,
      assignedToId: chloe.id,
    },
    {
      id: randomUUID(),
      title: "Create frontend shell",
      description: "React + Vite setup",
      dueDate: new Date(today.getTime() + 10 * 86400000),
      status: TaskStatus.NEW,
      priority: TaskPriority.MEDIUM,
      projectId: proj2.id,
      assignedToId: chloe.id,
    },
    {
      id: randomUUID(),
      title: "Migrate settings store",
      description: "Use SQLite instead of local storage",
      dueDate: new Date(today.getTime() - 1 * 86400000),
      status: TaskStatus.IN_PROGRESS,
      priority: TaskPriority.HIGH,
      projectId: proj2.id,
      assignedToId: ben.id,
    },
  ];

  state.users = [alice, ben, chloe];
  state.projects = [proj1, proj2];
  state.tasks = tasks;
}

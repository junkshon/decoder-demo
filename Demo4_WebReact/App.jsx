import React, { useEffect, useState } from "react";
import { getUsers, getProjects, getTasks, getAnalytics } from "./api";

export default function App() {
  const [view, setView] = useState("dashboard");
  const [users, setUsers] = useState([]);
  const [projects, setProjects] = useState([]);
  const [tasks, setTasks] = useState([]);
  const [analytics, setAnalytics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const loadData = async () => {
    try {
      setLoading(true);
      const [u, p, t, a] = await Promise.all([
        getUsers(),
        getProjects(),
        getTasks(),
        getAnalytics(),
      ]);
      setUsers(u);
      setProjects(p);
      setTasks(t);
      setAnalytics(a);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  if (loading) return <div className="p-6 text-gray-500">Loading data...</div>;
  if (error) return <div className="p-6 text-red-500">Error: {error}</div>;

  const navItems = [
    { key: "dashboard", label: "Dashboard" },
    { key: "users", label: "Users" },
    { key: "projects", label: "Projects" },
    { key: "tasks", label: "Tasks" },
    { key: "analytics", label: "Analytics" },
  ];

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <nav className="flex gap-6 p-4 bg-indigo-600 text-white font-semibold shadow">
        {navItems.map((n) => (
          <button
            key={n.key}
            onClick={() => setView(n.key)}
            className={`hover:text-yellow-300 ${
              view === n.key ? "text-yellow-300 underline" : ""
            }`}
          >
            {n.label}
          </button>
        ))}
      </nav>

      <main className="p-6 flex-1 overflow-auto">
        {view === "dashboard" && (
          <Dashboard users={users} projects={projects} tasks={tasks} />
        )}
        {view === "users" && <UserList users={users} />}
        {view === "projects" && <ProjectList projects={projects} />}
        {view === "tasks" && <TaskList tasks={tasks} />}
        {view === "analytics" && <AnalyticsView analytics={analytics} />}
      </main>
    </div>
  );
}

// ---------- Components ----------

function Dashboard({ users, projects, tasks }) {
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Task Tracker Dashboard</h1>
      <div className="grid grid-cols-3 gap-4">
        <Card title="Users" value={users.length} />
        <Card title="Projects" value={projects.length} />
        <Card title="Tasks" value={tasks.length} />
      </div>
    </div>
  );
}

function UserList({ users }) {
  return (
    <div>
      <h2 className="text-xl font-semibold mb-3">Users</h2>
      <table className="min-w-full border bg-white shadow-sm">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">Name</th>
            <th className="text-left p-2">Email</th>
          </tr>
        </thead>
        <tbody>
          {users.map((u) => (
            <tr key={u.id} className="border-t">
              <td className="p-2">{u.name}</td>
              <td className="p-2">{u.email}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function ProjectList({ projects }) {
  return (
    <div>
      <h2 className="text-xl font-semibold mb-3">Projects</h2>
      <table className="min-w-full border bg-white shadow-sm">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">Name</th>
            <th className="text-left p-2">Description</th>
            <th className="text-left p-2">Owner</th>
          </tr>
        </thead>
        <tbody>
          {projects.map((p) => (
            <tr key={p.id} className="border-t">
              <td className="p-2">{p.name}</td>
              <td className="p-2">{p.description}</td>
              <td className="p-2">{p.owner?.name ?? "—"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function TaskList({ tasks }) {
  return (
    <div>
      <h2 className="text-xl font-semibold mb-3">Tasks</h2>
      <table className="min-w-full border bg-white shadow-sm">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">Title</th>
            <th className="text-left p-2">Project</th>
            <th className="text-left p-2">Assignee</th>
            <th className="text-left p-2">Status</th>
            <th className="text-left p-2">Priority</th>
          </tr>
        </thead>
        <tbody>
          {tasks.map((t) => (
            <tr key={t.id} className="border-t">
              <td className="p-2">{t.title}</td>
              <td className="p-2">{t.project?.name ?? "—"}</td>
              <td className="p-2">{t.assignedTo?.name ?? "—"}</td>
              <td className="p-2">{t.status}</td>
              <td className="p-2">{t.priority}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function AnalyticsView({ analytics }) {
  if (!analytics) return <p>No analytics data available.</p>;

  return (
    <div>
      <h2 className="text-xl font-semibold mb-3">Analytics Summary</h2>
      <div className="mb-4 grid grid-cols-3 gap-4">
        <Card title="Total Users" value={analytics.totalUsers} />
        <Card title="Total Projects" value={analytics.totalProjects} />
        <Card title="Total Tasks" value={analytics.totalTasks} />
      </div>
      <table className="min-w-full border bg-white shadow-sm">
        <thead>
          <tr className="bg-gray-100">
            <th className="text-left p-2">User</th>
            <th className="text-left p-2">Total</th>
            <th className="text-left p-2">Completed</th>
            <th className="text-left p-2">Overdue</th>
            <th className="text-left p-2">Active</th>
          </tr>
        </thead>
        <tbody>
          {analytics.perUser.map((a, i) => (
            <tr key={i} className="border-t">
              <td className="p-2">{a.userName}</td>
              <td className="p-2">{a.totalTasks}</td>
              <td className="p-2">{a.completedTasks}</td>
              <td className="p-2">{a.overdueTasks}</td>
              <td className="p-2">{a.activeTasks}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function Card({ title, value }) {
  return (
    <div className="p-4 bg-white shadow rounded-lg text-center">
      <h3 className="text-gray-500 text-sm">{title}</h3>
      <p className="text-2xl font-bold">{value}</p>
    </div>
  );
}

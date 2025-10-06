const API_BASE = import.meta.env.VITE_API_BASE || "http://localhost:5000/api";

export async function getUsers() {
  return fetchJson(`${API_BASE}/users`);
}

export async function getProjects() {
  return fetchJson(`${API_BASE}/projects`);
}

export async function getTasks() {
  return fetchJson(`${API_BASE}/tasks`);
}

export async function getAnalytics() {
  return fetchJson(`${API_BASE}/analytics/summary`);
}

async function fetchJson(url) {
  const res = await fetch(url);
  if (!res.ok) throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
  return res.json();
}

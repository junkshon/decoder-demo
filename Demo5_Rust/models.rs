use serde::{Deserialize, Serialize};
use uuid::Uuid;
use chrono::{NaiveDate, Utc};

#[derive(Clone, Serialize, Deserialize)]
pub struct User {
    pub id: Uuid,
    pub name: String,
    pub email: String,
}

#[derive(Clone, Serialize, Deserialize)]
pub struct Project {
    pub id: Uuid,
    pub name: String,
    pub description: String,
    pub start_date: NaiveDate,
    pub end_date: Option<NaiveDate>,
    pub owner_id: Option<Uuid>,
}

#[derive(Clone, Serialize, Deserialize, PartialEq)]
pub enum TaskStatus {
    New,
    InProgress,
    Blocked,
    Completed,
    Cancelled,
}

#[derive(Clone, Serialize, Deserialize, PartialEq)]
pub enum TaskPriority {
    Low,
    Medium,
    High,
    Critical,
}

#[derive(Clone, Serialize, Deserialize)]
pub struct Task {
    pub id: Uuid,
    pub title: String,
    pub description: String,
    pub due_date: Option<NaiveDate>,
    pub status: TaskStatus,
    pub priority: TaskPriority,
    pub project_id: Uuid,
    pub assigned_to_id: Option<Uuid>,
}

#[derive(Clone, Serialize, Deserialize)]
pub struct AnalyticsItem {
    pub user_id: Option<Uuid>,
    pub user_name: String,
    pub total_tasks: usize,
    pub completed_tasks: usize,
    pub overdue_tasks: usize,
    pub active_tasks: usize,
}

#[derive(Clone, Serialize, Deserialize)]
pub struct AnalyticsSummary {
    pub per_user: Vec<AnalyticsItem>,
    pub total_users: usize,
    pub total_projects: usize,
    pub total_tasks: usize,
}

#[derive(Default, Clone)]
pub struct AppState {
    pub users: Vec<User>,
    pub projects: Vec<Project>,
    pub tasks: Vec<Task>,
}

pub fn seed_demo_data() -> AppState {
    let now = Utc::now().date_naive();

    let alice = User {
        id: Uuid::new_v4(),
        name: "Alice Ahmed".into(),
        email: "alice@example.com".into(),
    };
    let ben = User {
        id: Uuid::new_v4(),
        name: "Ben Brown".into(),
        email: "ben@example.com".into(),
    };
    let chloe = User {
        id: Uuid::new_v4(),
        name: "Chloe Chen".into(),
        email: "chloe@example.com".into(),
    };

    let proj1 = Project {
        id: Uuid::new_v4(),
        name: "Customer Portal".into(),
        description: "Build a secure customer portal MVP.".into(),
        start_date: now - chrono::Duration::days(21),
        end_date: None,
        owner_id: Some(alice.id),
    };
    let proj2 = Project {
        id: Uuid::new_v4(),
        name: "Mobile App Rewrite".into(),
        description: "Rewrite legacy mobile app using Rust + Flutter.".into(),
        start_date: now - chrono::Duration::days(35),
        end_date: Some(now + chrono::Duration::days(30)),
        owner_id: Some(ben.id),
    };

    let tasks = vec![
        Task {
            id: Uuid::new_v4(),
            title: "Design auth flow".into(),
            description: "Plan OAuth2 + JWT flow".into(),
            due_date: Some(now + chrono::Duration::days(7)),
            status: TaskStatus::InProgress,
            priority: TaskPriority::High,
            project_id: proj1.id,
            assigned_to_id: Some(alice.id),
        },
        Task {
            id: Uuid::new_v4(),
            title: "Implement login API".into(),
            description: "Build REST endpoints for auth".into(),
            due_date: Some(now + chrono::Duration::days(3)),
            status: TaskStatus::Blocked,
            priority: TaskPriority::Critical,
            project_id: proj1.id,
            assigned_to_id: Some(ben.id),
        },
        Task {
            id: Uuid::new_v4(),
            title: "Add telemetry baseline".into(),
            description: "Set up tracing".into(),
            due_date: Some(now - chrono::Duration::days(2)),
            status: TaskStatus::Completed,
            priority: TaskPriority::Medium,
            project_id: proj1.id,
            assigned_to_id: Some(chloe.id),
        },
        Task {
            id: Uuid::new_v4(),
            title: "Create Flutter shell".into(),
            description: "UI shell for cross-platform app".into(),
            due_date: Some(now + chrono::Duration::days(10)),
            status: TaskStatus::New,
            priority: TaskPriority::Medium,
            project_id: proj2.id,
            assigned_to_id: Some(chloe.id),
        },
        Task {
            id: Uuid::new_v4(),
            title: "Migrate settings store".into(),
            description: "Move configs to SQLite".into(),
            due_date: Some(now - chrono::Duration::days(1)),
            status: TaskStatus::InProgress,
            priority: TaskPriority::High,
            project_id: proj2.id,
            assigned_to_id: Some(ben.id),
        },
    ];

    AppState {
        users: vec![alice.clone(), ben.clone(), chloe.clone()],
        projects: vec![proj1, proj2],
        tasks,
    }
}

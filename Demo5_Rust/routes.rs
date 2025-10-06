use axum::{
    extract::{Path, State},
    http::StatusCode,
    response::IntoResponse,
    routing::{get, post, delete},
    Json, Router,
};
use serde_json::json;
use std::sync::MutexGuard;
use uuid::Uuid;

use crate::{
    APP_STATE,
    models::{AppState, AnalyticsItem, AnalyticsSummary, TaskStatus, User, Project, Task},
};

pub fn api_routes() -> Router {
    Router::new()
        .route("/users", get(list_users).post(create_user))
        .route("/users/:id", get(get_user).delete(delete_user))
        .route("/projects", get(list_projects))
        .route("/tasks", get(list_tasks))
        .route("/analytics/summary", get(analytics_summary))
}

pub async fn health() -> impl IntoResponse {
    Json(json!({ "status": "ok", "timeUtc": chrono::Utc::now() }))
}

// ---------- USERS ----------
async fn list_users() -> impl IntoResponse {
    let state = APP_STATE.lock().unwrap();
    Json(&state.users)
}

async fn get_user(Path(id): Path<Uuid>) -> impl IntoResponse {
    let state = APP_STATE.lock().unwrap();
    match state.users.iter().find(|u| u.id == id) {
        Some(u) => Json(u).into_response(),
        None => (StatusCode::NOT_FOUND, "User not found").into_response(),
    }
}

async fn create_user(Json(payload): Json<User>) -> impl IntoResponse {
    let mut state = APP_STATE.lock().unwrap();
    let mut user = payload.clone();
    user.id = Uuid::new_v4();
    state.users.push(user.clone());
    (StatusCode::CREATED, Json(user))
}

async fn delete_user(Path(id): Path<Uuid>) -> impl IntoResponse {
    let mut state = APP_STATE.lock().unwrap();
    let before = state.users.len();
    state.users.retain(|u| u.id != id);
    if state.users.len() < before {
        StatusCode::NO_CONTENT
    } else {
        StatusCode::NOT_FOUND
    }
}

// ---------- PROJECTS ----------
async fn list_projects() -> impl IntoResponse {
    let state = APP_STATE.lock().unwrap();
    Json(&state.projects)
}

// ---------- TASKS ----------
async fn list_tasks() -> impl IntoResponse {
    let state = APP_STATE.lock().unwrap();
    Json(&state.tasks)
}

// ---------- ANALYTICS ----------
async fn analytics_summary() -> impl IntoResponse {
    let state = APP_STATE.lock().unwrap();
    let now = chrono::Utc::now().date_naive();

    let mut per_user = Vec::<AnalyticsItem>::new();

    for user in &state.users {
        let user_tasks: Vec<&Task> = state
            .tasks
            .iter()
            .filter(|t| t.assigned_to_id == Some(user.id))
            .collect();

        let completed = user_tasks.iter().filter(|t| t.status == TaskStatus::Completed).count();
        let overdue = user_tasks
            .iter()
            .filter(|t| {
                if let Some(due) = t.due_date {
                    due < now && t.status != TaskStatus::Completed && t.status != TaskStatus::Cancelled
                } else {
                    false
                }
            })
            .count();

        per_user.push(AnalyticsItem {
            user_id: Some(user.id),
            user_name: user.name.clone(),
            total_tasks: user_tasks.len(),
            completed_tasks: completed,
            overdue_tasks: overdue,
            active_tasks: user_tasks.len().saturating_sub(completed),
        });
    }

    // Unassigned
    let unassigned: Vec<&Task> = state.tasks.iter().filter(|t| t.assigned_to_id.is_none()).collect();
    if !unassigned.is_empty() {
        let completed = unassigned.iter().filter(|t| t.status == TaskStatus::Completed).count();
        let overdue = unassigned
            .iter()
            .filter(|t| {
                if let Some(due) = t.due_date {
                    due < now && t.status != TaskStatus::Completed && t.status != TaskStatus::Cancelled
                } else {
                    false
                }
            })
            .count();

        per_user.push(AnalyticsItem {
            user_id: None,
            user_name: "Unassigned".into(),
            total_tasks: unassigned.len(),
            completed_tasks: completed,
            overdue_tasks: overdue,
            active_tasks: unassigned.len().saturating_sub(completed),
        });
    }

    let summary = AnalyticsSummary {
        per_user,
        total_users: state.users.len(),
        total_projects: state.projects.len(),
        total_tasks: state.tasks.len(),
    };

    Json(summary)
}

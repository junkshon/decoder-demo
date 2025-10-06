from fastapi import APIRouter, HTTPException
from uuid import UUID
from datetime import date
from models import (
    state,
    User,
    Project,
    Task,
    AnalyticsSummary,
    AnalyticsItem,
    TaskStatus,
)

router = APIRouter()

# ---------- USERS ----------
@router.get("/users", response_model=list[User])
def list_users():
    return list(state.users.values())


@router.get("/users/{user_id}", response_model=User)
def get_user(user_id: UUID):
    user = state.users.get(user_id)
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    return user


@router.post("/users", response_model=User, status_code=201)
def create_user(user: User):
    if user.id in state.users:
        raise HTTPException(status_code=400, detail="User already exists")
    state.users[user.id] = user
    return user


# ---------- PROJECTS ----------
@router.get("/projects", response_model=list[Project])
def list_projects():
    return list(state.projects.values())


@router.get("/projects/{project_id}", response_model=Project)
def get_project(project_id: UUID):
    project = state.projects.get(project_id)
    if not project:
        raise HTTPException(status_code=404, detail="Project not found")
    return project


# ---------- TASKS ----------
@router.get("/tasks", response_model=list[Task])
def list_tasks(status: TaskStatus | None = None):
    tasks = list(state.tasks.values())
    if status:
        tasks = [t for t in tasks if t.status == status]
    return tasks


@router.get("/tasks/{task_id}", response_model=Task)
def get_task(task_id: UUID):
    task = state.tasks.get(task_id)
    if not task:
        raise HTTPException(status_code=404, detail="Task not found")
    return task


# ---------- ANALYTICS ----------
@router.get("/analytics/summary", response_model=AnalyticsSummary)
def analytics_summary():
    today = date.today()
    users = list(state.users.values())
    projects = list(state.projects.values())
    tasks = list(state.tasks.values())

    per_user: list[AnalyticsItem] = []

    for user in users:
        user_tasks = [t for t in tasks if t.assigned_to_id == user.id]
        completed = len([t for t in user_tasks if t.status == TaskStatus.COMPLETED])
        overdue = len(
            [
                t
                for t in user_tasks
                if t.due_date and t.due_date < today
                and t.status not in [TaskStatus.COMPLETED, TaskStatus.CANCELLED]
            ]
        )
        active = len(user_tasks) - completed

        per_user.append(
            AnalyticsItem(
                user_id=user.id,
                user_name=user.name,
                total_tasks=len(user_tasks),
                completed_tasks=completed,
                overdue_tasks=overdue,
                active_tasks=active,
            )
        )

    # Unassigned
    unassigned = [t for t in tasks if t.assigned_to_id is None]
    if unassigned:
        completed = len([t for t in unassigned if t.status == TaskStatus.COMPLETED])
        overdue = len(
            [
                t
                for t in unassigned
                if t.due_date and t.due_date < today
                and t.status not in [TaskStatus.COMPLETED, TaskStatus.CANCELLED]
            ]
        )
        active = len(unassigned) - completed
        per_user.append(
            AnalyticsItem(
                user_id=None,
                user_name="Unassigned",
                total_tasks=len(unassigned),
                completed_tasks=completed,
                overdue_tasks=overdue,
                active_tasks=active,
            )
        )

    return AnalyticsSummary(
        per_user=per_user,
        total_users=len(users),
        total_projects=len(projects),
        total_tasks=len(tasks),
    )

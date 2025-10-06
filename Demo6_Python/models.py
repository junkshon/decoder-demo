from enum import Enum
from pydantic import BaseModel
from uuid import uuid4, UUID
from datetime import date, timedelta

# ---------- ENUMS ----------
class TaskStatus(str, Enum):
    NEW = "NEW"
    IN_PROGRESS = "IN_PROGRESS"
    BLOCKED = "BLOCKED"
    COMPLETED = "COMPLETED"
    CANCELLED = "CANCELLED"


class TaskPriority(str, Enum):
    LOW = "LOW"
    MEDIUM = "MEDIUM"
    HIGH = "HIGH"
    CRITICAL = "CRITICAL"


# ---------- MODELS ----------
class User(BaseModel):
    id: UUID
    name: str
    email: str


class Project(BaseModel):
    id: UUID
    name: str
    description: str
    start_date: date
    end_date: date | None = None
    owner_id: UUID | None = None


class Task(BaseModel):
    id: UUID
    title: str
    description: str
    due_date: date | None = None
    status: TaskStatus
    priority: TaskPriority
    project_id: UUID
    assigned_to_id: UUID | None = None


class AnalyticsItem(BaseModel):
    user_id: UUID | None = None
    user_name: str
    total_tasks: int
    completed_tasks: int
    overdue_tasks: int
    active_tasks: int


class AnalyticsSummary(BaseModel):
    per_user: list[AnalyticsItem]
    total_users: int
    total_projects: int
    total_tasks: int


# ---------- STATE ----------
class State:
    def __init__(self):
        self.users: dict[UUID, User] = {}
        self.projects: dict[UUID, Project] = {}
        self.tasks: dict[UUID, Task] = {}


state = State()


# ---------- SEED DEMO DATA ----------
def seed_demo_data():
    today = date.today()

    alice = User(id=uuid4(), name="Alice Ahmed", email="alice@example.com")
    ben = User(id=uuid4(), name="Ben Brown", email="ben@example.com")
    chloe = User(id=uuid4(), name="Chloe Chen", email="chloe@example.com")

    proj1 = Project(
        id=uuid4(),
        name="Customer Portal",
        description="Build a secure customer portal MVP.",
        start_date=today - timedelta(days=21),
        owner_id=alice.id,
    )

    proj2 = Project(
        id=uuid4(),
        name="Mobile App Rewrite",
        description="Rewrite legacy app with FastAPI backend.",
        start_date=today - timedelta(days=35),
        end_date=today + timedelta(days=30),
        owner_id=ben.id,
    )

    tasks = [
        Task(
            id=uuid4(),
            title="Design auth flow",
            description="Define user login journeys",
            due_date=today + timedelta(days=7),
            status=TaskStatus.IN_PROGRESS,
            priority=TaskPriority.HIGH,
            project_id=proj1.id,
            assigned_to_id=alice.id,
        ),
        Task(
            id=uuid4(),
            title="Implement login API",
            description="FastAPI + JWT",
            due_date=today + timedelta(days=3),
            status=TaskStatus.BLOCKED,
            priority=TaskPriority.CRITICAL,
            project_id=proj1.id,
            assigned_to_id=ben.id,
        ),
        Task(
            id=uuid4(),
            title="Add telemetry",
            description="Include basic logging",
            due_date=today - timedelta(days=2),
            status=TaskStatus.COMPLETED,
            priority=TaskPriority.MEDIUM,
            project_id=proj1.id,
            assigned_to_id=chloe.id,
        ),
        Task(
            id=uuid4(),
            title="Create React shell",
            description="Set up Vite frontend",
            due_date=today + timedelta(days=10),
            status=TaskStatus.NEW,
            priority=TaskPriority.MEDIUM,
            project_id=proj2.id,
            assigned_to_id=chloe.id,
        ),
        Task(
            id=uuid4(),
            title="Migrate settings",
            description="Move configs to SQLite",
            due_date=today - timedelta(days=1),
            status=TaskStatus.IN_PROGRESS,
            priority=TaskPriority.HIGH,
            project_id=proj2.id,
            assigned_to_id=ben.id,
        ),
    ]

    state.users = {u.id: u for u in [alice, ben, chloe]}
    state.projects = {p.id: p for p in [proj1, proj2]}
    state.tasks = {t.id: t for t in tasks}

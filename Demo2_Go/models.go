package main

import (
	"time"

	"gorm.io/gorm"
)

type TaskStatus string
type TaskPriority string

const (
	StatusNew        TaskStatus = "New"
	StatusInProgress TaskStatus = "InProgress"
	StatusBlocked    TaskStatus = "Blocked"
	StatusCompleted  TaskStatus = "Completed"
	StatusCancelled  TaskStatus = "Cancelled"

	PriorityLow      TaskPriority = "Low"
	PriorityMedium   TaskPriority = "Medium"
	PriorityHigh     TaskPriority = "High"
	PriorityCritical TaskPriority = "Critical"
)

type User struct {
	ID       string  `gorm:"primarykey" json:"id"`
	Name     string  `json:"name"`
	Email    string  `gorm:"unique" json:"email"`
	Projects []Project `gorm:"foreignKey:OwnerID" json:"projects,omitempty"`
	Tasks    []Task    `gorm:"foreignKey:AssignedToID" json:"tasks,omitempty"`
}

type Project struct {
	ID          string    `gorm:"primarykey" json:"id"`
	Name        string    `json:"name"`
	Description string    `json:"description"`
	StartDate   time.Time `json:"startDate"`
	EndDate     *time.Time `json:"endDate,omitempty"`
	OwnerID     *string   `json:"ownerId"`
	Owner       *User     `json:"owner,omitempty"`
	Tasks       []Task    `gorm:"foreignKey:ProjectID" json:"tasks,omitempty"`
}

type Task struct {
	ID           string        `gorm:"primarykey" json:"id"`
	Title        string        `json:"title"`
	Description  string        `json:"description"`
	DueDate      *time.Time    `json:"dueDate,omitempty"`
	Status       TaskStatus    `json:"status"`
	Priority     TaskPriority  `json:"priority"`
	ProjectID    string        `json:"projectId"`
	Project      *Project      `json:"project,omitempty"`
	AssignedToID *string       `json:"assignedToId,omitempty"`
	AssignedTo   *User         `json:"assignedTo,omitempty"`
}

type AnalyticsSummary struct {
	PerUser      []AnalyticsItem `json:"perUser"`
	TotalProjects int             `json:"totalProjects"`
	TotalUsers    int             `json:"totalUsers"`
	TotalTasks    int             `json:"totalTasks"`
}

type AnalyticsItem struct {
	UserID        *string `json:"userId,omitempty"`
	UserName      string  `json:"userName"`
	TotalTasks    int     `json:"totalTasks"`
	Completed     int     `json:"completedTasks"`
	Overdue       int     `json:"overdueTasks"`
	Active        int     `json:"activeTasks"`
}

func SeedDemoData(db *gorm.DB) error {
	var count int64
	db.Model(&User{}).Count(&count)
	if count > 0 {
		return nil
	}

	now := time.Now().UTC()

	alice := User{Name: "Alice Ahmed", Email: "alice@example.com"}
	ben := User{Name: "Ben Brown", Email: "ben@example.com"}
	chloe := User{Name: "Chloe Chen", Email: "chloe@example.com"}
	db.Create(&[]User{alice, ben, chloe})

	proj1 := Project{
		Name:        "Customer Portal",
		Description: "Build a secure customer portal MVP.",
		StartDate:   now.AddDate(0, 0, -21),
		Owner:       &alice,
	}
	proj2 := Project{
		Name:        "Mobile App Rewrite",
		Description: "Rewrite legacy app using Flutter.",
		StartDate:   now.AddDate(0, 0, -30),
		EndDate:     ptrTime(now.AddDate(0, 0, 30)),
		Owner:       &ben,
	}
	db.Create(&[]Project{proj1, proj2})

	tasks := []Task{
		{Title: "Design auth flow", Project: &proj1, Priority: PriorityHigh, Status: StatusInProgress, DueDate: ptrTime(now.AddDate(0, 0, 7)), AssignedTo: &alice},
		{Title: "Implement login API", Project: &proj1, Priority: PriorityCritical, Status: StatusBlocked, DueDate: ptrTime(now.AddDate(0, 0, 3)), AssignedTo: &ben},
		{Title: "Telemetry baseline", Project: &proj1, Priority: PriorityMedium, Status: StatusCompleted, DueDate: ptrTime(now.AddDate(0, 0, -2)), AssignedTo: &chloe},
		{Title: "Create app shell", Project: &proj2, Priority: PriorityMedium, Status: StatusNew, DueDate: ptrTime(now.AddDate(0, 0, 10)), AssignedTo: &chloe},
		{Title: "Migrate settings store", Project: &proj2, Priority: PriorityHigh, Status: StatusInProgress, DueDate: ptrTime(now.AddDate(0, 0, -1)), AssignedTo: &ben},
	}
	db.Create(&tasks)

	return nil
}

func ptrTime(t time.Time) *time.Time {
	return &t
}

package main

import (
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"gorm.io/gorm"
)

// --------------------- USERS ---------------------

func ListUsers(c *gin.Context) {
	var users []User
	db.Find(&users)
	c.JSON(http.StatusOK, users)
}

func GetUser(c *gin.Context) {
	var user User
	if err := db.First(&user, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "user not found"})
		return
	}
	c.JSON(http.StatusOK, user)
}

func CreateUser(c *gin.Context) {
	var input User
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	db.Create(&input)
	c.JSON(http.StatusCreated, input)
}

func UpdateUser(c *gin.Context) {
	var user User
	if err := db.First(&user, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "user not found"})
		return
	}
	var input User
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	user.Name = input.Name
	user.Email = input.Email
	db.Save(&user)
	c.JSON(http.StatusOK, user)
}

func DeleteUser(c *gin.Context) {
	db.Delete(&User{}, "id = ?", c.Param("id"))
	c.Status(http.StatusNoContent)
}

// --------------------- PROJECTS ---------------------

func ListProjects(c *gin.Context) {
	var projects []Project
	db.Preload("Owner").Preload("Tasks").Find(&projects)
	c.JSON(http.StatusOK, projects)
}

func GetProject(c *gin.Context) {
	var project Project
	if err := db.Preload("Owner").Preload("Tasks").First(&project, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "project not found"})
		return
	}
	c.JSON(http.StatusOK, project)
}

func CreateProject(c *gin.Context) {
	var input Project
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	db.Create(&input)
	c.JSON(http.StatusCreated, input)
}

func UpdateProject(c *gin.Context) {
	var project Project
	if err := db.First(&project, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "project not found"})
		return
	}
	var input Project
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	project.Name = input.Name
	project.Description = input.Description
	project.StartDate = input.StartDate
	project.EndDate = input.EndDate
	project.OwnerID = input.OwnerID
	db.Save(&project)
	c.JSON(http.StatusOK, project)
}

func DeleteProject(c *gin.Context) {
	db.Delete(&Project{}, "id = ?", c.Param("id"))
	c.Status(http.StatusNoContent)
}

// --------------------- TASKS ---------------------

func ListTasks(c *gin.Context) {
	var tasks []Task
	q := db.Preload("Project").Preload("AssignedTo")

	if status := c.Query("status"); status != "" {
		q = q.Where("status = ?", status)
	}
	if priority := c.Query("priority"); priority != "" {
		q = q.Where("priority = ?", priority)
	}
	q.Find(&tasks)
	c.JSON(http.StatusOK, tasks)
}

func GetTask(c *gin.Context) {
	var task Task
	if err := db.Preload("Project").Preload("AssignedTo").First(&task, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "task not found"})
		return
	}
	c.JSON(http.StatusOK, task)
}

func CreateTask(c *gin.Context) {
	var input Task
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	db.Create(&input)
	c.JSON(http.StatusCreated, input)
}

func UpdateTask(c *gin.Context) {
	var task Task
	if err := db.First(&task, "id = ?", c.Param("id")).Error; err != nil {
		c.JSON(http.StatusNotFound, gin.H{"error": "task not found"})
		return
	}
	var input Task
	if err := c.ShouldBindJSON(&input); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}
	task.Title = input.Title
	task.Description = input.Description
	task.DueDate = input.DueDate
	task.Status = input.Status
	task.Priority = input.Priority
	task.ProjectID = input.ProjectID
	task.AssignedToID = input.AssignedToID
	db.Save(&task)
	c.JSON(http.StatusOK, task)
}

func DeleteTask(c *gin.Context) {
	db.Delete(&Task{}, "id = ?", c.Param("id"))
	c.Status(http.StatusNoContent)
}

// --------------------- ANALYTICS ---------------------

func GetAnalytics(c *gin.Context) {
	var users []User
	var tasks []Task
	var projects []Project

	db.Find(&users)
	db.Find(&tasks)
	db.Find(&projects)

	now := time.Now().UTC()
	var perUser []AnalyticsItem

	for _, u := range users {
		userTasks := filterTasks(tasks, func(t Task) bool { return t.AssignedToID != nil && *t.AssignedToID == u.ID })
		completed := countTasks(userTasks, func(t Task) bool { return t.Status == StatusCompleted })
		overdue := countTasks(userTasks, func(t Task) bool {
			return t.DueDate != nil && t.DueDate.Before(now) && t.Status != StatusCompleted && t.Status != StatusCancelled
		})
		active := len(userTasks) - completed
		perUser = append(perUser, AnalyticsItem{
			UserID:   &u.ID,
			UserName: u.Name,
			TotalTasks: len(userTasks),
			Completed:  completed,
			Overdue:    overdue,
			Active:     active,
		})
	}

	unassigned := filterTasks(tasks, func(t Task) bool { return t.AssignedToID == nil })
	if len(unassigned) > 0 {
		completed := countTasks(unassigned, func(t Task) bool { return t.Status == StatusCompleted })
		overdue := countTasks(unassigned, func(t Task) bool {
			return t.DueDate != nil && t.DueDate.Before(now) && t.Status != StatusCompleted && t.Status != StatusCancelled
		})
		active := len(unassigned) - completed
		perUser = append(perUser, AnalyticsItem{
			UserID:   nil,
			UserName: "Unassigned",
			TotalTasks: len(unassigned),
			Completed:  completed,
			Overdue:    overdue,
			Active:     active,
		})
	}

	c.JSON(http.StatusOK, AnalyticsSummary{
		PerUser:       perUser,
		TotalProjects: len(projects),
		TotalUsers:    len(users),
		TotalTasks:    len(tasks),
	})
}

func filterTasks(tasks []Task, f func(Task) bool) []Task {
	var result []Task
	for _, t := range tasks {
		if f(t) {
			result = append(result, t)
		}
	}
	return result
}

func countTasks(tasks []Task, f func(Task) bool) int {
	count := 0
	for _, t := range tasks {
		if f(t) {
			count++
		}
	}
	return count
}

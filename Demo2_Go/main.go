package main

import (
	"log"
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"gorm.io/driver/sqlite"
	"gorm.io/gorm"
)

var db *gorm.DB

func main() {
	var err error
	db, err = gorm.Open(sqlite.Open("tasktracker.db"), &gorm.Config{})
	if err != nil {
		log.Fatalf("failed to connect database: %v", err)
	}

	// Auto migrate models
	err = db.AutoMigrate(&User{}, &Project{}, &Task{})
	if err != nil {
		log.Fatalf("failed migration: %v", err)
	}

	// Seed demo data
	if err := SeedDemoData(db); err != nil {
		log.Fatalf("failed to seed data: %v", err)
	}

	r := gin.Default()

	r.GET("/health", func(c *gin.Context) {
		c.JSON(http.StatusOK, gin.H{"status": "ok", "timeUtc": time.Now().UTC()})
	})

	// Users
	r.GET("/api/users", ListUsers)
	r.GET("/api/users/:id", GetUser)
	r.POST("/api/users", CreateUser)
	r.PUT("/api/users/:id", UpdateUser)
	r.DELETE("/api/users/:id", DeleteUser)

	// Projects
	r.GET("/api/projects", ListProjects)
	r.GET("/api/projects/:id", GetProject)
	r.POST("/api/projects", CreateProject)
	r.PUT("/api/projects/:id", UpdateProject)
	r.DELETE("/api/projects/:id", DeleteProject)

	// Tasks
	r.GET("/api/tasks", ListTasks)
	r.GET("/api/tasks/:id", GetTask)
	r.POST("/api/tasks", CreateTask)
	r.PUT("/api/tasks/:id", UpdateTask)
	r.DELETE("/api/tasks/:id", DeleteTask)

	// Analytics
	r.GET("/api/analytics/summary", GetAnalytics)

	log.Println("TaskTrackerPro-Go running on http://localhost:8080")
	if err := r.Run(":8080"); err != nil {
		log.Fatal(err)
	}
}

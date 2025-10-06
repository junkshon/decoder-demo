# Decoder Sample Apps

## Demo1_ASPNETCSHARP

### Technical Profile

- Framework: .NET 8
- Language: C# 12
- Database: SQLite via Entity Framework Core
- API Style: RESTful
- Includes: Dependency injection, async methods, basic LINQ, and data annotations

### App Details

- Has multiple namespaces and class relationships (great for dependency mapping)
- Includes async/await patterns and LINQ (useful for static and runtime analysis)
- Moderate size (15–20 classes, ~1,000 LOC)
- Contains real-world structure and logic

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo2_ASPNETCSHARP

## Demo2_Go

### Technical Profile

Language: Go 1.22+
Framework: Gin for HTTP
ORM: GORM with SQLite (file-based DB for realism)

### App Details 

• main.go
• models.go
• handlers.go

CRUD + filtering + analytics

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo2_Go

## Demo3_Java

### Technical Profile

- Language: Java 21+                                                              
- Framework:  Spring Boot 3.x                                                       
- Database: H2 in-memory (auto-created)                                           
- ORM: Spring Data JPA (Hibernate)                                           


### Application Details

- CRUD endpoints for Users, Projects, and Tasks
- /api/analytics/summary endpoint with simple aggregation
- JPA relationships with @OneToMany, @ManyToOne
- Data seeded on startup
- JSON serialisation handled automatically via Jackson

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo3_Java

## Demo4_WebReact

### Technical Profile

- Language: React (JavaScript or TypeScript – I’ll use **JavaScript** for simplicity) 
- Tooling: Vite (fast dev server & build)                                            
- UI Library: Tailwind CSS (lightweight and modern)                                     
- Components: Dashboard, User list, Project list, Task list, Analytics summary          
- API: Configurable base URL (default `http://localhost:5000` or `:8080`)        

### Application Details
A React app that interacts with a REST API (so you can pair it with any of the backends above).

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo4_WebReact

## Demo5_Rust

### Technical Profile

- Language: Rust (edition 2021 or 2024)                                           
- Framework: [Axum](https://github.com/tokio-rs/axum) (modern async web framework)
- Database: In-memory (via `Arc<Mutex<Vec<T>>>`) — no external deps               
- Features: CRUD APIs for users, projects, tasks + analytics endpoint             

### Application Details

- Pure Rust async app — no DB dependency
- Contains modules, structs, enums, Arc<Mutex> state, and JSON I/O
- Demonstrates strong typing, ownership, and concurrency patterns
- ~600 LOC across 3 files — ideal for static analysis demos

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo5_Rust

## Demo6_Python

### Technical Profile


- Language: Python 3.11+                                                          
- Framework: [FastAPI](https://fastapi.tiangolo.com/)                              
- Database: In-memory (using `dict` and `uuid`)                                   
- Features: CRUD APIs for Users, Projects, and Tasks + Analytics summary          

### Application Details

- Realistic yet lightweight API surface for your analysis tool
- Uses modern FastAPI idioms: routers, models, validation, enums, Pydantic
- Contains multiple files for upload and dependency analysis
- No database — fast startup and reset between runs
- Roughly 350–400 lines, ideal for performance and inspection

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo6_Python


## Demo7_NodeJS

### Technical Profile

- Language: JavaScript (Node.js 20+)                                     
- Framework: [Express](https://expressjs.com/)                            
- Database: In-memory (using arrays & UUIDs)                             

### Application Details

- CRUD APIs for Users, Projects, Tasks + Analytics Summary
- Uses modern ES modules and idiomatic Express 5 syntax
- Includes routing, modular separation, and JSON handling
- Matches entities (User, Project, Task) with the other demos
- In-memory storage for instant startup
- Small (≈300 LOC) but rich enough for dependency & request/response analysis

### Repo URL

You can drop this URL into the GitHub Repository URL inside the application:

https://github.com/junkshon/decoder-demo/tree/main/Demo7_NodeJS

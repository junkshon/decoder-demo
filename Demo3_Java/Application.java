package com.example.tasktrackerpro;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import java.time.LocalDate;

@SpringBootApplication
public class Application {

    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }

    // Seed demo data at startup
    @Bean
    CommandLineRunner initData(UserRepository userRepo, ProjectRepository projectRepo, TaskRepository taskRepo) {
        return args -> {
            if (userRepo.count() > 0) return;

            var alice = new User("Alice Ahmed", "alice@example.com");
            var ben = new User("Ben Brown", "ben@example.com");
            var chloe = new User("Chloe Chen", "chloe@example.com");
            userRepo.save(alice);
            userRepo.save(ben);
            userRepo.save(chloe);

            var proj1 = new Project("Customer Portal", "Build secure customer portal MVP",
                    LocalDate.now().minusDays(21), null, alice);
            var proj2 = new Project("Mobile App Rewrite", "Rewrite legacy app using Flutter",
                    LocalDate.now().minusDays(35), LocalDate.now().plusDays(30), ben);
            projectRepo.save(proj1);
            projectRepo.save(proj2);

            taskRepo.save(new Task("Design auth flow", "User journey design",
                    LocalDate.now().plusDays(7), TaskStatus.IN_PROGRESS, TaskPriority.HIGH, proj1, alice));
            taskRepo.save(new Task("Implement login API", "Handle JWT auth",
                    LocalDate.now().plusDays(3), TaskStatus.BLOCKED, TaskPriority.CRITICAL, proj1, ben));
            taskRepo.save(new Task("Telemetry baseline", "Add initial observability",
                    LocalDate.now().minusDays(2), TaskStatus.COMPLETED, TaskPriority.MEDIUM, proj1, chloe));
            taskRepo.save(new Task("Create Flutter shell", "Scaffold project structure",
                    LocalDate.now().plusDays(10), TaskStatus.NEW, TaskPriority.MEDIUM, proj2, chloe));
            taskRepo.save(new Task("Migrate settings", "Move from SharedPrefs to SQLite",
                    LocalDate.now().minusDays(1), TaskStatus.IN_PROGRESS, TaskPriority.HIGH, proj2, ben));
        };
    }
}

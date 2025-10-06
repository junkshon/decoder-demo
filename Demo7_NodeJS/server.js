import express from "express";
import bodyParser from "body-parser";
import { router } from "./routes.js";
import { seedDemoData } from "./models.js";

const app = express();
const PORT = process.env.PORT || 5000;

// Middleware
app.use(bodyParser.json());

// Health check
app.get("/health", (req, res) => {
  res.json({ status: "ok", timeUtc: new Date().toISOString() });
});

// Root
app.get("/", (req, res) => {
  res.json({ message: "Welcome to TaskTrackerPro Node.js API" });
});

// Seed demo data
seedDemoData();

// Mount routes
app.use("/api", router);

// Start server
app.listen(PORT, () => {
  console.log(`🚀 TaskTrackerPro-Node running on http://localhost:${PORT}`);
});

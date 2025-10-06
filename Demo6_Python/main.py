from fastapi import FastAPI
from routes import router
from models import seed_demo_data, state

app = FastAPI(title="TaskTrackerPro-Py", version="1.0")

# Seed in-memory data
seed_demo_data()

# Include API routes
app.include_router(router, prefix="/api")

@app.get("/health")
def health():
    from datetime import datetime
    return {"status": "ok", "timeUtc": datetime.utcnow().isoformat()}

@app.get("/")
def index():
    return {"message": "Welcome to TaskTrackerPro Python API"}

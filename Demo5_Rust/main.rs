mod models;
mod routes;

use axum::{routing::get, Router};
use routes::*;
use std::net::SocketAddr;
use once_cell::sync::Lazy;
use std::sync::Mutex;

use crate::models::{seed_demo_data, AppState};

// Shared in-memory store
pub static APP_STATE: Lazy<Mutex<AppState>> = Lazy::new(|| Mutex::new(seed_demo_data()));

#[tokio::main]
async fn main() {
    let app = Router::new()
        .route("/health", get(health))
        .nest("/api", api_routes());

    let addr = SocketAddr::from(([127, 0, 0, 1], 8080));
    println!("🚀 TaskTrackerPro-RS running on http://{addr}");
    axum::serve(tokio::net::TcpListener::bind(addr).await.unwrap(), app)
        .await
        .unwrap();
}

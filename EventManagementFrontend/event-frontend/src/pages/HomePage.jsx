import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";

export default function HomePage() {
  const navigate = useNavigate();
  const auth = useSelector((s) => s.auth);

  // Auto-redirect if already logged in
  useEffect(() => {
    if (auth.token && auth.role) {
      if (auth.role === "Admin") {
        navigate("/admin/dashboard");
      } else {
        navigate("/user/dashboard");
      }
    }
  }, [auth, navigate]);

  return (
    <div style={{
      minHeight: "100vh",
      display: "flex",
      flexDirection: "column",
      justifyContent: "center",
      alignItems: "center",
      background:  "#59abafff ",
      color: "#fff",
      textAlign: "center",
      padding: 20
    }}>
      <h1 style={{ fontSize: 48, marginBottom: 20 }}>
        Event Management System
      </h1>
      
      <p style={{ fontSize: 20, marginBottom: 40, maxWidth: 600 }}>
        Discover events, register and manage.
      </p>

      <div style={{ display: "flex", gap: 20 }}>
        <button
          onClick={() => navigate("/login")}
          style={{
            padding: "15px 40px",
            fontSize: 18,
            background: "#121b19ff",
            color: "#fff",
            border: "none",
            borderRadius: 8,
            cursor: "pointer",
            fontWeight: "bold",
            boxShadow: "0 4px 6px rgba(0,0,0,0.1)"
          }}
        >
          Get Started
        </button>
      </div>

      <div style={{
        marginTop: 60,
        display: "grid",
        gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
        gap: 20,
        maxWidth: 900,
        width: "100%"
      }}>
        <div style={{
          background: "#e8f5c0ff",
          color: "#000000",
          padding: 20,
          borderRadius: 8,
          backdropFilter: "blur(10px)"
        }}>
          <h3>Browse Events</h3>
          
        </div>

        <div style={{
          background: "#e8f5c0ff",
          color: "#000000",
          padding: 20,
          borderRadius: 8,
          backdropFilter: "blur(10px)"
        }}>
          <h3> Registrations</h3>
          
        </div>

        <div style={{
          background: "#e8f5c0ff",
          color: "#000000",
          padding: 20,
          borderRadius: 8,
          backdropFilter: "blur(10px)"
        }}>
          <h3>Manage Events</h3>
          
        </div>
      </div>
    </div>
  );
}
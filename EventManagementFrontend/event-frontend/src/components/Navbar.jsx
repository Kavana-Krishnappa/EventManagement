import React from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate, Link } from "react-router-dom";
import { logout } from "../features/auth/authSlice";

export default function Navbar() {
  //get auth state
  const auth = useSelector((s) => s.auth); 
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLogout = () => {
    dispatch(logout());
    navigate("/login");
  };

  if (!auth.token) return null;

  return (
    <nav
      style={{
        background: "linear-gradient(90deg, #283048, #6892a1ff)",
        padding: "16px 28px",
        color: "white",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      
        fontFamily: "Inter, sans-serif",
      }}
    >
      {/* Project Name */}
      <div style={{ display: "flex", gap: 24, alignItems: "center" }}>
        <h2 style={{ margin: 0, fontSize: 22, fontWeight: 700 }}>EventBase</h2>

        {/* USER ROUTES */}
        {auth.role === "User" && (
          <>
            <NavItem to="/user/dashboard" label="Dashboard" />
            <NavItem to="/events" label="Calendar" />
          </>
        )}

        {/* ADMIN ROUTES */}
        {auth.role === "Admin" && (
          <>
            <NavItem to="/admin/dashboard" label="Dashboard" />
            <NavItem to="/admin/participants" label="Participants" />
          </>
        )}
      </div>

      {/* Right Section */}
      <div style={{ display: "flex", gap: 16, alignItems: "center" }}>
        
        {/* User Badge */}
        <span
          style={{
            background: "rgba(255,255,255,0.15)",
            padding: "6px 14px",
            borderRadius: 20,
            fontSize: 14,
            backdropFilter: "blur(4px)",
          }}
        >
          {auth.profile?.fullName || auth.profile?.email}
        </span>

        {/* Logout Button */}
        <button
          onClick={handleLogout}
          style={{
            padding: "8px 18px",
            background: "#df7272ff",
            border: "none",
            borderRadius: 20,
            color: "#fff",
            cursor: "pointer",
            fontWeight: 600,
            transition: "0.25s",
          }}
          onMouseOver={(e) =>
            (e.target.style.background = "#ff4d4d")
          }
          onMouseOut={(e) =>
            (e.target.style.background = "#ff7070")
          }
        >
          Logout
        </button>
      </div>
    </nav>
  );
}

// Reusable nav item component
function NavItem({ to, label }) {
  return (
    <Link
      to={to}
      style={{
        color: "#fff",
        textDecoration: "none",
        fontSize: 15,
        fontWeight: 500,
        padding: "6px 12px",
        borderRadius: 8,
        transition: "0.25s",
      }}
      onMouseOver={(e) => {
        e.target.style.background = "rgba(255,255,255,0.18)";
      }}
      onMouseOut={(e) => {
        e.target.style.background = "transparent";
      }}
    >
      {label}
    </Link>
  );
}

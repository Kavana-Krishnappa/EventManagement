import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useSelector } from "react-redux";

import ProtectedRoute from "./components/ProtectedRoute";
import Navbar from "./components/Navbar";

// Pages
import LoginPage from "./features/auth/LoginPage";
import HomePage from "./pages/HomePage";

// Admin Pages
import AdminDashboard from "./pages/Admin/AdminDashboard";

// User Pages
import UserDashboard from "./pages/User/UserDashboard";
import UserEvents from "./pages/User/UserEvents";
import EventDetails from "./pages/Events/EventDetails";

import AdminParticipants from "./pages/Admin/AdminParticipants";

import SignupPage from "./features/auth/SignupPage";

function App() {
  const auth = useSelector((s) => s.auth);

  return (
    <Router>
      <Navbar />
      <Routes>
        {/* Public Route */}
        <Route path="/" element={<HomePage />} />

        <Route 
          path="/login" 
          element={
            auth.token ? (
              auth.role === "Admin" ? (
                <Navigate to="/admin/dashboard" replace />
              ) : (
                <Navigate to="/events" replace />
              )
            ) : (
              <LoginPage />
            )
          } 
        />

        {/* User Protected Routes */}
        <Route
          path="/user/dashboard"
          element={
            <ProtectedRoute requiredRole="user">
              <UserDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/events"
          element={
            <ProtectedRoute requiredRole="user">
              <UserEvents />
            </ProtectedRoute>
          }
        />
        <Route
          path="/events/:id"
          element={
            <ProtectedRoute requiredRole="user">
              <EventDetails />
            </ProtectedRoute>
          }
        />

        {/* Admin Protected Routes */}
<Route
  path="/admin/dashboard"
  element={
    <ProtectedRoute requiredRole="admin">
      <AdminDashboard />
    </ProtectedRoute>
  }
/>
{/* Signup Route */}
<Route 
  path="/signup" 
  element={
    auth.token ? (
      auth.role === "Admin" ? (
        <Navigate to="/admin/dashboard" replace />
      ) : (
        <Navigate to="/events" replace />
      )
    ) : (
      <SignupPage />
    )
  } 
/>


<Route
  path="/admin/participants"
  element={
    <ProtectedRoute requiredRole="admin">
      <AdminParticipants />
    </ProtectedRoute>
  }
/>

        <Route
          path="/admin/dashboard"
          element={
            <ProtectedRoute requiredRole="admin">
              <AdminDashboard />
            </ProtectedRoute>
          }
        />

        {/* Fallback - redirect to home if route not found */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Router>
  );
}

export default App;
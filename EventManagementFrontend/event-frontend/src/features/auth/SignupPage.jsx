import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { signupParticipant, clearSignupState } from "./signupSlice";
import { useNavigate, Link } from "react-router-dom";

export default function SignupPage() {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { loading, error, success } = useSelector((s) => s.signup);

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    password: "",
    confirmPassword: ""
  });

       const [validationError, setValidationError] = useState("");

  useEffect(() => {
    dispatch(clearSignupState());
  }, [dispatch]);

  useEffect(() => {
    
    if (success) {
      alert(" Signup successful! Please login with your credentials.");
      navigate("/login");
    }
  }, [success, navigate]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setValidationError(""); 
  };

  //validation
  const validateForm = () => {
    
    if (!form.fullName || !form.email || !form.phoneNumber || !form.password || !form.confirmPassword) {
      setValidationError("All fields are required");
      return false;
    }

    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(form.email)) {
      setValidationError("Invalid email format");
      return false;
    }

    
    const phoneRegex = /^\d{10}$/;
    if (!phoneRegex.test(form.phoneNumber.replace(/\D/g, ""))) {
      setValidationError("Phone number must be 10 digits");
      return false;
    }

 
    if (form.password.length < 6) {
      setValidationError("Password must be at least 6 characters");
      return false;
    }

    if (form.password !== form.confirmPassword) {
      setValidationError("Passwords do not match");
      return false;
    }

    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      await dispatch(signupParticipant({
        fullName: form.fullName,
        email: form.email,
        phoneNumber: form.phoneNumber,
        password: form.password
      })).unwrap();
    } catch (err) {
      console.error("Signup failed:", err);
    }
  };

  return (
    <div style={{ 
      minHeight: "100vh",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
      padding: 20
    }}>
      <div style={{ 
        maxWidth: 480, 
        width: "100%",
        background: "#fff",
        padding: 40, 
        borderRadius: 12,
        boxShadow: "0 10px 25px rgba(0,0,0,0.2)"
      }}>
        <h2 style={{ textAlign: "center", marginBottom: 10, color: "#333" }}>
          Create Account
        </h2>
        <p style={{ textAlign: "center", color: "#666", marginBottom: 30 }}>
          Sign up to start registering for events
        </p>

        <form onSubmit={handleSubmit} style={{ display: "grid", gap: 15 }}>
          
          <div>
            <label style={{ display: "block", marginBottom: 5, fontWeight: "bold", color: "#333" }}>
              Full Name *
            </label>
            <input
              type="text"
              name="fullName"
              value={form.fullName}
              onChange={handleChange}
              required
              style={{ 
                width: "100%", 
                padding: 12, 
                borderRadius: 6, 
                border: "1px solid #ddd",
                fontSize: 14
              }}
            />
          </div>

          
          <div>
            <label style={{ display: "block", marginBottom: 5, fontWeight: "bold", color: "#333" }}>
              Email *
            </label>
            <input
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
             
              required
              style={{ 
                width: "100%", 
                padding: 12, 
                borderRadius: 6, 
                border: "1px solid #ddd",
                fontSize: 14
              }}
            />
          </div>

        
          <div>
            <label style={{ display: "block", marginBottom: 5, fontWeight: "bold", color: "#333" }}>
              Phone Number *
            </label>
            <input
              type="tel"
              name="phoneNumber"
              value={form.phoneNumber}
              onChange={handleChange}
              
              required
              style={{ 
                width: "100%", 
                padding: 12, 
                borderRadius: 6, 
                border: "1px solid #ddd",
                fontSize: 14
              }}
            />
          </div>

        
          <div>
            <label style={{ display: "block", marginBottom: 5, fontWeight: "bold", color: "#333" }}>
              Password *
            </label>
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
           
              required
              style={{ 
                width: "100%", 
                padding: 12, 
                borderRadius: 6, 
                border: "1px solid #ddd",
                fontSize: 14
              }}
            />
          </div>

         
          <div>
            <label style={{ display: "block", marginBottom: 5, fontWeight: "bold", color: "#333" }}>
              Confirm Password *
            </label>
            <input
              type="password"
              name="confirmPassword"
              value={form.confirmPassword}
              onChange={handleChange}
             
              required
              style={{ 
                width: "100%", 
                padding: 12, 
                borderRadius: 6, 
                border: "1px solid #ddd",
                fontSize: 14
              }}
            />
          </div>

          {/* Validation Error */}
          {validationError && (
            <div style={{ 
              color: "#dc3545", 
              background: "#f8d7da", 
              padding: 10, 
              borderRadius: 6,
              border: "1px solid #f5c6cb"
            }}>
              {validationError}
            </div>
          )}

          {/* Backend Error */}
          {error && (
            <div style={{ 
              color: "#dc3545", 
              background: "#f8d7da", 
              padding: 10, 
              borderRadius: 6,
              border: "1px solid #f5c6cb"
            }}>
              {typeof error === "string" ? error : JSON.stringify(error)}
            </div>
          )}

          {/* Loading State */}
          {loading && (
            <div style={{ textAlign: "center", color: "#667eea" }}>
              Creating account...
            </div>
          )}

          {/* Submit Button */}
          <button 
            type="submit" 
            disabled={loading}
            style={{ 
              padding: 14, 
              background: loading ? "#ccc" : "#667eea", 
              color: "#fff", 
              border: "none", 
              borderRadius: 6, 
              cursor: loading ? "not-allowed" : "pointer",
              fontSize: 16,
              fontWeight: "bold",
              marginTop: 10
            }}
          >
            {loading ? "Creating Account..." : "Sign Up"}
          </button>
        </form>

        {/* Login Link */}
        <div style={{ textAlign: "center", marginTop: 20, color: "#666" }}>
          Already have an account?{" "}
          <Link 
            to="/login" 
            style={{ color: "#667eea", fontWeight: "bold", textDecoration: "none" }}
          >
            Login here
          </Link>
        </div>
      </div>
    </div>
  );
}
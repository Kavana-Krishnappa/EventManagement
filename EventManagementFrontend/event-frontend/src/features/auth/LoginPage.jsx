import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { login } from "./authSlice";
import {useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";

export default function LoginPage(){
    const dispatch = useDispatch();
     const navigate = useNavigate();
     const auth = useSelector((s) => s.auth);

     const[role, setRole ] = useState("User");
     const[email, setEmail] = useState("");
     const[password, setPassword] = useState("");

     const handleRoleClick = (r) => setRole(r);

     const handleSubmit = async (e) =>{
        e.preventDefault(); //prevent page reload
        try{
            const result = await dispatch(login({email, password,role})).unwrap();

            if(result.role === "Admin"){
                navigate("/admin/dashboard");
            } else{
                navigate("/user/dashboard");
            }
        } catch(err){
            console.error("Login failed:",err);
            
        }
     };

     
  return (
    <div style={{ maxWidth: 480, margin: "40px auto", padding: 20, border: "1px solid #ddd", borderRadius: 8 }}>
      <h2 style={{ textAlign: "center" }}>Sign in</h2>

      <div style={{ display: "flex", justifyContent: "center", gap: 12, marginBottom: 16 }}>
        <button
          type="button"
          onClick={() => handleRoleClick("User")}
          style={{
            padding: "8px 14px",
            borderRadius: 6,
            border: role === "User" ? "2px solid #111" : "1px solid #ccc",
            background: role === "User" ? "#f5f5f5" : "#fff",
            cursor: "pointer"
          }}
        >
          User
        </button>

        <button
          type="button"
          onClick={() => handleRoleClick("Admin")}
          style={{
            padding: "8px 14px",
            borderRadius: 6,
            border: role === "Admin" ? "2px solid #111" : "1px solid #ccc",
            background: role === "Admin" ? "#f5f5f5" : "#fff",
            cursor: "pointer"
          }}
        >
          Admin
        </button>
      </div>

      <form onSubmit={handleSubmit} style={{ display: "grid", gap: 10 }}>
        <label htmlFor="email">Email</label>
        <input
        id="email"
          type="email"
          value={email}
          onChange={(e)=>setEmail(e.target.value)}
          required
          style={{ padding: 8, borderRadius: 4, border: "1px solid #ccc" }}
        />

        <label htmlFor="password">Password</label>
        <input
        id="password"
          type="password"
          value={password}
          onChange={(e)=>setPassword(e.target.value)}
          required
          style={{ padding: 8, borderRadius: 4, border: "1px solid #ccc" }}
        />

        {auth.error && (
  <div style={{ color: "red" }}>
    {typeof auth.error === "string"
      ? auth.error
      : JSON.stringify(auth.error)}
  </div>
)}

        {auth.loading && <div>Logging in...</div>}

        <button type="submit" style={{ padding: 10, background: "#111", color: "#fff", border: "none", borderRadius: 4 }}>
          Login
        </button>
        <div style={{ textAlign: "center", marginTop: 20, color: "#666" }}>
  Don't have an account?{" "}
  <Link 
    to="/signup" 
    style={{ color: "#667eea", fontWeight: "bold", textDecoration: "none" }}
  >
    Sign up here
  </Link>
</div>
      </form>
      
    </div>

    
  );

     


}
import { useSelector } from "react-redux";
import { Navigate } from "react-router-dom";


//child- component you wanna protect
export default function ProtectedRoute({ children, requiredRole }) {
  const auth = useSelector((s) => s.auth);

  // Redirect if not logged in
  if (!auth.token) {
    return <Navigate to="/login" replace />;
  }

  //lowercase
  const userRole = auth.role ? String(auth.role).toLowerCase() : "";
  const reqRole = requiredRole ? String(requiredRole).toLowerCase() : "";

  // redirect 
  if (userRole !== reqRole) {
    
    if (userRole === "admin") {
      return <Navigate to="/admin/dashboard" replace />;
    }
    
    if (userRole === "user") {
      return <Navigate to="/user/dashboard" replace />;
    }
    
    return <Navigate to="/login" replace />;
  }

  return children;
}
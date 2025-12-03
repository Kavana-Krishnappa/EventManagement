import React, { useEffect, useState, useCallback } from "react"; // Add useCallback
import { useParams, useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import eventApi from "../../api/eventApi";
import { registerForEvent } from "../../features/registrations/registrationsSlice";
import EventCard from "../../components/events/EventCard";

export default function EventDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  
  const auth = useSelector((s) => s.auth);
  const { loading: registering } = useSelector((s) => s.registrations);
  
  const [event, setEvent] = useState(null);
  const [capacityInfo, setCapacityInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  
  const fetchEventData = useCallback(async () => {
    try {
      setLoading(true);
     
      const eventRes = await eventApi.getById(id);
      setEvent(eventRes.data);
      
      // Fetch capacity info
      const capacityRes = await eventApi.getCapacity(id);
      setCapacityInfo(capacityRes.data);
      
      setError(null);
    } catch (err) {
      console.error("Failed to fetch event", err);
      setError("Failed to load event details");
    } finally {
      setLoading(false);
    }
  }, [id]); 

  useEffect(() => {
    fetchEventData();
  }, [fetchEventData]); 

  const handleRegister = async () => {
    
    if (!auth.token) {
      alert("Please log in to register for events");
      navigate("/login");
      return;
    }

   
    if (!auth.profile?.participantId) {
      alert("Only participants can register for events");
      return;
    }

   
    if (capacityInfo?.isFull) {
      alert("Sorry! This event has reached maximum capacity.");
      return;
    }

    try {
      await dispatch(registerForEvent({
        eventId: parseInt(id),
        participantId: auth.profile.participantId
      })).unwrap();
      
      alert("Successfully registered for this event!");
      
      // Refresh capacity info after registration
      await fetchEventData();
      

      setTimeout(() => {
        navigate("/user/dashboard");
      }, 1000);
    } catch (err) {
      console.error("Registration failed:", err);
  
      let errorMessage = "Registration failed. Please try again.";
      
      if (typeof err === 'string') {
        errorMessage = err;
      } else if (err?.message) {
        errorMessage = err.message;
      }
     
      if (errorMessage.includes("already registered")) {
        alert("You are already registered for this event");
        await fetchEventData(); // Refresh to update UI
      } else if (errorMessage.includes("maximum capacity")) {
        alert(" Sorry, this event has reached maximum capacity");
        await fetchEventData(); // Refresh to update UI
      } else {
        alert("Error: " + errorMessage);
      }
    }
  };


  if (loading) {
    return (
      <div style={{ padding: 40, textAlign: "center" }}>
        <div style={{
          width: 50,
          height: 50,
          border: "5px solid #f3f3f3",
          borderTop: "5px solid #667eea",
          borderRadius: "50%",
          animation: "spin 1s linear infinite",
          margin: "0 auto"
        }} />
        <p style={{ marginTop: 20 }}>Loading event details...</p>
        <style>
          {`
            @keyframes spin {
              0% { transform: rotate(0deg); }
              100% { transform: rotate(360deg); }
            }
          `}
        </style>
      </div>
    );
  }

  // Error state
  if (error) {
    return (
      <div style={{ padding: 20, textAlign: "center" }}>
        <p style={{ color: "red" }}>{error}</p>
        <button
          onClick={() => navigate("/events")}
          style={{
            marginTop: 10,
            padding: "10px 20px",
            background: "#6c757d",
            color: "#fff",
            border: "none",
            borderRadius: 4,
            cursor: "pointer"
          }}
        >
          Back to Events
        </button>
      </div>
    );
  }

  // No event found
  if (!event) {
    return (
      <div style={{ padding: 20, textAlign: "center" }}>
        <p>Event not found</p>
        <button
          onClick={() => navigate("/events")}
          style={{
            marginTop: 10,
            padding: "10px 20px",
            background: "#6c757d",
            color: "#fff",
            border: "none",
            borderRadius: 4,
            cursor: "pointer"
          }}
        >
          Back to Events
        </button>
      </div>
    );
  }

  const isFull = capacityInfo?.isFull || false;
  const isPastEvent = new Date(event.eventDate) < new Date();


  return (
    <div style={{ padding: 20, maxWidth: 800, margin: "0 auto" }}>
      <h1>Event Details</h1>
      
      <EventCard event={event} />

      {/* Capacity Info */}
      {capacityInfo && (
        <div style={{
          marginTop: 15,
          padding: 15,
          background: isFull ? "#fff3cd" : "#d4edda",
          border: `2px solid ${isFull ? "#ffc107" : "#28a745"}`,
          borderRadius: 8,
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center"
        }}>
          <div>
            <span style={{ fontWeight: "bold", fontSize: 16 }}>
              Registrations: {capacityInfo.currentRegistrations} / {capacityInfo.maxCapacity}
            </span>
            {!isFull && (
              <span style={{ marginLeft: 15, color: "#666", fontSize: 14 }}>
                ({capacityInfo.availableSpots} spots left)
              </span>
            )}
          </div>
          {isFull && (
            <span style={{ 
              background: "#dc3545", 
              color: "#fff", 
              padding: "6px 12px", 
              borderRadius: 4,
              fontSize: 14,
              fontWeight: "bold"
            }}>
              FULL
            </span>
          )}
        </div>
      )}

      {/* Action Buttons */}
      <div style={{ marginTop: 20, display: "flex", gap: 10, flexWrap: "wrap" }}>
        {/* Show Register button only for Users */}
        {auth.role === "User" && (
          <button
            onClick={handleRegister}
            disabled={registering || isFull || isPastEvent}
            style={{
              padding: "14px 28px",
              background: 
                isFull ? "#dc3545" :
                isPastEvent ? "#6c757d" :
                registering ? "#ccc" : "#28a745",
              color: "#fff",
              border: "none",
              borderRadius: 6,
              cursor: (registering || isFull || isPastEvent) ? "not-allowed" : "pointer",
              fontSize: 16,
              fontWeight: "bold",
              opacity: (isFull || isPastEvent) ? 0.8 : 1,
              boxShadow: (isFull || isPastEvent) ? "none" : "0 2px 4px rgba(0,0,0,0.1)"
            }}
          >
            {registering ? "Registering..." :
             isFull ? " Event Full" :
             isPastEvent ? "Event Ended" :
             " Register for Event"}
          </button>
        )}

        <button
          onClick={() => navigate(auth.role === "Admin" ? "/admin/dashboard" : "/user/dashboard")}
          style={{
            padding: "14px 28px",
            background: "#6c757d",
            color: "#fff",
            border: "none",
            borderRadius: 6,
            cursor: "pointer",
            fontSize: 16,
            fontWeight: "bold"
          }}
        >
          ← Back to {auth.role === "Admin" ? "Dashboard" : "My Events"}
        </button>
      </div>

      {/* Status Messages */}
      {isPastEvent && (
        <div style={{
          marginTop: 20,
          padding: 15,
          background: "#fff3cd",
          border: "1px solid #ffc107",
          borderRadius: 8,
          color: "#856404",
          fontSize: 15
        }}>
          This event has already passed
        </div>
      )}

      {isFull && !isPastEvent && (
        <div style={{
          marginTop: 20,
          padding: 15,
          background: "#f8d7da",
          border: "1px solid #dc3545",
          borderRadius: 8,
          color: "#721c24",
          fontSize: 15
        }}>
          This event has reached maximum capacity. Registration is closed.
        </div>
      )}
    </div>
  );
}
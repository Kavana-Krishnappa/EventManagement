import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { fetchEvents } from "../../features/events/eventsSlice";
import participantApi from "../../api/participantApi";

export default function UserDashboard() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const auth = useSelector((s) => s.auth);
  const { list: allEvents } = useSelector((s) => s.events);

  const [myRegisteredEvents, setMyRegisteredEvents] = useState([]);
  const [loading, setLoading] = useState(true);

 
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);

        // Fetch all events
        await dispatch(fetchEvents());

        if (auth.profile?.participantId) {
          const participantId = auth.profile.participantId;

          
          const upcomingRes = await participantApi.getUpcomingEvents(participantId);

          
          const previousRes = await participantApi.getPreviousEvents(participantId);

          
          setMyRegisteredEvents([
            ...upcomingRes.data,
            ...previousRes.data
          ]);
        }

      } catch (err) {
        console.error("Failed to load dashboard data:", err);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [dispatch, auth.profile]);

  
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
        <p style={{ marginTop: 20 }}>Loading your dashboard...</p>
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

  // ----------------------------------------
  // SEPARATE PAST & UPCOMING EVENTS
  // ----------------------------------------
  const now = new Date();
  const upcomingRegistered = myRegisteredEvents.filter(e => new Date(e.eventDate) >= now);
  const pastRegistered = myRegisteredEvents.filter(e => new Date(e.eventDate) < now);

  // All available upcoming events
  const allUpcomingEvents = allEvents.filter(e => new Date(e.eventDate) >= now);

  return (
    <div style={{ padding: 20, maxWidth: 1400, margin: "0 auto" }}>

      {/* Welcome Header */}
      <div style={{ marginBottom: 30 }}>
        <h1 style={{ margin: 0, fontSize: 32 }}>
          Welcome, {auth.profile?.fullName}!
        </h1>
      </div>

      {/* Stats Cards */}
      <div style={{
        display: "grid",
        gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
        gap: 20,
        marginBottom: 40
      }}>
        <div style={{
          background: "#97C5BB",
          color: "#1d1a1aff",
          padding: 25,
          borderRadius: 12,
          boxShadow: "0 4px 6px rgba(0,0,0,0.1)"
        }}>
          <h3 style={{ margin: 0, fontSize: 14 }}>Upcoming Events (Registered)</h3>
          <p style={{ fontSize: 42, fontWeight: "bold" }}>
            {upcomingRegistered.length}
          </p>
        </div>

        <div style={{
          background: "#97C5BB",
          color: "#1d1a1aff",
          padding: 25,
          borderRadius: 12,
          boxShadow: "0 4px 6px rgba(0,0,0,0.1)"
        }}>
          <h3 style={{ margin: 0, fontSize: 14 }}>Past Events (Registered)</h3>
          <p style={{ fontSize: 42, fontWeight: "bold" }}>
            {pastRegistered.length}
          </p>
        </div>

        <div style={{
          background: "#97C5BB",
          color: "#1d1a1aff",
          padding: 25,
          borderRadius: 12,
          boxShadow: "0 4px 6px rgba(0,0,0,0.1)"
        }}>
          <h3 style={{ margin: 0, fontSize: 14 }}>Available Events</h3>
          <p style={{ fontSize: 42, fontWeight: "bold" }}>
            {allUpcomingEvents.length}
          </p>
        </div>
      </div>

      {/* Quick Action */}
      <div style={{ marginBottom: 40 }}>
        <button
          onClick={() => navigate("/events")}
          style={{
            padding: "15px 30px",
            background: "#D5F087",
            color: "#000",
            border: "none",
            borderRadius: 8,
            cursor: "pointer",
            fontSize: 16,
            fontWeight: "bold",
            boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
          }}
        >
          View Events Calendar
        </button>
      </div>

      {/* Two Column Layout */}
      <div style={{
        display: "grid",
        gridTemplateColumns: "1fr 1fr",
        gap: 30
      }}>

        {/* Upcoming Registered Events */}
        <div>
          <h2 style={{ marginBottom: 20, fontSize: 24 }}>
            My Upcoming Events
          </h2>

          {upcomingRegistered.length === 0 ? (
            <div style={{
              textAlign: "center",
              padding: 40,
              background: "#fff",
              borderRadius: 8,
              border: "2px dashed #ddd"
            }}>
              <p style={{ color: "#999", marginBottom: 15 }}>No upcoming events registered</p>
              <button
                onClick={() => navigate("/events")}
                style={{
                  padding: "10px 20px",
                  background: "#667eea",
                  color: "#fff",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer"
                }}
              >
                Browse Events
              </button>
            </div>
          ) : (
            <div style={{ display: "grid", gap: 15 }}>
              {upcomingRegistered.map((event) => (
                <div
                  key={event.eventId}
                  onClick={() => navigate(`/events/${event.eventId}`)}
                  style={{
                    background: "#fff",
                    padding: 20,
                    borderRadius: 8,
                    border: "1px solid #ddd",
                    cursor: "pointer",
                    transition: "0.3s",
                    boxShadow: "0 2px 4px rgba(0,0,0,0.05)"
                  }}
                >
                  <h3>{event.eventName}</h3>
                  <p style={{ color: "#666", fontSize: 14 }}>
                    {new Date(event.eventDate).toLocaleString()}
                  </p>
                  <p style={{ color: "#666", fontSize: 14 }}>
                    At {event.location}
                  </p>
                  <div style={{
                    marginTop: 10,
                    padding: "4px 10px",
                    background: "#d4edda",
                    color: "#155724",
                    borderRadius: 4,
                    fontSize: 12,
                    fontWeight: "bold",
                    display: "inline-block"
                  }}>
                    ✓ Registered
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Past Events */}
        <div>
          <h2 style={{ marginBottom: 20, fontSize: 24 }}>
            Past Registered Events
          </h2>

          {pastRegistered.length === 0 ? (
            <div style={{
              textAlign: "center",
              padding: 40,
              background: "#f8f9fa",
              borderRadius: 8,
              border: "1px solid #e0e0e0"
            }}>
              <p style={{ color: "#999" }}>No past events yet</p>
            </div>
          ) : (
            <div style={{ display: "grid", gap: 15 }}>
              {pastRegistered.map((event) => (
                <div
                  key={event.eventId}
                  style={{
                    background: "#f8f9fa",
                    padding: 20,
                    borderRadius: 8,
                    border: "1px solid #e0e0e0",
                    opacity: 0.8
                  }}
                >
                  <h3 style={{ color: "#666" }}>{event.eventName}</h3>
                  <p style={{ color: "#888", fontSize: 14 }}>
                    {new Date(event.eventDate).toLocaleDateString()}
                  </p>
                  <p style={{ color: "#888", fontSize: 14 }}>
                    At {event.location}
                  </p>
                  <div style={{
                    marginTop: 10,
                    padding: "4px 10px",
                    background: "#e2e3e5",
                    color: "#6c757d",
                    borderRadius: 4,
                    fontSize: 12,
                    fontWeight: "bold",
                    display: "inline-block"
                  }}>
                    Completed
                  </div>
                </div>
              ))}
            </div>
          )}

        </div>
      </div>

    </div>
  );
}

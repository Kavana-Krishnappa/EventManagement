import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEvents } from "../../features/events/eventsSlice";
//import EventCard from "../../components/events/EventCard";
import { useNavigate } from "react-router-dom";

export default function UserEvents() {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { list: events, loading } = useSelector((s) => s.events);

  const [currentMonth, setCurrentMonth] = useState(new Date());
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    dispatch(fetchEvents());
  }, [dispatch]);

  if (loading) return <p>Loading events...</p>;

// Filter upcoming events with search
const today = new Date();
const upcomingEvents = events.filter(event => {
  const isFuture = new Date(event.eventDate) >= today;
  const matchesSearch = searchTerm 
    ? event.eventName.toLowerCase().includes(searchTerm.toLowerCase())
    : true;
  return isFuture && matchesSearch;
});

//for calender calculate 
  const firstDayOfMonth = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), 1);
  const totalDays = new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 0).getDate();

  //make an array of all days of the months
 
  const monthDays = [];
  for (let i = 1; i <= totalDays; i++) {
    monthDays.push(new Date(currentMonth.getFullYear(), currentMonth.getMonth(), i));
  }

  // Group events by date
  const eventsByDate = {};
  monthDays.forEach(day => {
    const key = day.toISOString().split("T")[0];
    eventsByDate[key] = upcomingEvents.filter(
      event => event.eventDate.split("T")[0] === key
    );
  });

  // Navigation
  const prevMonth = () => {
    const prev = new Date(currentMonth);
    prev.setMonth(prev.getMonth() - 1);
    setCurrentMonth(prev);
  };

  const nextMonth = () => {
    const next = new Date(currentMonth);
    next.setMonth(next.getMonth() + 1);
    setCurrentMonth(next);
  };

  const weekdays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

  return (
    <div style={{ padding: 20 }}>
      <h2>Monthly Events Calendar</h2>

      
    
    <div style={{ marginBottom: 15 }}>
      <input
        type="text"
        placeholder="Search"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        style={{
          width: "100%",
          padding: 12,
          fontSize: 16,
          borderRadius: 4,
          border: "1px solid #ddd"
        }}
      />
    </div>

    <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 10 }}></div>


      <div
  style={{
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 16,
    padding: "8px 12px",
    background: "#1A8085",
    borderRadius: 8,
    boxShadow: "0 1px 3px rgba(0,0,0,0.08)"
  }}
>
  <button
    onClick={prevMonth}
    style={{
      padding: "6px 12px",
      borderRadius: 6,
      border: "none",
      background: "#e2e8f0",
      fontFamily: "'Inter', sans-serif",
      cursor: "pointer",
      fontSize: 14,
      fontWeight: 500
    }}
  >
    ← Prev
  </button>

  <h3
    style={{
      margin: 0,
      fontFamily: "'Inter', 'Segoe UI', sans-serif",
      fontSize: 18,
      fontWeight: 600,
      color: "#fff"
    }}
  >
    {currentMonth.toLocaleString("default", {
      month: "long",
      year: "numeric"
    })}
  </h3>

  <button
    onClick={nextMonth}
    style={{
      padding: "6px 12px",
      borderRadius: 6,
      border: "none",
      background: "#e2e8f0",
      fontFamily: "'Inter', sans-serif",
      cursor: "pointer",
      fontSize: 14,
      fontWeight: 500
    }}
  >
    Next →
  </button>
</div>


      {/* Weekday headers */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(7, 1fr)", fontWeight: "bold" }}>
        {weekdays.map((d) => (
          <div key={d} style={{ padding: 5, textAlign: "center" }}>{d}</div>
        ))}
      </div>

      {/* Calendar days */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(7, 1fr)" }}>
        {/* blank cells for first day offset */}
        {[...Array(firstDayOfMonth.getDay())].map((_, i) => <div key={`blank-${i}`} />)}

        {monthDays.map((day) => {
          const key = day.toISOString().split("T")[0];
          return (
            <div
              key={key}
              style={{
                border: "1px solid #ddd",
                minHeight: 60,
                padding: 5,
                margin: 1,
                borderRadius: 4,
                backgroundColor: day.toDateString() === today.toDateString() ? "#f0f8ff" : "#fff"
              }}
            >
              <div style={{ fontSize: 12, fontWeight: "bold" }}>{day.getDate()}</div>

              {eventsByDate[key]?.map((event) => (
                <div
                  key={event.eventId}
                  onClick={() => navigate(`/events/${event.eventId}`)}
                  style={{
                    fontSize: 12,
                    backgroundColor: "#1A8085",
                    color: "#fff",
                    borderRadius: 4,
                    padding: "2px 4px",
                    marginTop: 2,
                    cursor: "pointer",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap"
                  }}
                  title={event.eventName}
                >
                  {event.eventName}
                </div>
              ))}
            </div>
          );
        })}
      </div>
    
{searchTerm && upcomingEvents.length === 0 && (
  <div style={{
    textAlign: "center",
    padding: 40,
    background: "#f8f9fa",
    borderRadius: 8,
    marginTop: 20
  }}>
    <p style={{ fontSize: 18, color: "#666" }}>
      No events found matching "{searchTerm}"
    </p>
    <button
      onClick={() => setSearchTerm("")}
      style={{
        marginTop: 10,
        padding: "8px 16px",
        background: "#667eea",
        color: "#fff",
        border: "none",
        borderRadius: 4,
        cursor: "pointer"
      }}
    >
      Clear Search
    </button>
  </div>
)}
    </div>
  );
}

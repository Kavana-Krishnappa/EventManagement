import React from "react";

export default function EventCard({ event, onEdit, onDelete, showActions = false }) {
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit"
    });
  };

  //check past event
  const isPastEvent = new Date(event.eventDate) < new Date();

  return (
    <div
      style={{
        border: "1px solid #ddd",
        padding: 20,
        borderRadius: 8,
        marginBottom: 12,
        background: isPastEvent ? "#f8f9fa" : "#fff",
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        position: "relative"
      }}
    >
      {isPastEvent && (
        <div style={{
          position: "absolute",
          top: 10,
          right: 10,
          background: "#e6cd84ff",
          color: "#000",
          padding: "4px 8px",
          borderRadius: 4,
          fontSize: 12,
          fontWeight: "bold"
        }}>
          PAST EVENT
        </div>
      )}

      <h3 style={{ marginTop: 0, marginBottom: 10, color: "#333" }}>
        {event.eventName}
      </h3>
      
      <p style={{ color: "#666", marginBottom: 10 }}>
        {event.description}
      </p>
      
      <div style={{ display: "grid", gap: 8, fontSize: 14 }}>
        <p style={{ margin: 0 }}>
          <strong>Date:</strong> {formatDate(event.eventDate)}
        </p>
        <p style={{ margin: 0 }}>
          <strong>Location:</strong> {event.location}
        </p>
        <p style={{ margin: 0 }}>
          <strong>Capacity:</strong> {event.maxCapacity} people
        </p>
      </div>

     
      {showActions && (onEdit || onDelete) && (
        <div style={{ marginTop: 15, display: "flex", gap: 10 }}>
          {onEdit && (
            <button
              onClick={() => onEdit(event)}
              style={{
                padding: "8px 16px",
                background: "#6e9ed1ff",
                color: "#fff",
                border: "none",
                borderRadius: 4,
                cursor: "pointer"
              }}
            >
              Edit
            </button>
          )}
          {onDelete && (
            <button
              onClick={() => onDelete(event.eventId)}
              style={{
                padding: "8px 16px",
                background: "#d85461ff",
                color: "#fff",
                border: "none",
                borderRadius: 4,
                cursor: "pointer"
              }}
            >
              Delete
            </button>
          )}
        </div>
      )}
    </div>
  );
}
import React from "react";
import EventCard from "./EventCard";

export default function EventList({ events, onEdit, onDelete }) {
  return (
    <div>
      {events.map((event) => (
        <EventCard 
          key={event.id} 
          event={event}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </div>
  );
}

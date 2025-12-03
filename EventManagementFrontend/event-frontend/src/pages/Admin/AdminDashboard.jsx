import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchEvents,
  createEvent,
  updateEvent,
  deleteEvent,
} from "../../features/events/eventsSlice";

import EventForm from "../../components/events/EventForm";
import EventCard from "../../components/events/EventCard";
import EventRegistrations from "../../components/events/EventRegistrations";


export default function AdminDashboard() {
  const dispatch = useDispatch();
  //auth
  const auth = useSelector((s) => s.auth);
  //load events
  const { list: events, loading, error } = useSelector((s) => s.events);

  const [showCreateForm, setShowCreateForm] = useState(false);//create
  const [editingEvent, setEditingEvent] = useState(null);//edit
  const [viewingRegistrations, setViewingRegistrations] = useState(null);
  const [filter, setFilter] = useState("all"); // all, upcoming, past
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    dispatch(fetchEvents());
  }, [dispatch]);

  const handleCreateEvent = async (formData) => {
    try {
      console.log("Creating event with data:", formData);

      const eventData = {
        eventName: formData.eventName,
        description: formData.description,
        location: formData.location,
        maxCapacity: formData.maxCapacity,
        eventDate: formData.eventDate, 

        createdByAdminId: auth.profile?.adminId || 1,
      };

      console.log(" Sending to backend:", eventData);

      await dispatch(createEvent(eventData)).unwrap();

      setShowCreateForm(false);
      alert("Event created successfully!");
    } catch (err) {
      console.error(" Create event error:", err);
      alert("Failed to create event: " + (err.message || JSON.stringify(err)));
    }
  };


  //patch request so op:replace
  const handleUpdateEvent = async (formData) => {
    try {
      console.log("Updating event:", editingEvent.eventId, formData);

      const patchDocument = [
        { op: "replace", path: "/eventName", value: formData.eventName },
        { op: "replace", path: "/description", value: formData.description },
        { op: "replace", path: "/location", value: formData.location },
        { op: "replace", path: "/maxCapacity", value: parseInt(formData.maxCapacity) },
        { op: "replace", path: "/eventDate", value: formData.eventDate },

      ];

      await dispatch(
        updateEvent({
          id: editingEvent.eventId,
          data: patchDocument,
        })
      ).unwrap();

      setEditingEvent(null);
      alert("Event updated successfully!");
    } catch (err) {
      console.error(" Update event error:", err);
      alert("Failed to update event: " + (err.message || JSON.stringify(err)));
    }
  };

  const handleDeleteEvent = async (eventId) => {
    if (!window.confirm("Are you sure you want to delete this event?")) return;

    try {
      await dispatch(deleteEvent(eventId)).unwrap();
      alert("Event deleted successfully!");
    } catch (err) {
      console.error(" Delete event error:", err);
      alert("Failed to delete event: " + (err.message || JSON.stringify(err)));
    }
  };

  const handleEdit = (event) => {
    setEditingEvent(event);
    setShowCreateForm(false);
  };

  const handleCancelEdit = () => {
    setEditingEvent(null);
  };

  // Filter events by search and date filter
const filteredEvents = events.filter(event => {
  const eventDate = new Date(event.eventDate);
  const now = new Date();

  // Date filter
  let passesDateFilter = true;
  if (filter === "upcoming") passesDateFilter = eventDate >= now;
  if (filter === "past") passesDateFilter = eventDate < now;

  // Search filter
  const searchLower = searchTerm.toLowerCase();
  const passesSearchFilter = 
    event.eventName?.toLowerCase().includes(searchLower);
    //event.location?.toLowerCase().includes(searchLower) ||
    //event.description?.toLowerCase().includes(searchLower);

  return passesDateFilter && passesSearchFilter;
});
  const sortedEvents = [...filteredEvents].sort(
    (a, b) => new Date(a.eventDate) - new Date(b.eventDate)
  );

  return (
    <div style={{ padding: 20, maxWidth: 1200, margin: "0 auto" }}>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 30,
        }}
      >
        <h1 style={{ margin: 0 }}> Admin Dashboard</h1>

        <button
          onClick={() => {
            setShowCreateForm(!showCreateForm);
            setEditingEvent(null);
          }}
          style={{
            padding: "12px 24px",
            background: showCreateForm ? "#6c757d" : "#7db8abff",
            color: "#fff",
            border: "none",
            borderRadius: 4,
            cursor: "pointer",
            fontSize: 16,
            fontWeight: "bold",
          }}
        >
          {showCreateForm ? "Cancel" : "Create New Event"}
        </button>
      </div>

       <div style={{ marginBottom: 20 }}>
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

      {/* Create Form */}
      {showCreateForm && (
        <div
          style={{
            background: "#f8f9fa",
            padding: 20,
            borderRadius: 8,
            marginBottom: 30,
            border: "2px solid #28a745",
          }}
        >
          <h2>Create New Event</h2>
          <EventForm onSubmit={handleCreateEvent} initialData={null} />
        </div>
      )}

      {/* Edit Form */}
      {editingEvent && (
        <div
          style={{
            background: "#fff3cd",
            padding: 20,
            borderRadius: 8,
            marginBottom: 30,
            border: "2px solid #ffc107",
          }}
        >
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <h2>Edit Event</h2>
            <button
              onClick={handleCancelEdit}
              style={{
                padding: "8px 16px",
                background: "#6c757d",
                color: "#fff",
                border: "none",
                borderRadius: 4,
                cursor: "pointer",
              }}
            >
              Cancel Edit
            </button>
          </div>

          <EventForm onSubmit={handleUpdateEvent} initialData={editingEvent} />
        </div>
      )}

      {/* Filter Buttons */}
      <div
        style={{
          display: "flex",
          gap: 10,
          marginBottom: 20,
          padding: "10px 0",
          borderBottom: "2px solid #ddd",
        }}
      >
        <button
          onClick={() => setFilter("all")}
          style={{
            padding: "8px 16px",
            background: filter === "all" ? "#7db8abff" : "#fff",
            color: filter === "all" ? "#fff" : "#333",
            border: "1px solid #007bff",
            borderRadius: 4,
          }}
        >
          All Events ({events.length})
        </button>

        <button
          onClick={() => setFilter("upcoming")}
          style={{
            padding: "8px 16px",
            background: filter === "upcoming" ? "#44cc64ff" : "#fff",
            color: filter === "upcoming" ? "#fff" : "#333",
            border: "1px solid #28a745",
            borderRadius: 4,
          }}
        >
          Upcoming (
          {events.filter((e) => new Date(e.eventDate) >= new Date()).length})
        </button>

        <button
          onClick={() => setFilter("past")}
          style={{
            padding: "8px 16px",
            background: filter === "past" ? "#6c757d" : "#fff",
            color: filter === "past" ? "#fff" : "#333",
            border: "1px solid #6c757d",
            borderRadius: 4,
          }}
        >
          Past (
          {events.filter((e) => new Date(e.eventDate) < new Date()).length})
        </button>
      </div>

      {/* Loading / Error */}
      {loading && <p>Loading events...</p>}
      {error && <p style={{ color: "red" }}>Error: {error}</p>}

      {/* Events List */}
      <div>
        <h2>All Events ({sortedEvents.length}{searchTerm && ` of ${events.length}`})</h2>

        {sortedEvents.length === 0 ? (
          <div
            style={{
              textAlign: "center",
              padding: 40,
              background: "#f8f9fa",
              borderRadius: 8,
            }}
          >
            <p style={{ fontSize: 18, color: "#666" }}>
              {filter === "all"
                ? "No events created yet. Create your first event!"
                : `No ${filter} events found.`}
            </p>
          </div>
        ) : (
          sortedEvents.map((event) => (
            <div key={event.eventId}>
              <EventCard
                event={event}
                onEdit={handleEdit}
                onDelete={handleDeleteEvent}
               
                showActions={true}
                
              />

              {/* View Registrations Button */}
              <div style={{ marginTop: -10, marginBottom: 20, marginLeft: 20 }}>
                <button
                  onClick={() => setViewingRegistrations(event)}
                  style={{
                    padding: "8px 16px",
                    background: "#93aedfff",
                    color: "#fff",
                    border: "none",
                    borderRadius: 4,
                    cursor: "pointer",
                    fontSize: 14,
                  }}
                >
                  View Registrations
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Registrations Modal */}
      {viewingRegistrations && (
        <EventRegistrations
          eventId={viewingRegistrations.eventId}
          eventName={viewingRegistrations.eventName}
          onClose={() => setViewingRegistrations(null)}
        />
      )}
    </div>
  );
}

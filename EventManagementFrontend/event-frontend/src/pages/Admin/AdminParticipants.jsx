import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchParticipants, fetchParticipantEvents, clearParticipantEvents } from "../../features/participants/participantsSlice";

export default function AdminParticipants() {
  const dispatch = useDispatch();
  const { list: participants, selectedParticipantEvents, loading, error } = useSelector((s) => s.participants);
  const [selectedParticipant, setSelectedParticipant] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    dispatch(fetchParticipants());
  }, [dispatch]);

  const handleViewEvents = async (participant) => {
    setSelectedParticipant(participant);
    await dispatch(fetchParticipantEvents(participant.participantId));
  };

  const handleCloseEvents = () => {
    setSelectedParticipant(null);
    dispatch(clearParticipantEvents());
  };

  
  const filteredParticipants = participants.filter(p => 
    p.fullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.email?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: 20, maxWidth: 1200, margin: "0 auto" }}>
      <h1>Participant Management</h1>

      {/* Search Bar */}
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

      {loading && <p>Loading participants...</p>}
      {error && <p style={{ color: "red" }}>Error: {error}</p>}

      {/* Participants Table */}
      <div style={{ 
        background: "#fff", 
        borderRadius: 8, 
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        overflow: "hidden"
      }}>
        <table style={{ 
          width: "100%", 
          borderCollapse: "collapse"
        }}>
          <thead>
            <tr style={{ background: "#f8f9fa", borderBottom: "2px solid #ddd" }}>
              <th style={{ padding: 15, textAlign: "left" }}>ID</th>
              <th style={{ padding: 15, textAlign: "left" }}>Full Name</th>
              <th style={{ padding: 15, textAlign: "left" }}>Email</th>
              <th style={{ padding: 15, textAlign: "left" }}>Phone</th>
              <th style={{ padding: 15, textAlign: "center" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredParticipants.length === 0 ? (
              <tr>
                <td colSpan={5} style={{ padding: 40, textAlign: "center", color: "#666" }}>
                  {searchTerm ? "No participants found matching your search" : "No participants registered yet"}
                </td>
              </tr>
            ) : (
              filteredParticipants.map((participant) => (
                <tr key={participant.participantId} style={{ borderBottom: "1px solid #eee" }}>
                  <td style={{ padding: 15 }}>{participant.participantId}</td>
                  <td style={{ padding: 15, fontWeight: "bold" }}>{participant.fullName}</td>
                  <td style={{ padding: 15 }}>{participant.email}</td>
                  <td style={{ padding: 15 }}>{participant.phoneNumber || "N/A"}</td>
                  <td style={{ padding: 15, textAlign: "center" }}>
                    <button
                      onClick={() => handleViewEvents(participant)}
                      style={{
                        padding: "8px 16px",
                        background: "#88b2dfff",
                        color: "#fff",
                        border: "none",
                        borderRadius: 4,
                        cursor: "pointer"
                      }}
                    >
                      View Events
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Total Count */}
      <div style={{ marginTop: 20, fontSize: 14, color: "#666" }}>
        Total Participants: <strong>{filteredParticipants.length}</strong>
        {searchTerm && ` (filtered from ${participants.length})`}
      </div>

      {/* Selected Participant Events Modal */}
      {selectedParticipant && (
        <div style={{
          position: "fixed",
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background: "rgba(0,0,0,0.5)",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          zIndex: 1000
        }}>
          <div style={{
            background: "#fff",
            padding: 30,
            borderRadius: 8,
            maxWidth: 600,
            width: "90%",
            maxHeight: "80vh",
            overflow: "auto"
          }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
              <h2 style={{ margin: 0 }}>
                 Events for {selectedParticipant.fullName}
              </h2>
              <button
                onClick={handleCloseEvents}
                style={{
                  padding: "8px 16px",
                  background: "#6c757d",
                  color: "#fff",
                  border: "none",
                  borderRadius: 4,
                  cursor: "pointer"
                }}
              >
                Close
              </button>
            </div>

            {selectedParticipantEvents.length === 0 ? (
              <p style={{ textAlign: "center", color: "#666", padding: 20 }}>
                No upcoming events registered
              </p>
            ) : (
              <div style={{ display: "grid", gap: 15 }}>
                {selectedParticipantEvents.map((event) => (
                  <div
                    key={event.eventId}
                    style={{
                      border: "1px solid #ddd",
                      padding: 15,
                      borderRadius: 8,
                      background: "#f8f9fa"
                    }}
                  >
                    <h3 style={{ margin: "0 0 10px 0" }}>{event.eventName}</h3>
                    <p style={{ margin: "5px 0", fontSize: 14 }}>
                      <strong>Date:</strong> {new Date(event.eventDate).toLocaleDateString()}
                    </p>
                    <p style={{ margin: "5px 0", fontSize: 14 }}>
                      <strong>Location:</strong> {event.location}
                    </p>
                    <p style={{ margin: "5px 0", fontSize: 14, color: "#666" }}>
                      {event.description}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
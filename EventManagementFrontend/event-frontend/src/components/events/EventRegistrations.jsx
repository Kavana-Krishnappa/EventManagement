import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchEventRegistrations, deleteRegistration } from "../../features/registrations/registrationsSlice";

export default function EventRegistrations({ eventId, eventName, onClose }) {
  const dispatch = useDispatch(); //reduc actions
  const { list: registrations, loading } = useSelector((s) => s.registrations);

  //fetch registration on load
  useEffect(() => {
    if (eventId) {
      dispatch(fetchEventRegistrations(eventId));
    }
  }, [dispatch, eventId]);

  //delete
  const handleDeleteRegistration = async (registrationId, participantName) => {
    if (window.confirm(`Remove ${participantName} from this event?`)) {
      try {
        await dispatch(deleteRegistration(registrationId)).unwrap();
        alert(" Registration removed successfully");
      } catch (err) {
        alert("Failed to remove registration: " + err);
      }
    }
  };

  return (
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
        maxWidth: 800,
        width: "90%",
        maxHeight: "80vh",
        overflow: "auto"
      }}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 20 }}>
          <h2 style={{ margin: 0 }}>
            Registrations for "{eventName}"
          </h2>
          <button
            onClick={onClose}
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

        {loading && <p>Loading registrations..</p>}
         
  

        {!loading && registrations.length === 0 && (
          <p style={{ textAlign: "center", color: "#666", padding: 20 }}>
            No registrations yet for this event
          </p>
        )}

        {!loading && registrations.length > 0 && (
          <>
            <p style={{ marginBottom: 15, color: "#666" }}>
              Total Registrations: <strong>{registrations.length}</strong>
            </p>
            
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead>
                <tr style={{ background: "#f8f9fa", borderBottom: "2px solid #ddd" }}>
                  <th style={{ padding: 12, textAlign: "left" }}>ID</th>
                  <th style={{ padding: 12, textAlign: "left" }}>Participant ID</th>
                  <th style={{ padding: 12, textAlign: "left" }}>Status</th>
                  <th style={{ padding: 12, textAlign: "left" }}>Registered At</th>
                  <th style={{ padding: 12, textAlign: "center" }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {registrations.map((reg) => (
                  <tr key={reg.registrationId} style={{ borderBottom: "1px solid #eee" }}>
                    <td style={{ padding: 12 }}>{reg.registrationId}</td>
                    <td style={{ padding: 12 }}>{reg.participantId}</td>
                    <td style={{ padding: 12 }}>
                      <span style={{
                        padding: "4px 8px",
                        background: reg.status === "Confirmed" ? "#d4edda" : "#fff3cd",
                        color: reg.status === "Confirmed" ? "#155724" : "#856404",
                        borderRadius: 4,
                        fontSize: 12,
                        fontWeight: "bold"
                      }}>
                        {reg.status}
                      </span>
                    </td>
                    <td style={{ padding: 12 }}>
                      {new Date(reg.registeredAt).toLocaleString()}
                    </td>
                    <td style={{ padding: 12, textAlign: "center" }}>
                      <button
                        onClick={() => handleDeleteRegistration(reg.registrationId, `Participant ${reg.participantId}`)}
                        style={{
                          padding: "6px 12px",
                          background: "#ac3e49ff",
                          color: "#fff",
                          border: "none",
                          borderRadius: 4,
                          cursor: "pointer",
                          fontSize: 12
                        }}
                      >
                        Remove
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </>
        )}
      </div>
    </div>
  );
}
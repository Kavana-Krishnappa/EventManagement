import React, { useState, useEffect } from "react";


//input values
export default function EventForm({ initialData, onSubmit }) {
  const [form, setForm] = useState({
    eventName: "",
    description: "",
    location: "",
    maxCapacity: "",
    eventDate: ""
  });


  //can't pick previous date
  const getCurrentDateTime = () => {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  useEffect(() => {
    if (initialData) {
      let dateValue = "";
      if (initialData.eventDate) {
        const date = new Date(initialData.eventDate);
        dateValue = date.toISOString().slice(0, 16);
      }

      setForm({
        eventName: initialData.eventName || "",
        description: initialData.description || "",
        location: initialData.location || "",
        maxCapacity: initialData.maxCapacity || "",
        eventDate: dateValue
      });
    }
  }, [initialData]);

  
//update form state
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    

    //Validation checks
  
    if (!form.eventName || !form.description || !form.location || !form.maxCapacity || !form.eventDate) {
      alert("Please fill in all fields");
      return;
    }

    if (parseInt(form.maxCapacity) <= 0) {
      alert("Max capacity must be greater than 0");
      return;
    }

    


    if (!initialData) {
      const selectedDate = new Date(form.eventDate);
      const now = new Date();
      
      if (selectedDate < now) {
        alert("Event date cannot be in the past. Please select a future date and time.");
        return;
      }
    }

    onSubmit(form);
  };

  return (
    <form 
      onSubmit={handleSubmit} 
      style={{ 
        display: "grid", 
        gap: 15,
        maxWidth: 600 
      }}
    >
      <div>
        <label style={{ display: "block", marginBottom: 5, fontWeight: "bold" }}>
          Event Name *
        </label>
        <input 
          name="eventName" 
          value={form.eventName} 
          onChange={handleChange} 
          required 
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #ddd",
            fontSize: 14
          }}
        />
      </div>

      <div>
        <label style={{ display: "block", marginBottom: 5, fontWeight: "bold" }}>
          Description *
        </label>
        <textarea 
          name="description" 
          value={form.description} 
          onChange={handleChange} 
          required 
          rows={4}
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #ddd",
            fontSize: 14,
            fontFamily: "inherit"
          }}
        />
      </div>

      <div>
        <label style={{ display: "block", marginBottom: 5, fontWeight: "bold" }}>
          Location *
        </label>
        <input 
          name="location" 
          value={form.location} 
          onChange={handleChange} 
          required 
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #ddd",
            fontSize: 14
          }}
        />
      </div>

      <div>
        <label style={{ display: "block", marginBottom: 5, fontWeight: "bold" }}>
          Max Capacity *
        </label>
        <input 
          name="maxCapacity" 
          type="number" 
          min="1"
          value={form.maxCapacity} 
          onChange={handleChange} 
          required 
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #ddd",
            fontSize: 14
          }}
        />
      </div>

      <div>
        <label style={{ display: "block", marginBottom: 5, fontWeight: "bold" }}>
          Event Date & Time *
        </label>
        <input 
          name="eventDate" 
          type="datetime-local" 
          min={!initialData ? getCurrentDateTime() : undefined} // Only restrict for new events
          value={form.eventDate} 
          onChange={handleChange} 
          required 
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #ddd",
            fontSize: 14
          }}
        />
        {!initialData && (
          <small style={{ color: "#666", fontSize: 12 }}>
            * Must be a future date and time
          </small>
        )}
      </div>

      <button 
        type="submit"
        style={{
          padding: "12px 24px",
          background: "#007bff",
          color: "#fff",
          border: "none",
          borderRadius: 4,
          cursor: "pointer",
          fontSize: 16,
          fontWeight: "bold"
        }}
      >
        {initialData ? "Update Event" : "Create Event"}
      </button>
    </form>
  );
}
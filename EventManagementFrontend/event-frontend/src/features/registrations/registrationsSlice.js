import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import registrationApi from "../../api/registrationApi";

export const registerForEvent = createAsyncThunk(
  "registrations/register",
  async ({ eventId, participantId }, { rejectWithValue }) => {
    try {
      console.log("Registering:", { eventId, participantId });
      const res = await registrationApi.register(eventId, participantId);
      console.log(" Registration success:", res.data);
      return res.data;
    } catch (err) {
      console.error("Registration error:", err.response || err);
      
      // Extract error message from backend response
      const message = 
        err.response?.data?.message || 
        err.response?.data || 
        err.message ||
        "Registration failed";
      
      return rejectWithValue(message);
    }
  }
);

// Fetch registrations for an event
export const fetchEventRegistrations = createAsyncThunk(
  "registrations/fetchEventRegistrations",
  async (eventId, { rejectWithValue }) => {
    try {
      const res = await registrationApi.getEventRegistrations(eventId);
      return res.data;
    } catch (err) {
      return rejectWithValue(err.response?.data || "Failed to fetch registrations");
    }
  }
);

// Delete registration 
export const deleteRegistration = createAsyncThunk(
  "registrations/delete",
  async (registrationId, { rejectWithValue }) => {
    try {
      await registrationApi.deleteRegistration(registrationId);
      return registrationId;
    } catch (err) {
      return rejectWithValue(err.response?.data || "Failed to delete registration");
    }
  }
);

const registrationsSlice = createSlice({
  name: "registrations",
  initialState: {
    list: [],
    loading: false,
    error: null
  },
  reducers: {
    clearRegistrations: (state) => {
      state.list = [];
      state.error = null;
    }
  },
  extraReducers: (builder) => {
    builder
      // Register for event
      .addCase(registerForEvent.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(registerForEvent.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(registerForEvent.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Fetch registrations
      .addCase(fetchEventRegistrations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEventRegistrations.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload;
      })
      .addCase(fetchEventRegistrations.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Delete registration
      .addCase(deleteRegistration.fulfilled, (state, action) => {
        state.list = state.list.filter(r => r.registrationId !== action.payload);
      });
  }
});

export const { clearRegistrations } = registrationsSlice.actions;
export default registrationsSlice.reducer;
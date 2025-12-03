import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosClient from "../../api/axiosClient";

// Async Thunks
export const fetchEvents = createAsyncThunk(
  "events/fetchEvents",
  async (_, { rejectWithValue }) => {
    try {
      const res = await axiosClient.get("/Events/All");
      return res.data;
    } catch (err) {
      console.error("Fetch events error:", err.response || err);
      return rejectWithValue(err.response?.data || "Failed to fetch events");
    }
  }
);

//create

export const createEvent = createAsyncThunk(
  "events/createEvent",
  async (data, { rejectWithValue }) => {
    try {
      console.log("createEvent thunk - sending:", data);
      const res = await axiosClient.post("/Events/create", data);
      console.log("createEvent response:", res.data);
      return res.data;
    } catch (err) {
      console.error("createEvent error:", {
        status: err.response?.status,
        data: err.response?.data,
        message: err.message
      });
      return rejectWithValue(err.response?.data || "Failed to create event");
    }
  }
);

//update

export const updateEvent = createAsyncThunk(
  "events/updateEvent",
  async ({ id, data }, { rejectWithValue }) => {
    try {
      console.log("updateEvent thunk - sending:", { id, data });
      await axiosClient.patch(`/Events/${id}`, data);
      console.log("updateEvent success");
      return { id, data };
    } catch (err) {
      console.error("updateEvent error:", err.response || err);
      return rejectWithValue(err.response?.data || "Failed to update event");
    }
  }
);

//delete

export const deleteEvent = createAsyncThunk(
  "events/deleteEvent",
  async (id, { rejectWithValue }) => {
    try {
      console.log("deleteEvent thunk - deleting:", id);
      await axiosClient.delete(`/Events/${id}`);
      console.log("deleteEvent success");
      return id;
    } catch (err) {
      console.error("deleteEvent error:", err.response || err);
      return rejectWithValue(err.response?.data || "Failed to delete event");
    }
  }
);

// Slice
const eventsSlice = createSlice({
  name: "events",
  initialState: { list: [], loading: false, error: null },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchEvents.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEvents.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload;
      })
      .addCase(fetchEvents.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(createEvent.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createEvent.fulfilled, (state, action) => {
        state.loading = false;
        state.list.push(action.payload);
      })
      .addCase(createEvent.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(updateEvent.fulfilled, (state, action) => {
        const index = state.list.findIndex(e => e.eventId === action.payload.id);
        if (index !== -1) {
          state.list[index] = { ...state.list[index], ...action.payload.data };
        }
      })
      .addCase(deleteEvent.fulfilled, (state, action) => {
        state.list = state.list.filter(e => e.eventId !== action.payload);
      });
  }
});

export default eventsSlice.reducer;
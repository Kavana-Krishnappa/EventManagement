import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import participantApi from "../../api/participantApi";

export const fetchParticipants = createAsyncThunk(
  "participants/fetchAll",
  async (_, { rejectWithValue }) => {
    try {
      const res = await participantApi.getAll();
      return res.data;
    } catch (err) {
      console.error("Fetch participants error:", err.response || err);
      return rejectWithValue(err.response?.data || "Failed to fetch participants");
    }
  }
);

export const fetchParticipantEvents = createAsyncThunk(
  "participants/fetchEvents",
  async (participantId, { rejectWithValue }) => {
    try {
      const res = await participantApi.getUpcomingEvents(participantId);
      return res.data;
    } catch (err) {
      return rejectWithValue(err.response?.data || "Failed to fetch participant events");
    }
  }
);

const participantsSlice = createSlice({
  name: "participants",
  initialState: {
    list: [],
    selectedParticipantEvents: [],
    loading: false,
    error: null
  },
  reducers: {
    clearParticipantEvents: (state) => {
      state.selectedParticipantEvents = [];
    }
  },
  extraReducers: (builder) => {
    builder
      
      .addCase(fetchParticipants.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchParticipants.fulfilled, (state, action) => {
        state.loading = false;
        state.list = action.payload;
      })
      .addCase(fetchParticipants.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      
      .addCase(fetchParticipantEvents.fulfilled, (state, action) => {
        state.selectedParticipantEvents = action.payload;
      });
  }
});

export const { clearParticipantEvents } = participantsSlice.actions;
export default participantsSlice.reducer;
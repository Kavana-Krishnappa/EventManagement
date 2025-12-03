import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axiosClient from "../../api/axiosClient";


export const signupParticipant = createAsyncThunk(
  "signup/signupParticipant",
  async (data, { rejectWithValue }) => {
    try {
      console.log(" Signing up participant:", data);
      const res = await axiosClient.post("/Participants/SignUp", {
        fullName: data.fullName,
        email: data.email,
        phoneNumber: data.phoneNumber,
        password: data.password,
        role: "User" 

      });
      console.log(" Signup successful:", res.data);
      return res.data;
    } catch (err) {
      console.error("Signup error:", err.response || err);
      const message = err.response?.data?.message || err.response?.data || "Signup failed";
      return rejectWithValue(message);
    }
  }
);

const signupSlice = createSlice({
  name: "signup",
  initialState: {
    loading: false,
    error: null,
    success: false
  },
  reducers: {
    clearSignupState: (state) => {
      state.loading = false;
      state.error = null;
      state.success = false;
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(signupParticipant.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.success = false;
      })
      .addCase(signupParticipant.fulfilled, (state) => {
        state.loading = false;
        state.success = true;
        state.error = null;
      })
      .addCase(signupParticipant.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.success = false;
      });
  }
});

export const { clearSignupState } = signupSlice.actions;
export default signupSlice.reducer;
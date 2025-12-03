
import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import authApi from "../../api/authApi";



//send login request to backend
export const login = createAsyncThunk(
  "auth/login",
  async ({ email, password, role }, { rejectWithValue }) => {
    try {
      if (role === "Admin") {
        const res = await authApi.loginAdmin({ email, password });
      
        return { token: res.data.token, profile: res.data.admin, role: "Admin" };
      } else {
        const res = await authApi.loginParticipant({ email, password, role: "User" });
       
        return { token: res.data.token, profile: res.data.participant, role: "User" };
      }
    } catch (err) {
  const backend = err.response?.data;

  const message =
    backend?.message ||
    backend?.title ||
    (backend?.errors ? JSON.stringify(backend.errors) : null) ||
    err.message ||
    "Login failed";

  return rejectWithValue(message);
}

  }
);

//to keep user logged in even after refreshing the page

const initialAuth = (() => {
  try {
    const raw = localStorage.getItem("auth");
    if (raw) return JSON.parse(raw);
  } catch (e) {
    console.error("Failed to parse auth from localStorage", e);
  }
  return { token: null, profile: null, role: null };
})();


const authSlice = createSlice({
  name: "auth",
  initialState: {
    token: initialAuth.token,
    profile: initialAuth.profile,
    role: initialAuth.role,
    loading: false,
    error: null
  },
  //logout
  reducers: {
    logout: (state) => {
      state.token = null;
      state.profile = null;
      state.role = null;
      state.error = null;
      localStorage.removeItem("auth");
    }
  },
  //handles async login states
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        state.token = action.payload.token;
        state.profile = action.payload.profile;
        state.role = action.payload.role;
        state.error = null;
        
        localStorage.setItem("auth", JSON.stringify({
          token: state.token,
          profile: state.profile,
          role: state.role
        }));
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "Login failed";
      });
  }
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;

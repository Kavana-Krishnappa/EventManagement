import { configureStore } from "@reduxjs/toolkit";
import authReducer from "../features/auth/authSlice";
import eventReducer from "../features/events/eventsSlice";
import registrationsReducer from "../features/registrations/registrationsSlice";
import participantsReducer from "../features/participants/participantsSlice";
import signupReducer from "../features/auth/signupSlice"; 

export const store = configureStore({
  reducer: {
    auth: authReducer,
    events: eventReducer,
    registrations: registrationsReducer,
    participants: participantsReducer,
    signup: signupReducer 
  }
});
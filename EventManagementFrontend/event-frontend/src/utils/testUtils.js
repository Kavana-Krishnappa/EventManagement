
import React from 'react';
import { render } from '@testing-library/react';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import eventReducer from '../features/events/eventsSlice';
import registrationsReducer from '../features/registrations/registrationsSlice';
import participantsReducer from '../features/participants/participantsSlice';
import signupReducer from '../features/auth/signupSlice';

//Custom render function that includes Redux Provider and Router
 
export function renderWithProviders(
  ui,
  {
    preloadedState = {},
    store = configureStore({
      reducer: {
        auth: authReducer,
        events: eventReducer,
        registrations: registrationsReducer,
        participants: participantsReducer,
        signup: signupReducer,
      },
      preloadedState,
    }),
    ...renderOptions
  } = {}
) {
  function Wrapper({ children }) {
    return (
      <Provider store={store}>
        <BrowserRouter>{children}</BrowserRouter>
      </Provider>
    );
  }

  return { store, ...render(ui, { wrapper: Wrapper, ...renderOptions }) };
}


  //Create mock store with initial state
 
export function createMockStore(initialState = {}) {
  return configureStore({
    reducer: {
      auth: authReducer,
      events: eventReducer,
      registrations: registrationsReducer,
      participants: participantsReducer,
      signup: signupReducer,
    },
    preloadedState: initialState,
  });
}


  //Mock authenticated user state
 
export const mockAuthState = {
  token: 'mock-token-123',
  profile: {
    adminId: 1,
    email: 'admin@test.com',
    fullName: 'Test Admin',
  },
  role: 'Admin',
  loading: false,
  error: null,
};


 // Mock participant user state
 
export const mockParticipantAuthState = {
  token: 'mock-token-456',
  profile: {
    participantId: 1,
    email: 'user@test.com',
    fullName: 'Test User',
    phoneNumber: '1234567890',
  },
  role: 'User',
  loading: false,
  error: null,
};


 // Mock events data
 
export const mockEvents = [
  {
    eventId: 1,
    eventName: 'Tech Conference 2024',
    description: 'Annual tech conference',
    location: 'Convention Center',
    maxCapacity: 100,
    eventDate: '2024-12-31T10:00:00',
    createdByAdminId: 1,
  },
  {
    eventId: 2,
    eventName: 'Workshop on AI',
    description: 'Learn about AI basics',
    location: 'Tech Hub',
    maxCapacity: 50,
    eventDate: '2024-11-15T14:00:00',
    createdByAdminId: 1,
  },
];


 //Mock registrations data
 
export const mockRegistrations = [
  {
    registrationId: 1,
    eventId: 1,
    participantId: 1,
    status: 'Confirmed',
    registeredAt: '2024-10-01T10:00:00',
  },
  {
    registrationId: 2,
    eventId: 1,
    participantId: 2,
    status: 'Confirmed',
    registeredAt: '2024-10-02T11:00:00',
  },
];


  //Mock participants data
 
export const mockParticipants = [
  {
    participantId: 1,
    fullName: 'John Doe',
    email: 'john@test.com',
    phoneNumber: '1234567890',
  },
  {
    participantId: 2,
    fullName: 'Jane Smith',
    email: 'jane@test.com',
    phoneNumber: '0987654321',
  },
];


 // Wait for async updates
 
export const waitForAsync = () => new Promise(resolve => setTimeout(resolve, 0));

// Re-export everything from React Testing Library
export * from '@testing-library/react';
export { renderWithProviders as render };
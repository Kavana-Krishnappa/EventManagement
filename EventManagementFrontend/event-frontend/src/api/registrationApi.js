import axiosClient from "./axiosClient";

const registrationApi = {
 
  register: (eventId, participantId) =>
    axiosClient.post(`/Registrations/event/${eventId}/register`, {
      participantId,
      status: "Confirmed" 
    }),

  getEventRegistrations: (eventId) =>
    axiosClient.get(`/Registrations/event/${eventId}/registrations`), 

  
  deleteRegistration: (registrationId) =>
    axiosClient.delete(`/Registrations/registration/${registrationId}`) 
};

export default registrationApi;
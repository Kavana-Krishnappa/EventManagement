import axiosClient from "./axiosClient";

const participantApi = {

  getAll: () => axiosClient.get("/Participants/All"),

  getById: (id) => axiosClient.get(`/Participants/id?id=${id}`),

  getUpcomingEvents: (participantId) =>
    axiosClient.get(`/Participants/${participantId}/upcoming-events`),

  getPreviousEvents: (participantId) =>
    axiosClient.get(`/Participants/${participantId}/previous-events`),

  signUp: (data) => axiosClient.post("/Participants/SignUp", data)
};

export default participantApi;
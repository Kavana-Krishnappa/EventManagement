import axiosClient from "./axiosClient";

const authApi = {
    loginAdmin: ({ email, password }) =>
  axiosClient.post("/auth/login", {
    email,
    password   
  }),

  loginParticipant: (credentials) => axiosClient.post("/Participants/Login", credentials)
};

export default authApi;
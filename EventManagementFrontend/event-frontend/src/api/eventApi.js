import axiosClient from "./axiosClient";

const eventApi = {
 
  getAll: () => axiosClient.get("Events/All"),


  getById: (id) => axiosClient.get(`/Events/${id}`),

  
  create: (data) => axiosClient.post("/Events/create", data),

   getCapacity: (id) => axiosClient.get(`/Events/${id}/capacity`),


  update: (id, data) =>
    axiosClient.patch(`/Events/${id}`, data),  

  
  delete: (id) => axiosClient.delete(`/Events/${id}`)
};

export default eventApi;

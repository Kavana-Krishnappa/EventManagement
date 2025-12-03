import axios from "axios";

//url
const baseURL = "http://localhost:5036/api";

//axios instance
const axiosClient = axios.create({
    baseURL,
    headers: {
        "Content-Type": "application/json"
    },
    timeout: 15000,
});

//for automatically attachingtoken
axiosClient.interceptors.request.use((config) => {
    try{
        const raw = localStorage.getItem("auth");
        if(raw) {
            const auth = JSON.parse(raw);
            if(auth && auth.token){
                config.headers.Authorization = `Bearer ${auth.token}`;
            }
        }
    }catch(err){
        //ignore parse errors
    }
    return config;

}, (error) => Promise.reject(error));

export default axiosClient;
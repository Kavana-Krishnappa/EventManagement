import React from "react";
import ReactDOM from "react-dom/client";
//wrapping entire app, giving every component access to the redux store
import { Provider } from "react-redux";
import App from "./App";
import { store } from "./app/store";

const root = ReactDOM.createRoot(document.getElementById("root"));

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode> //helps catch common bugs
);

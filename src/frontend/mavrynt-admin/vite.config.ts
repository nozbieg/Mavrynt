import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const adminApiTarget =
    process.env.ADMINAPI_HTTPS ||
    process.env.ADMINAPI_HTTP ||
    "http://localhost:5001";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: adminApiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  preview: {
    proxy: {
      "/api": {
        target: adminApiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
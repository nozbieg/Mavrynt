import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const apiTarget =
    process.env.API_HTTPS ||
    process.env.API_HTTP ||
    "http://localhost:5000";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  preview: {
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
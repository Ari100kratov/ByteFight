import path from "path"
import tailwindcss from "@tailwindcss/vite"
import { defineConfig } from "vite"
import react from "@vitejs/plugin-react"

const localBackendTarget = "http://127.0.0.1:5000"

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    hmr: {
      host: "127.0.0.1",
      protocol: "ws",
    },
    proxy: {
      "/api": {
        target: localBackendTarget,
        changeOrigin: true,
        secure: false,
      },
      "/game-runtime-hub": {
        target: localBackendTarget,
        ws: true,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  build: {
    rolldownOptions: {
      output: {
        codeSplitting: {
          groups: [
            {
              test: /node_modules\/(?:monaco-editor|@monaco-editor\/react)\//,
              name: "monaco",
            },
            {
              test: /node_modules\/(?:@radix-ui|radix-ui|react-resizable-panels)\//,
              name: "ui-vendor",
            },
            {
              test: /node_modules\/(?:react|react-dom|react-router-dom|@tanstack\/react-query|@tanstack\/react-table|zustand|sonner|lucide-react|next-themes)\//,
              name: "react-vendor",
            },
          ],
        },
      },
    },
  },
})

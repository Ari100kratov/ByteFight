import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

import App from "./App.tsx"
import { Toaster } from "./components/ui/sonner.tsx"
import "./index.css"

const queryClient = new QueryClient()
const rootElement = document.getElementById("root")

if (!rootElement) {
  throw new Error("Root element #root not found")
}

createRoot(rootElement).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
      <Toaster position="top-right" expand={false} richColors />
    </QueryClientProvider>
  </StrictMode>,
)

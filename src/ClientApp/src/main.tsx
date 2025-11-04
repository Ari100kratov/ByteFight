import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import "./monaco-setup"

import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from './components/ui/sonner.tsx'

// Создаём глобальный QueryClient для react-query
const queryClient = new QueryClient()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
      <Toaster position='top-right' expand={false} richColors />
    </QueryClientProvider>
  </StrictMode>
)

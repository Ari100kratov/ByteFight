import { getAccessToken } from "@/shared/lib/auth"
import { Navigate } from "react-router-dom"

interface ProtectedRouteProps {
  children: React.ReactNode
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const token = getAccessToken()
  if (!token) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}

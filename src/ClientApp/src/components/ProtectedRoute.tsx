import { Navigate } from "react-router-dom"
import { jwtDecode } from "jwt-decode"

interface ProtectedRouteProps {
  children: React.ReactNode
}

interface JwtPayload {
  exp: number
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const token = localStorage.getItem("token")

  if (!token) {
    return <Navigate to="/login" replace />
  }

  try {
    const decoded = jwtDecode<JwtPayload>(token)
    const expired = decoded.exp * 1000 < Date.now()
    if (expired) {
      localStorage.removeItem("token")
      return <Navigate to="/login" replace />
    }
  } catch {
    // токен битый
    localStorage.removeItem("token")
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}

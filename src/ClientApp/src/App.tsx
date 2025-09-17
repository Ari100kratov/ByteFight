import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import LoginPage from "./pages/LoginPage"
import RegisterPage from "./pages/RegisterPage"
import ProtectedRoute from "./components/ProtectedRoute"
import MainLayout from "./layouts/MainLayout"
import InProgressPage from "./pages/InProgressPage"

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* публичные */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* защищённые */}
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <MainLayout />
            </ProtectedRoute>
          }
        >
          {/* вложенные страницы */}
          <Route index element={<Navigate to="/play" replace />} />
          <Route path="play" element={<InProgressPage title="Играть" />} />
          <Route path="characters" element={<InProgressPage title="Мои персонажи" />} />
          <Route path="docs" element={<InProgressPage title="Документация" />} />
          <Route path="settings" element={<InProgressPage title="Настройки" />} />
        </Route>

        {/* fallback */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

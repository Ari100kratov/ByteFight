import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import LoginPage from "./pages/LoginPage"
import RegisterPage from "./pages/RegisterPage"
import ProtectedRoute from "./components/ProtectedRoute"
import MainLayout from "./layouts/MainLayout"
import InProgressPage from "./pages/InProgressPage"
import CreateCharacterPage from "./features/character-create/CreateCharacterPage"
import CharactersPage from "./features/characters/CharactersPage"
import CharacterPage from "./features/character/CharacterPage"
import { BreadcrumbProvider } from "./layouts/BreadcrumbProvider"
import GameModesPage from "./features/game-modes/GameModesPage"
import GameMapsPage from "./features/game-maps/GameMapsPage"

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
              <BreadcrumbProvider>
                <MainLayout />
              </BreadcrumbProvider>
            </ProtectedRoute>
          }
        >
          {/* вложенные страницы */}
          <Route index element={<Navigate to="/play" replace />} />
          
          <Route path="play">
            <Route index element={<GameModesPage />} />
            <Route path=":id" element={<GameMapsPage />} />
          </Route>

          <Route path="characters">
            <Route index element={<CharactersPage />} />
            <Route path="create" element={<CreateCharacterPage />} />
            <Route path=":id" element={<CharacterPage />} />
          </Route>

          <Route path="docs" element={<InProgressPage title="Документация" />} />
          <Route path="settings" element={<InProgressPage title="Настройки" />} />
        </Route>

        {/* fallback */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import LoginPage from "./features/user-login-page/LoginPage"
import RegisterPage from "./features/user-register-page/RegisterPage"
import ProtectedRoute from "./components/ProtectedRoute"
import MainLayout from "./layouts/MainLayout"
import InProgressPage from "./pages/InProgressPage"
import CreateCharacterPage from "./features/character-create-page/CreateCharacterPage"
import CharactersPage from "./features/characters-page/CharactersPage"
import CharacterPage from "./features/character-page/CharacterPage"
import { BreadcrumbProvider } from "./layouts/BreadcrumbProvider"
import GameModesPage from "./features/game-modes-page/GameModesPage"
import GameArenasPage from "./features/game-arenas-page/GameArenasPage"
import GameArenaPage from "./features/game-arena-page/GameArenaPage"

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
            <Route path=":modeType" element={<GameArenasPage />} />
            <Route path=":modeType/:arenaId" element={<GameArenaPage />} key={window.location.pathname} />
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

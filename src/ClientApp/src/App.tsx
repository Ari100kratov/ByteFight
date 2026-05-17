import { lazy, Suspense } from "react"
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom"

import { Spinner } from "@/components/ui/spinner"
import ProtectedRoute from "./components/ProtectedRoute"
import MainLayout from "./layouts/MainLayout"
import { BreadcrumbProvider } from "./layouts/BreadcrumbProvider"

const LoginPage = lazy(() => import("./features/user-login-page/LoginPage"))
const RegisterPage = lazy(() => import("./features/user-register-page/RegisterPage"))
const InProgressPage = lazy(() => import("./pages/InProgressPage"))
const CreateCharacterPage = lazy(
  () => import("./features/character-create-page/CreateCharacterPage"),
)
const CharactersPage = lazy(() => import("./features/characters-page/CharactersPage"))
const CharacterPage = lazy(() => import("./features/character-page/CharacterPage"))
const GameModesPage = lazy(() => import("./features/game-modes-page/GameModesPage"))
const GameArenasPage = lazy(() => import("./features/game-arenas-page/GameArenasPage"))
const GameArenaPage = lazy(() => import("./features/game-arena-page/GameArenaPage"))
const NotFoundPage = lazy(() => import("./pages/NotFoundPage"))
const BattleHistoryPage = lazy(() => import("./features/battle-history-page/BattleHistoryPage"))
const ChronicleNominationsPage = lazy(
  () => import("./features/chronicle-nominations-page/ChronicleNominationsPage"),
)
const AccountPage = lazy(() => import("./features/account-page/AccountPage"))
const ScriptApiDocsPage = lazy(() => import("./features/script-api-docs-page/ScriptApiDocsPage"))
const QuickStartDocsPage = lazy(() => import("./features/docs-page/QuickStartDocsPage"))
const RecipesDocsPage = lazy(() => import("./features/docs-page/RecipesDocsPage"))

function RouteFallback() {
  return (
    <div className="flex min-h-64 w-full items-center justify-center p-6">
      <Spinner className="size-5" />
    </div>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <Suspense fallback={<RouteFallback />}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

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
            <Route index element={<Navigate to="/play" replace />} />

            <Route path="play">
              <Route index element={<GameModesPage />} />
              <Route path=":modeType" element={<GameArenasPage />} />
              <Route path=":modeType/:arenaId" element={<GameArenaPage />} />
              <Route path=":modeType/:arenaId/:sessionId" element={<GameArenaPage />} />
            </Route>

            <Route path="characters">
              <Route index element={<CharactersPage />} />
              <Route path="create" element={<CreateCharacterPage />} />
              <Route path=":id" element={<CharacterPage />} />
            </Route>

            <Route path="battle-archive">
              <Route index element={<Navigate to="history" replace />} />
              <Route path="history" element={<BattleHistoryPage />} />
              <Route path="hall-of-fame" element={<ChronicleNominationsPage />} />
              <Route path="stats" element={<InProgressPage title="Статистика боев" />} />
              <Route path="leaderboard" element={<InProgressPage title="Рейтинг" />} />
            </Route>

            <Route path="docs">
              <Route index element={<Navigate to="quick-start" replace />} />
              <Route path="quick-start" element={<QuickStartDocsPage />} />
              <Route path="recipes" element={<RecipesDocsPage />} />
              <Route path="script-api" element={<ScriptApiDocsPage />} />
            </Route>

            <Route path="settings" element={<InProgressPage title="Настройки" />} />
            <Route path="account" element={<AccountPage />} />
            <Route path="*" element={<NotFoundPage />} />
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Suspense>
    </BrowserRouter>
  )
}

import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { LoaderState } from '@/components/common/LoaderState'
import { Skeleton } from '@/components/ui/skeleton'
import { cn } from '@/shared/lib/utils'

import { useArena } from './hooks/useArena'
import { useArenaBreadcrumbs } from '@/shared/hooks/useArenaBreadcrumbs'
import { useCharacters } from '../characters-page/useCharacters'
import { useStartGame } from '../game/api/useStartGame'
import { BattleScreen } from '../battle/BattleScreen'

function GameArenaPageSkeleton() {
  return (
    <div className="flex h-full gap-4">
      <Skeleton className="h-full w-64 rounded-xl" />
      <Skeleton className="h-full flex-1 rounded-xl" />
    </div>
  )
}

/**
 * Страница арены: выбор героя и старт ручного боя.
 * Режим программирования скрыт: бой идёт под управлением игрока.
 */
export default function GameArenaPage() {
  const { modeType, arenaId } = useParams()

  const [sessionId, setSessionId] = useState<string | null>(null)
  const [selectedCharacterId, setSelectedCharacterId] = useState<string | null>(null)

  const { data: arena, isLoading, error } = useArena(arenaId)
  const { data: characters, isLoading: charactersLoading } = useCharacters()

  const startGame = useStartGame()

  useArenaBreadcrumbs({ modeType, arena })

  // Полный перезапуск при выходе из сессии: чистим сторы боя.
  useEffect(() => {
    return () => {
      setSessionId(null)
    }
  }, [])

  if (sessionId) {
    return <BattleScreen sessionId={sessionId} />
  }

  const canStart = Boolean(arena && selectedCharacterId) && !startGame.isPending

  const handleStart = () => {
    if (!arena || !selectedCharacterId || !modeType) return

    startGame.mutate(
      {
        arenaId: arena.id,
        mode: modeType,
        characterId: selectedCharacterId,
        code: null,
      },
      { onSuccess: setSessionId },
    )
  }

  return (
    <div className="flex h-full min-h-0 gap-4">
      <LoaderState
        isLoading={isLoading}
        error={error}
        skeletonClassName="w-72 h-full"
        loadingFallback={<GameArenaPageSkeleton />}
      >
        <Card className="h-full w-72 shrink-0 border-[#2c3a24] bg-[#161f12]/80">
          <CardHeader>
            <CardTitle className="text-[#e8d9a0]">Выбор героя</CardTitle>
          </CardHeader>

          <CardContent className="flex min-h-0 flex-col gap-3">
            {charactersLoading && <Skeleton className="h-24 w-full" />}

            <div className="flex min-h-0 flex-1 flex-col gap-2 overflow-y-auto pr-1">
              {characters?.map((character) => (
                <button
                  key={character.id}
                  type="button"
                  onClick={() => { setSelectedCharacterId(character.id); }}
                  className={cn(
                    'flex flex-col items-start rounded-lg border p-3 text-left transition-colors',
                    selectedCharacterId === character.id
                      ? 'border-[#9c7b3a] bg-[#2c3a24]'
                      : 'border-[#3a4a30] bg-[#1a2415] hover:border-[#9c7b3a]',
                  )}
                >
                  <span className="text-sm font-semibold text-[#e8d9a0]">{character.name}</span>
                  <span className="text-xs text-[#a8a88f]">
                    {character.className} · {character.specName}
                  </span>
                </button>
              ))}
            </div>

            <Button
              className="w-full border-[#9c7b3a] bg-[#2c3a24] text-[#e8d9a0] hover:bg-[#3a4a30]"
              variant="outline"
              disabled={!canStart}
              onClick={() => { handleStart() }}
            >
              {startGame.isPending ? 'Рубка начинается...' : 'В бой!'}
            </Button>
          </CardContent>
        </Card>
      </LoaderState>

      <Card className="min-h-0 flex-1 border-[#2c3a24] bg-[#161f12]/80">
        <CardHeader>
          <CardTitle className="text-[#e8d9a0]">{arena?.name ?? 'Арена'}</CardTitle>
        </CardHeader>
        <CardContent className="min-h-0 overflow-y-auto text-sm text-[#c9c9b0]">
          <p>{arena?.description}</p>
          <p className="mt-3 text-xs text-[#a8a88f]">
            Поле {arena?.gridWidth}×{arena?.gridHeight} гексов. Управляйте героем вручную:
            перемещение тратит очки шагов, способности — очки действия и ману.
          </p>
        </CardContent>
      </Card>
    </div>
  )
}

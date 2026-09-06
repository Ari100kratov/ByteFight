import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'

import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Skeleton } from '@/components/ui/skeleton'
import { cn } from '@/shared/lib/utils'

import { fetchCharacterTalents, learnTalent, resetTalents } from './api/talentsApi'

interface TalentsTreeProps {
  characterId: string
}

/**
 * Дерево талантов героя: две ветки по три ступени,
 * пассивки и активки, прокачка в один клик.
 */
export function TalentsTree({ characterId }: TalentsTreeProps) {
  const queryClient = useQueryClient()

  const { data, isLoading } = useQuery({
    queryKey: ['character-talents', characterId],
    queryFn: () => fetchCharacterTalents(characterId),
  })

  const learn = useMutation({
    mutationFn: (talentId: string) => learnTalent(characterId, talentId),
    onSuccess: () => {
      toast.success('Талант постигнут')
      queryClient.invalidateQueries({ queryKey: ['character-talents', characterId] }).catch((err: unknown) => { console.error('Не удалось обновить таланты:', err) })
    },
    onError: (error: Error) => toast.error(error.message),
  })

  const reset = useMutation({
    mutationFn: () => resetTalents(characterId),
    onSuccess: () => {
      toast.success('Таланты сброшены, очки возвращены')
      queryClient.invalidateQueries({ queryKey: ['character-talents', characterId] }).catch((err: unknown) => { console.error('Не удалось обновить таланты:', err) })
    },
    onError: (error: Error) => toast.error(error.message),
  })

  if (isLoading || !data) {
    return <Skeleton className="h-64 w-full rounded-xl" />
  }

  const tiers = [1, 2, 3]

  return (
    <div className="flex h-full min-h-0 flex-col gap-3">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <Badge variant="outline" className="border-[#9c7b3a] text-[#e8d9a0]">
            Уровень {data.level}
          </Badge>
          <Badge variant="outline" className="border-[#9c7b3a] text-[#e8d9a0]">
            Очки талантов: {data.talentPointsAvailable}
          </Badge>
        </div>

        <Button
          size="sm"
          variant="outline"
          className="border-[#9c7b3a] text-[#c9c9b0] hover:bg-[#2c3a24]"
          disabled={data.talentPointsSpent === 0 || reset.isPending}
          onClick={() => { reset.mutate() }}
        >
          Сбросить
        </Button>
      </div>

      <div className="grid min-h-0 flex-1 grid-cols-2 gap-3 overflow-y-auto pr-1">
        {data.branches.map((branch) => (
          <div key={branch.id} className="flex flex-col gap-2 rounded-xl border border-[#2c3a24] bg-[#161f12]/80 p-3">
            <div>
              <div className="text-sm font-semibold text-[#e8d9a0]">{branch.name}</div>
              <div className="text-xs text-[#a8a88f]">{branch.description}</div>
            </div>

            {tiers.map((tier) => (
              <div key={tier} className="flex flex-col gap-1.5 border-t border-[#2c3a24] pt-2">
                {branch.nodes
                  .filter((node) => node.tier === tier)
                  .map((node) => (
                    <button
                      key={node.id}
                      type="button"
                      disabled={!node.canLearn || learn.isPending}
                      onClick={() => { learn.mutate(node.id); }}
                      title={`${node.description}\nТребуется уровень ${String(tier * 2)}`}
                      className={cn(
                        'flex flex-col items-start gap-0.5 rounded-lg border p-2 text-left transition-colors',
                        node.currentRank > 0
                          ? 'border-[#9c7b3a] bg-[#2c3a24]'
                          : node.canLearn
                            ? 'border-[#3a4a30] bg-[#1a2415] hover:border-[#9c7b3a]'
                            : 'cursor-not-allowed border-[#2c3a24] bg-[#141b12] opacity-50',
                      )}
                    >
                      <span className="flex w-full items-center justify-between">
                        <span className="text-sm text-[#e8d9a0]">{node.name}</span>
                        <span className="flex items-center gap-1">
                          {Array.from({ length: node.maxRank }).map((_, i) => (
                            <span
                              key={i}
                              className={cn(
                                'inline-block size-2 rounded-full',
                                i < node.currentRank ? 'bg-[#e8d9a0]' : 'bg-[#3a4a30]',
                              )}
                            />
                          ))}
                        </span>
                      </span>
                      <span className="text-[11px] leading-tight text-[#a8a88f]">
                        {node.isPassive ? 'Пассивный' : 'Активный'} · {node.description}
                      </span>
                    </button>
                  ))}
              </div>
            ))}
          </div>
        ))}
      </div>
    </div>
  )
}

import { Button } from "@/components/ui/button"

type BattleHistoryPaginationProps = {
  page: number
  pageSize: number
  totalCount: number
  isFetching?: boolean
  onPageChange: (page: number) => void
}

export function BattleHistoryPagination({
  page,
  pageSize,
  totalCount,
  isFetching = false,
  onPageChange,
}: BattleHistoryPaginationProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize))
  const canGoPrev = page > 1
  const canGoNext = page < totalPages

  const from = totalCount === 0 ? 0 : (page - 1) * pageSize + 1
  const to = Math.min(page * pageSize, totalCount)

  return (
    <div className="flex items-center justify-between gap-4">
      <div className="text-sm text-muted-foreground">
        {isFetching
          ? "Обновляем список..."
          : `Показано ${from}–${to} из ${totalCount}`}
      </div>

      <div className="flex items-center">
        <Button
          variant="outline"
          onClick={() => onPageChange(page - 1)}
          disabled={!canGoPrev}
        >
          Назад
        </Button>

        <div className="text-sm text-muted-foreground min-w-[90px] text-center">
          {page} / {totalPages}
        </div>

        <Button
          variant="outline"
          onClick={() => onPageChange(page + 1)}
          disabled={!canGoNext}
        >
          Вперед
        </Button>
      </div>
    </div>
  )
}
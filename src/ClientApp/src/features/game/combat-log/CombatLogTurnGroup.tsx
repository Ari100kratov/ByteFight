import { Separator } from "@/components/ui/separator"
import { Badge } from "@/components/ui/badge"
import { CombatLogEntryItem } from "./CombatLogEntryItem"
import type { TurnLog } from "../types/TurnLog"

interface Props {
  turn: TurnLog
}

export function CombatLogTurnGroup({ turn }: Props) {
  return (
    <div className="mb-4">
      <div className="flex items-center gap-2 mb-2">
        <Badge variant="secondary">
          Ход {turn.turnIndex}
        </Badge>
        <Separator className="flex-1" />
      </div>

      <div className="space-y-2">
        {turn.logs.map(entry => (
          <CombatLogEntryItem
            key={entry.id}
            entry={entry}
          />
        ))}
      </div>
    </div>
  )
}
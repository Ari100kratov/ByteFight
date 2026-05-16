import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { cn } from "@/shared/lib/utils"

import type { ChronicleNominationLeaderboard } from "../types"
import { NominationEntry } from "./NominationEntry"
import { getNominationVisual } from "./nominationVisuals"

type NominationCardProps = {
  nomination: ChronicleNominationLeaderboard
  canOpenCharacters: boolean
}

export function NominationCard({
  nomination,
  canOpenCharacters,
}: NominationCardProps) {
  const visual = getNominationVisual(nomination.code)
  const Icon = visual.Icon
  const podium = nomination.entries.slice(0, 3)
  const rest = nomination.entries.slice(3)

  return (
    <Card className="relative overflow-hidden border-foreground/10 bg-card/95">
      <div className={cn("absolute inset-x-0 top-0 h-32 bg-gradient-to-b", visual.accent)} />
      <CardHeader className="relative">
        <div className="flex items-start gap-4">
          <div className={cn("flex size-12 shrink-0 items-center justify-center rounded-2xl", visual.glow)}>
            <Icon className="size-6" />
          </div>
          <div className="min-w-0">
            <CardTitle className="leading-tight">{nomination.title}</CardTitle>
            <CardDescription className="mt-2 line-clamp-2">
              {nomination.description}
            </CardDescription>
          </div>
        </div>

        <div className="mt-4 flex flex-wrap gap-2">
          <Badge variant="secondary">{nomination.metricLabel}</Badge>
          <Badge variant="outline">
            {nomination.sortDirection === "ascending" ? "меньше лучше" : "больше лучше"}
          </Badge>
        </div>
      </CardHeader>

      <CardContent className="relative space-y-3">
        <div className="grid gap-2">
          {podium.map(entry => (
            <NominationEntry
              key={entry.characterId}
              entry={entry}
              metricUnit={nomination.metricUnit}
              canOpenCharacter={canOpenCharacters}
            />
          ))}
        </div>

        {rest.length > 0 && (
          <div className="grid gap-2 border-t pt-3">
            {rest.map(entry => (
              <NominationEntry
                key={entry.characterId}
                entry={entry}
                metricUnit={nomination.metricUnit}
                canOpenCharacter={canOpenCharacters}
                compact
              />
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  )
}

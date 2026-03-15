import {
  isAttack,
  isDeath,
  isIdle,
  isWalk,
  type GameActionLogEntry,
} from "../types/TurnLog"

interface Props {
  entry: GameActionLogEntry
}

function formatPosition(x: number, y: number) {
  return `(${x}, ${y})`
}

export function formatCombatLog({ entry }: Props) {
  const actorClass = "font-medium text-foreground"
  const accentClass = "font-medium text-foreground"

  if (isAttack(entry)) {
    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" атакует "}
        <span className={actorClass}>{entry.targetName}</span>
        {" и наносит "}
        <span className={accentClass}>{entry.damage}</span>
        {" урона"}
      </>
    )
  }

  if (isWalk(entry)) {
    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" перемещается в клетку "}
        <span className={accentClass}>
          {formatPosition(entry.to.x, entry.to.y)}
        </span>
      </>
    )
  }

  if (isDeath(entry)) {
    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" погибает"}
      </>
    )
  }

  if (isIdle(entry)) {
    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" пропускает ход"}
      </>
    )
  }

  return null
}
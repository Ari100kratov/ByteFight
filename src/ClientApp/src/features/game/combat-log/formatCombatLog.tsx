import { AbilityEffectType } from "@/shared/types/ability"
import {
  isAbilityUsed,
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

function formatAbilityName(name?: string | null) {
  return name?.trim() ? ` «${name}»` : ""
}

export function formatCombatLog({ entry }: Props) {
  const actorClass = "font-medium text-foreground"
  const accentClass = "font-medium text-foreground"

  if (isAbilityUsed(entry)) {
    const abilityName = formatAbilityName(entry.abilityName)

    if (entry.effectType === AbilityEffectType.Damage) {
      const hasAbilityName = !!abilityName

      if (hasAbilityName) {
        return (
          <>
            <span className={actorClass}>{entry.actorName}</span>
            {" использует "}
            {abilityName}
            {" против "}
            <span className={actorClass}>{entry.targetName}</span>
            {" и наносит "}
            <span className={accentClass}>{entry.value}</span>
            {" урона"}
          </>
        )
      }

      return (
        <>
          <span className={actorClass}>{entry.actorName}</span>
          {" атакует "}
          <span className={actorClass}>{entry.targetName}</span>
          {" и наносит "}
          <span className={accentClass}>{entry.value}</span>
          {" урона"}
        </>
      )
    }

    if (entry.effectType === AbilityEffectType.Healing) {
      const isSelfHeal = entry.actorId === entry.targetId

      if (isSelfHeal) {
        return (
          <>
            <span className={actorClass}>{entry.actorName}</span>
            {" использует"}
            {abilityName}
            {" и восстанавливает себе "}
            <span className={accentClass}>{entry.value}</span>
            {" здоровья"}
          </>
        )
      }

      return (
        <>
          <span className={actorClass}>{entry.actorName}</span>
          {" использует"}
          {abilityName}
          {" и восстанавливает "}
          <span className={accentClass}>{entry.value}</span>
          {" здоровья "}
          <span className={actorClass}>{entry.targetName}</span>
        </>
      )
    }

    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" применяет"}
        {abilityName || " способность"}
      </>
    )
  }

  if (isWalk(entry)) {
    return (
      <>
        <span className={actorClass}>{entry.actorName}</span>
        {" перемещается на клетку "}
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
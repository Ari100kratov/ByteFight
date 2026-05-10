# Arena Items System Architecture

## Назначение

Документ описывает текущую архитектуру системы предметов на арене (Arena Items) в проекте ByteFight.

Цель:
- дать единый reference для будущих изменений;
- упростить автоматизацию через AI/Codex;
- сохранить единые архитектурные решения;
- избежать дублирования и расхождения логики между backend/frontend/runtime.

---

# Общая концепция

Предметы:
- размещаются на конкретной арене;
- лежат в определённой клетке;
- могут быть подняты юнитом при входе в клетку;
- применяют эффект мгновенно;
- после подбора удаляются из runtime-состояния арены;
- отображаются на фронте как отдельные Pixi-сущности.

---

# Архитектурные принципы

## 1. Разделение шаблона предмета и размещённого предмета

ВАЖНО:
один и тот же предмет может использоваться на нескольких аренах.

Поэтому:
- `ArenaItem` = шаблон предмета;
- `ArenaPlacedItem` = размещение предмета на конкретной арене.

---

# Domain

## ArenaItem (шаблон)

Namespace:

```csharp
Domain.Game.ArenaItems
```

Содержит:
- Id
- Name
- Description
- Type
- Sprite
- Value

Не содержит:
- позиции;
- ArenaId.

---

## ArenaPlacedItem

Namespace:

```csharp
Domain.Game.Arenas.ArenaPlacedItems
```

Содержит:
- ArenaId
- ItemId
- Position

---

# Runtime

## ArenaItemDefinition

Namespace:

```csharp
GameRuntime.Common.World.ArenaItems
```

Runtime DTO предмета внутри боя.

Содержит:
- PlacedItemId
- ItemId
- Type
- Name
- Description
- Position
- Value
- Sprite

---

# ArenaDefinition

Runtime arena хранит предметы внутри себя.

```csharp
public required IReadOnlyList<ArenaItemDefinition> Items { get; init; }
```

---

# Инкапсуляция логики

## Удаление предмета

Удаление предмета выполняется на уровне арены:

```csharp
Arena.RemoveItem(Guid placedItemId)
```

НЕ внутри `ArenaWorld`.

Причина:
предметы являются частью состояния арены.

---

## Поиск предмета

```csharp
Arena.GetItemAt(Position position)
```

---

# Подбор предмета

## Точка входа

Подбор предмета происходит внутри:

```csharp
MoveAction.Execute()
```

После перемещения юнита.

---

## Логика

Алгоритм:

1. Юнит перемещается.
2. Создаётся WalkLogEntry.
3. Проверяется предмет в новой клетке.
4. Если предмет найден:
   - удаляется из арены;
   - применяется к юниту;
   - создаётся ItemPickedUpLogEntry.

---

# Применение предметов

## ВАЖНО

Логика применения предметов инкапсулирована внутри:

```csharp
BaseUnit.ApplyItem()
```

НЕ в MoveAction.

---

## Пример

```csharp
public StatSnapshot ApplyItem(ArenaItemDefinition item)
{
    ThrowIfDead();

    return item.Type switch
    {
        ArenaItemType.HealingPotion => Stats.Heal(item.Value),
        _ => throw new NotImplementedException(
            $"Item type '{item.Type}' is not supported yet.")
    };
}
```

---

# RuntimeStats

## Использовать существующие методы

Запрещено вручную изменять HP:

ПЛОХО:

```csharp
Stats.Set(...)
```

ХОРОШО:

```csharp
Stats.Heal(...)
Stats.ApplyDamage(...)
```

---

# Fail Fast

## Не реализованные предметы

Если тип предмета ещё не поддерживается:

```csharp
throw new NotImplementedException(...)
```

Это intentional behavior.

Причина:
AI/Codex должен сразу видеть не реализованный сценарий.

---

# Защита от некорректных действий

Все действия юнита должны проверять:

```csharp
ThrowIfDead()
```

Минимум:
- Move
- Turn
- ApplyItem

---

# Combat Logs

## Новый тип

Добавлен:

```csharp
ItemPickedUp
```

---

## ItemPickedUpLogEntry

Содержит:
- actor;
- placedItemId;
- itemType;
- itemName;
- value;
- actorHp.

---

# Frontend Architecture

# Arena Items Store

Store:

```ts
useArenaItemsStateStore
```

---

## ВАЖНО

Нельзя blindly overwrite state.

ПЛОХО:

```ts
set({ items })
```

Причина:
повторный refetch может resurrect already picked items.

---

## Правильный подход

Merge/update only.

Store должен:
- обновлять существующие;
- добавлять новые;
- сохранять локально удалённые.

---

# Floating Combat Text

## Не использовать AbilityEffectType

Предметы не являются ability.

Вместо этого используется отдельный enum:

```ts
CombatTextType
```

Примеры:
- Damage
- Healing
- ManaRestore
- Shield
- Poison

---

# Pixi Rendering

# ВАЖНО

Предметы НЕ должны перекрывать юнитов.

---

## zIndex

Предмет:

```ts
const zIndex = cell.y + cell.height - 50
```

Юнит:

```ts
const zIndex = spriteY + healthPriority
```

---

# Anchor

Для предметов используется:

```tsx
anchor={{ x: 0.5, y: 1 }}
```

Причина:
нижняя точка предмета считается точкой опоры.

---

# Hover Animation

Предметы имеют idle hover animation:

```ts
Math.sin(time) * amplitude
```

Параметры:

```ts
const HOVER_AMPLITUDE = 5
const HOVER_SPEED = 0.004
```

---

# Single Frame Sprites

Если:

```ts
frameCount <= 1
```

используется:

```tsx
<pixiSprite />
```

НЕ:

```tsx
<pixiAnimatedSprite />
```

Причина:
Pixi AnimatedSprite для single-frame избыточен.

---

# Asset Loading

## Single texture

```ts
getOrLoadTexture()
```

---

## Animated textures

```ts
getOrLoadTextures()
```

---

# UI

## Arena Cards

Карточки арен отображают:
- размеры;
- врагов;
- предметы.

---

## Item Icon

Используется:

```tsx
<FlaskConical />
```

---

# Popovers

Предметы имеют отдельный popover.

Не используют:

```tsx
UnitPreview
```

Причина:
предметы не имеют:
- abilities;
- action assets;
- stats.

---

# Runtime Animation Processing

## Item pickup

При обработке:

```ts
isItemPickedUp(entry)
```

нужно:
1. удалить предмет из store;
2. показать floating text;
3. обновить hp юнита.

---

# Seeder Rules

## Healing Potion

Текущий предмет:
- HealingPotion
- Heal: 100 HP

---

## Naming Style

Допускается лёгкий юмор.

Пример:
- "Подозрительная настойка"
- "Зелье из очень сертифицированных грибов"

---

# EF Core

## Owned Position Index Problem

Нельзя индексировать:

```csharp
x.Position.X
```

в anonymous object через owned type.

EF Core 10 design-time может падать.

---

## Recommendation

Не создавать индекс по Position unless really needed.

Сейчас индекс не нужен.

---

# Documentation UX

В редакторе поведения добавлена ссылка:

```tsx
/docs/script-api
```

Открывается:
- в новой вкладке;
- через action-link в CardHeader.

---

# Future Extensions

Система должна легко поддерживать:

- mana potions;
- buffs;
- temporary effects;
- traps;
- gold/resources;
- consumables;
- interactable arena objects.

---

# Main Design Philosophy

## Runtime = authoritative

Все игровые эффекты:
- применяются только на backend/runtime;
- frontend лишь воспроизводит logs/state.

---

## Logs drive visuals

Frontend animation/state driven by:

```ts
GameActionLogEntry[]
```

НЕ локальной логикой клиента.

---

# IMPORTANT

При добавлении новых предметов:

1. Добавить новый ArenaItemType.
2. Реализовать ApplyItem().
3. Добавить новый CombatTextType при необходимости.
4. Добавить frontend formatting.
5. Добавить ItemPickedUp UI behavior.
6. Добавить seeder.
7. Добавить docs/API exposure.

Никогда не изменять HP напрямую вне RuntimeStats.


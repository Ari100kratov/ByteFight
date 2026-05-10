import type { ReactNode } from "react"

export type CodeExample = {
  title: string
  description: string
  code: string
  usedApi: string[]
}

export type QuickStartFact = {
  title: string
  content: ReactNode
}

export const defaultScriptExample = `// Берём только живых врагов и выбираем цель.
// Сначала приоритет у тех, кого уже можно атаковать.
// Если таких нет — выбираем ближайшего.
var target = world.AliveEnemies
    .OrderBy(e => world.Self.CanAttack(e) ? 0 : 1)
    .ThenBy(e => world.Self.DistanceTo(e))
    .FirstOrDefault();

// Если живых врагов нет — пропускаем ход.
if (target is null)
{
    return new Idle();
}

// Если цель уже в радиусе атаки — атакуем её.
if (world.Self.CanAttack(target))
{
    return new Attack(target.Id);
}

// Иначе двигаемся в сторону выбранной цели.
return new MoveTowards(target.Id);`

export const quickStartFacts: QuickStartFact[] = [
  {
    title: "Модель выполнения",
    content: (
      <>
        Каждый ход игра вызывает <code>Decide(UserWorldView world)</code>.
        В объекте <code>world</code> находится текущее состояние боя:
        твой персонаж, враги, арена и прочее.
      </>
    ),
  },
  {
    title: "Что должен вернуть скрипт",
    content: (
      <>
        Скрипт должен вернуть действие персонажа:
        <code>Attack</code>, <code>MoveTowards</code>,
        <code>Idle</code> или другое действие из доступного API.
      </>
    ),
  },
  {
    title: ".NET 10 и C# 14",
    content: (
      <>
        Код выполняется внутри <code>.NET 10</code> с поддержкой
        современных возможностей <code>C# 14</code>.
      </>
    ),
  },
  {
    title: "Доступные возможности языка",
    content: (
      <>
        Можно использовать <code>LINQ</code>, <code>collections</code>,
        <code>pattern matching</code>, <code>nullable reference types</code>,
        <code>Math</code>, <code>records</code>,
        <code>switch expressions</code> и другие стандартные возможности платформы.
      </>
    ),
  },
  {
    title: "Локальные методы",
    content: (
      <>
        Повторяющуюся логику удобно выносить в методы в конце скрипта:
        <code>bool IsPathBlocked(Position target) &#123; ... &#125;</code>.
      </>
    ),
  },
  {
    title: "Запуск боя и сохранение",
    content: (
      <>
        При запуске боя используется текущий код из редактора. 
        Его необязательно всегда сохранять перед тестовым запуском.
      </>
    ),
  },
  {
    title: "Доступные using",
    content: (
      <>
        Помимо внутренних пространств имен игрового API,
        в шаблон уже добавлены <code>System</code>,
        <code>System.Linq</code> и
        <code>System.Collections.Generic</code>.
      </>
    ),
  },
  {
    title: "Не хватает API или using?",
    content: (
      <>
        Если для стратегии не хватает метода, свойства, действия
        или стандартного <code>using</code> из .NET —
        напиши через пункт «Связаться со мной»
        или через форму «Обратная связь» в боковом меню.
      </>
    ),
  },
]

export const recipes: CodeExample[] = [
  {
    title: "Выбрать врага с лечением",
    description:
      "Полезно, если на арене есть лекарь. Такой враг часто должен быть первой целью.",
    usedApi: [
      "world.AliveEnemies",
      "enemy.Abilities.Has(AbilityType.Healing)",
    ],
    code: `var healer = world.AliveEnemies
    .FirstOrDefault(e => e.Abilities.Has(AbilityType.Healing));`,
  },
  {
    title: "Выбрать цель для дальней атаки",
    description:
      "Ищет врага, которого можно атаковать базовой дальней атакой. Соседние цели исключаются, чтобы наносить максимальный урон при наличии такой возможности.",
    usedApi: [
      "world.Self.CanAttackRanged(enemy)",
      "world.Self.DistanceTo(enemy)",
      "enemy.Health",
    ],
    code: `var rangedTarget = world.AliveEnemies
    .Where(e => world.Self.CanAttackRanged(e))
    .Where(e => world.Self.DistanceTo(e) > 1)
    .OrderBy(e => e.Health)
    .FirstOrDefault();`,
  },
  {
    title: "Проверить, что путь к цели заблокирован",
    description:
      "Помогает понять, нужно ли сначала расчистить проход, а не просто идти к точке.",
    usedApi: [
      "world.GetWalkableNeighbors4(position)",
      "Position.ManhattanDistance(position)",
    ],
    code: `bool IsPathBlocked(Position target)
{
    return !world.GetWalkableNeighbors4(world.Self.Position)
        .Any(p => p.ManhattanDistance(target) <
                  world.Self.Position.ManhattanDistance(target));
}`,
  },
  {
    title: "Найти предмет для восполнения здоровья",
    description:
      "Ищет лечебное зелье среди предметов арены.",
    usedApi: [
      "world.Arena.Items",
      "ArenaItemType.HealingPotion",
    ],
    code: `var healingPotion = world.Arena.Items
    .FirstOrDefault(i => i.Type == ArenaItemType.HealingPotion);`,
  },
  {
    title: "Выбрать более правую цель",
    description:
      "Пример позиционного приоритета. Бывает полезно, чтобы расчистить путь по правой стороне, например.",
    usedApi: [
      "enemy.Position.X",
      "enemy.Health",
    ],
    code: `var target = world.AliveEnemies
    .OrderByDescending(e => e.Position.X)
    .ThenBy(e => e.Health)
    .FirstOrDefault();`,
  },
  {
    title: "Не выходить из сильной позиции",
    description:
      "Если персонаж уже стоит на нужной клетке, он атакует доступные цели или пропускает ход.",
    usedApi: [
      "world.Self.Position",
      "world.Self.CanAttack(enemy)",
      "Attack",
      "Idle",
    ],
    code: `var strongPosition = new Position(8, 6);

if (world.Self.Position.Equals(strongPosition))
{
    var target = world.AliveEnemies
        .Where(world.Self.CanAttack)
        .OrderBy(e => e.Health)
        .FirstOrDefault();

    return target is null
        ? new Idle()
        : new Attack(target.Id);
}`,
  },
]
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
    title: "Найти ближайшего врага",
    description: "Возвращает ближайшего живого врага или null.",
    usedApi: [
      "world.AliveEnemies",
      "world.Self.DistanceTo(enemy)",
    ],
    code: `var target = world.AliveEnemies
    .OrderBy(e => world.Self.DistanceTo(e))
    .FirstOrDefault();`,
  },
  {
    title: "Найти врага с минимальным здоровьем",
    description: "Полезно для выбора цели на добивание.",
    usedApi: [
      "world.AliveEnemies",
      "enemy.Health",
    ],
    code: `var target = world.AliveEnemies
    .OrderBy(e => e.Health)
    .FirstOrDefault();`,
  },
  {
    title: "Найти врага с лечением",
    description: "Ищет живого врага, у которого есть способность лечения.",
    usedApi: [
      "enemy.Abilities.Has(AbilityType.Healing)",
    ],
    code: `var healer = world.AliveEnemies
    .FirstOrDefault(e => e.Abilities.Has(AbilityType.Healing));`,
  },
  {
    title: "Найти врага с дальнобойной атакой",
    description: "Ищет врага, у которого есть базовая дальняя атака.",
    usedApi: [
      "enemy.Abilities.Has(AbilityType.BasicRangedAttack)",
    ],
    code: `var rangedEnemy = world.AliveEnemies
    .FirstOrDefault(e => e.Abilities.Has(AbilityType.BasicRangedAttack));`,
  },
  {
    title: "Найти врага по названию способности",
    description: "Ищет врага, у которого есть способность с указанным фрагментом названия.",
    usedApi: [
      "enemy.Abilities.All",
      "ability.Name",
      "StringComparison.OrdinalIgnoreCase",
    ],
    code: `var enemy = world.AliveEnemies
    .FirstOrDefault(e => e.Abilities.All.Any(a =>
        a.Name.Contains("fire", StringComparison.OrdinalIgnoreCase)));`,
  },
  {
    title: "Найти врага с большим запасом здоровья",
    description: "Ищет самого крепкого врага по максимальному здоровью.",
    usedApi: [
      "enemy.MaxHealth",
    ],
    code: `var tank = world.AliveEnemies
    .OrderByDescending(e => e.MaxHealth)
    .FirstOrDefault();`,
  },
  {
    title: "Найти раненого врага",
    description: "Ищет врага, у которого осталось меньше 30% здоровья.",
    usedApi: [
      "enemy.HealthPercent",
    ],
    code: `var wounded = world.AliveEnemies
    .FirstOrDefault(e => e.HealthPercent < 0.3m);`,
  },
  {
    title: "Найти доступную цель для дальней атаки",
    description: "Ищет врага, которого можно достать базовой дальней атакой.",
    usedApi: [
      "world.Self.Abilities.Get(AbilityType.BasicRangedAttack)",
      "ability.CanReach(distance)",
      "world.Self.DistanceTo(enemy)",
    ],
    code: `var rangedAttack = world.Self.Abilities.Get(AbilityType.BasicRangedAttack);

var rangedTarget = rangedAttack is null
    ? null
    : world.AliveEnemies
        .Where(e => rangedAttack.CanReach(world.Self.DistanceTo(e)))
        .OrderBy(e => world.Self.DistanceTo(e))
        .FirstOrDefault();`,
  },
  {
    title: "Переключиться на дальнюю цель, если враг рядом",
    description: "Полезно для стрелка: если рядом стоит враг, ищем другую цель для дальней атаки.",
    usedApi: [
      "world.Self.DistanceTo(enemy)",
      "AbilityType.BasicRangedAttack",
      "ability.CanReach(distance)",
    ],
    code: `var hasEnemyNearby = world.AliveEnemies
    .Any(e => world.Self.DistanceTo(e) <= 1);

var rangedAttack = world.Self.Abilities.Get(AbilityType.BasicRangedAttack);

var rangedTarget = !hasEnemyNearby || rangedAttack is null
    ? null
    : world.AliveEnemies
        .Where(e => world.Self.DistanceTo(e) > 1)
        .Where(e => rangedAttack.CanReach(world.Self.DistanceTo(e)))
        .OrderBy(e => world.Self.DistanceTo(e))
        .ThenBy(e => e.Health)
        .FirstOrDefault();`,
  },
  {
    title: "Найти безопасную клетку арены",
    description: "Ищет свободную клетку, которая находится дальше всего от ближайшего врага.",
    usedApi: [
      "world.Arena.GridWidth",
      "world.Arena.GridHeight",
      "world.IsWalkable(position)",
      "enemy.Position.ManhattanDistance(position)",
    ],
    code: `var safePosition = Enumerable.Range(0, world.Arena.GridWidth)
    .SelectMany(x => Enumerable.Range(0, world.Arena.GridHeight)
        .Select(y => new Position(x, y)))
    .Where(world.IsWalkable)
    .OrderByDescending(position => world.AliveEnemies
        .Min(enemy => enemy.Position.ManhattanDistance(position)))
    .ThenBy(position => world.Self.Position.ManhattanDistance(position))
    .FirstOrDefault();`,
  },
  {
    title: "Найти все свободные клетки арены",
    description: "Фильтрует клетки арены, по которым можно ходить.",
    usedApi: [
      "new Position(x, y)",
      "world.IsWalkable(position)",
    ],
    code: `var walkablePositions = Enumerable.Range(0, world.Arena.GridWidth)
    .SelectMany(x => Enumerable.Range(0, world.Arena.GridHeight)
        .Select(y => new Position(x, y)))
    .Where(world.IsWalkable);`,
  },
  {
    title: "Найти узкое место на арене",
    description: "Ищет свободную клетку с минимальным количеством проходимых соседей.",
    usedApi: [
      "world.IsWalkable(position)",
      "world.GetWalkableNeighbors4(position)",
    ],
    code: `var chokePoint = Enumerable.Range(0, world.Arena.GridWidth)
    .SelectMany(x => Enumerable.Range(0, world.Arena.GridHeight)
        .Select(y => new Position(x, y)))
    .Where(world.IsWalkable)
    .OrderBy(position => world.GetWalkableNeighbors4(position).Count())
    .ThenBy(position => world.Self.Position.ManhattanDistance(position))
    .FirstOrDefault();`,
  },
]
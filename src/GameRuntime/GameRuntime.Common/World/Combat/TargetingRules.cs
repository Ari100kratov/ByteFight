using Domain.Game.Abilities;
using Domain.Game.Arenas;
using Domain.ValueObjects;
using GameRuntime.Common.World.Abilities;
using GameRuntime.Common.World.Units;

namespace GameRuntime.Common.World.Combat;

/// <summary>
/// Правила выбора целей на гексагональном поле: допустимые гексы
/// применения и поражаемые области для форм способностей.
/// </summary>
public static class TargetingRules
{
    /// <summary>
    /// Возвращает гексы, на которые юнит может применить способность:
    /// в пределах дальности, с учётом типа цели и линии видимости.
    /// </summary>
    public static IReadOnlyList<Position> GetTargetHexes(
        ArenaWorld world,
        BaseUnit actor,
        RuntimeAbility ability)
    {
        int range = ability.GetRange();

        IEnumerable<Position> candidates = actor.Position
            .GetHexesInRange(range)
            .Where(world.Arena.IsWithin);

        return ability.TargetType switch
        {
            AbilityTargetType.Self => [actor.Position],

            AbilityTargetType.Enemy => candidates
                .Where(p => HasHostileOccupant(world, actor, p))
                .Where(p => HasLineOfSight(world, actor.Position, p))
                .ToList(),

            AbilityTargetType.Ally => candidates
                .Where(p => HasFriendlyOccupant(world, actor, p))
                .ToList(),

            // Область: можно целиться в любой гекс в пределах дальности.
            AbilityTargetType.Area => candidates
                .Where(p => HasLineOfSight(world, actor.Position, p))
                .ToList(),

            _ => []
        };
    }

    /// <summary>
    /// Разрешает форму способности в множество поражаемых гексов.
    /// </summary>
    public static IReadOnlyList<Position> GetAffectedHexes(
        ArenaWorld world,
        BaseUnit actor,
        RuntimeAbility ability,
        Position targetHex)
    {
        switch (ability.Shape)
        {
            case AbilityShape.Radius:
            {
                int radius = Math.Max(1, ability.GetAreaRadius());
                return targetHex
                    .GetHexesInRange(radius)
                    .Where(world.Arena.IsWithin)
                    .ToList();
            }

            case AbilityShape.Line:
            {
                // Линия тянется от применяющего через цель до конца дальности.
                int range = ability.GetRange();
                Position direction = targetHex;
                List<Position> full = HexGeometry.GetLine(actor.Position, direction);

                if (full.Count > range + 1)
                {
                    full = [.. full.Take(range + 1)];
                }

                return full
                    .Skip(1) // гекс применяющего не поражается
                    .Where(world.Arena.IsWithin)
                    .ToList();
            }

            case AbilityShape.Cone:
            {
                int reach = Math.Max(1, ability.GetAreaRadius() is > 0 ? ability.GetAreaRadius() : 2);
                double aimAngle = GetAngleDegrees(actor.Position, targetHex);

                return actor.Position
                    .GetHexesInRange(reach)
                    .Where(world.Arena.IsWithin)
                    .Where(p => p != actor.Position)
                    .Where(p => Math.Abs(AngleDiff(GetAngleDegrees(actor.Position, p), aimAngle)) <= 60)
                    .ToList();
            }

            case AbilityShape.SingleTarget:
            default:
                return [targetHex];
        }
    }

    /// <summary>
    /// Проверяет прямую видимость между гексами: линию не должен
    /// перекрывать камень.
    /// </summary>
    public static bool HasLineOfSight(ArenaWorld world, Position from, Position to)
    {
        if (from == to)
        {
            return true;
        }

        foreach (Position cell in HexGeometry.GetLine(from, to))
        {
            if (cell == from || cell == to)
            {
                continue;
            }

            if (world.Arena.TerrainAt(cell) is TerrainType.Rock)
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasHostileOccupant(ArenaWorld world, BaseUnit actor, Position hex) =>
        world.GetOccupant(hex) is { } occupant && world.IsHostile(actor, occupant);

    private static bool HasFriendlyOccupant(ArenaWorld world, BaseUnit actor, Position hex) =>
        world.GetOccupant(hex) is { } occupant && !world.IsHostile(actor, occupant);

    private static double GetAngleDegrees(Position from, Position to)
    {
        (double x1, double y1) = HexGeometry.ToPixel(from, 1);
        (double x2, double y2) = HexGeometry.ToPixel(to, 1);

        return Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI;
    }

    private static double AngleDiff(double a, double b)
    {
        double diff = (a - b) % 360;

        if (diff > 180)
        {
            diff -= 360;
        }

        if (diff < -180)
        {
            diff += 360;
        }

        return Math.Abs(diff);
    }
}

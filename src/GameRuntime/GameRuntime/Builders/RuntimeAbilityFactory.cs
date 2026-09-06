using Domain.Game.Abilities;
using GameRuntime.Common.World.Abilities;

namespace GameRuntime.Builders;

/// <summary>
/// Создаёт runtime-способности из каталога — единственного источника
/// игровых значений. База хранит только состав способностей класса или врага.
/// </summary>
internal static class RuntimeAbilityFactory
{
    /// <summary>
    /// Создаёт runtime-способность по типу из каталога.
    /// </summary>
    public static RuntimeAbility Create(AbilityType type, int priority = 0)
    {
        AbilityDefinition definition = AbilityCatalog.Get(type);

        return new RuntimeAbility
        {
            Type = definition.Type,
            EffectType = definition.EffectType,
            TargetType = definition.TargetType,
            Shape = definition.Shape,
            Name = definition.Name,
            Priority = priority,
            Stats = definition.Stats,
            AppliedStatuses = definition.Statuses ?? [],
            DashesToTarget = definition.DashesToTarget
        };
    }
}

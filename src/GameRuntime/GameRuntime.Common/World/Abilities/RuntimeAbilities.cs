using Domain.Game.Abilities;

namespace GameRuntime.Common.World.Abilities;

/// <summary>
/// Хранит способности юнита и предоставляет методы выбора способности во время боя.
/// </summary>
public sealed class RuntimeAbilities
{
    private static readonly HashSet<AbilityType> BasicAttackTypes =
    [
        AbilityType.BasicMeleeAttack,
        AbilityType.BasicRangedAttack
    ];

    private readonly Dictionary<AbilityType, RuntimeAbility> byType;
    private readonly List<RuntimeAbility> orderedByPriority;

    /// <summary>
    /// Создает коллекцию runtime-способностей.
    /// </summary>
    public RuntimeAbilities(IEnumerable<RuntimeAbility> abilities)
    {
        orderedByPriority = [.. abilities.OrderByDescending(x => x.Priority)];
        byType = orderedByPriority.ToDictionary(x => x.Type);
    }

    /// <summary>
    /// Добавляет способность (например, открытую талантом).
    /// </summary>
    public void Add(RuntimeAbility ability)
    {
        if (byType.ContainsKey(ability.Type))
        {
            return;
        }

        byType[ability.Type] = ability;
        orderedByPriority.Add(ability);
        orderedByPriority.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    /// <summary>
    /// Все способности, отсортированные по приоритету.
    /// </summary>
    public IReadOnlyCollection<RuntimeAbility> All => orderedByPriority;

    /// <summary>
    /// Возвращает способность по типу.
    /// </summary>
    public RuntimeAbility Get(AbilityType type) => byType[type];

    /// <summary>
    /// Пытается найти способность по типу.
    /// </summary>
    public bool TryGet(AbilityType type, out RuntimeAbility ability) =>
        byType.TryGetValue(type, out ability!);

    /// <summary>
    /// Проверяет наличие способности указанного типа.
    /// </summary>
    public bool Has(AbilityType type) => byType.ContainsKey(type);

    /// <summary>
    /// Возвращает базовые атакующие способности, доступные на указанной дистанции.
    /// </summary>
    public IEnumerable<RuntimeAbility> FindAvailableBasicAttacks(int distance) =>
        orderedByPriority.Where(x =>
            BasicAttackTypes.Contains(x.Type) &&
            x.TargetType == AbilityTargetType.Enemy &&
            x.EffectType == AbilityEffectType.Damage &&
            x.CanReach(distance));

    /// <summary>
    /// Возвращает лучшую базовую атакующую способность на указанной дистанции.
    /// </summary>
    public RuntimeAbility? FindBestBasicAttack(int distance) =>
        FindAvailableBasicAttacks(distance).FirstOrDefault();

    /// <summary>
    /// Возвращает лучшую исцеляющую способность на указанной дистанции.
    /// </summary>
    public RuntimeAbility? FindBestHealingAbility(int distance) =>
        orderedByPriority
            .FirstOrDefault(x =>
                x.EffectType == AbilityEffectType.Healing &&
                x.TargetType == AbilityTargetType.Ally &&
                x.CanReach(distance));

    /// <summary>
    /// Возвращает лучшую исцеляющую способность без проверки дистанции.
    /// </summary>
    public RuntimeAbility? FindBestHealingAbility() =>
        orderedByPriority
            .FirstOrDefault(x =>
                x.EffectType == AbilityEffectType.Healing &&
                x.TargetType == AbilityTargetType.Ally);
}

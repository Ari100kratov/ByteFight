using Domain.Game.Abilities;
using SharedKernel;

namespace GameRuntime.Logic.User.Api;

/// <summary>
/// Представление списка способностей юнита, доступное пользовательскому скрипту.
/// </summary>
[UserCodeApi]
public sealed class UserAbilitiesView
{
    private static readonly HashSet<AbilityType> BasicAttackTypes =
    [
        AbilityType.BasicMeleeAttack,
        AbilityType.BasicRangedAttack
    ];

    /// <summary>
    /// Все способности юнита, отсортированные по приоритету.
    /// </summary>
    public required IReadOnlyList<UserAbilityView> All { get; init; }

    private Dictionary<AbilityType, UserAbilityView> ByType =>
        field ??= All.ToDictionary(x => x.Type);

    /// <summary>
    /// Возвращает способность по типу или null, если такой способности нет.
    /// </summary>
    public UserAbilityView? Get(AbilityType type) =>
        ByType.TryGetValue(type, out UserAbilityView? ability) ? ability : null;

    /// <summary>
    /// Проверяет, есть ли у юнита способность указанного типа.
    /// </summary>
    public bool Has(AbilityType type) => ByType.ContainsKey(type);

    /// <summary>
    /// Возвращает базовые атакующие способности, доступные на указанной дистанции.
    /// </summary>
    public IEnumerable<UserAbilityView> FindAvailableBasicAttacks(int distance) =>
        All.Where(x =>
            BasicAttackTypes.Contains(x.Type) &&
            x.TargetType == AbilityTargetType.Enemy &&
            x.EffectType == AbilityEffectType.Damage &&
            x.CanReach(distance));

    /// <summary>
    /// Возвращает лучшую базовую атакующую способность на указанной дистанции.
    /// </summary>
    public UserAbilityView? FindBestBasicAttack(int distance) =>
        FindAvailableBasicAttacks(distance).FirstOrDefault();

    /// <summary>
    /// Возвращает доступные базовые дальние атаки на указанной дистанции.
    /// </summary>
    public IEnumerable<UserAbilityView> FindAvailableBasicRangedAttacks(int distance) =>
        All.Where(x =>
            x.Type == AbilityType.BasicRangedAttack &&
            x.TargetType == AbilityTargetType.Enemy &&
            x.EffectType == AbilityEffectType.Damage &&
            x.CanReach(distance));

    /// <summary>
    /// Возвращает лучшую базовую дальнюю атаку на указанной дистанции.
    /// </summary>
    public UserAbilityView? FindBestBasicRangedAttack(int distance) =>
        FindAvailableBasicRangedAttacks(distance).FirstOrDefault();
}

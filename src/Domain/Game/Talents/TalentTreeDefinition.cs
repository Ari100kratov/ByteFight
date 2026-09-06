using Domain.Game.CharacterClasses;

namespace Domain.Game.Talents;

/// <summary>
/// Дерево талантов класса: ветки и входящие в них узлы.
/// </summary>
public sealed record TalentTreeDefinition(
    CharacterClassType Class,
    IReadOnlyList<TalentBranchDefinition> Branches,
    IReadOnlyList<TalentNodeDefinition> Nodes)
{
    /// <summary>
    /// Ищет узел по идентификатору.
    /// </summary>
    public TalentNodeDefinition? FindNode(string talentId) =>
        Nodes.FirstOrDefault(x => x.Id == talentId);

    /// <summary>
    /// Возвращает узлы указанной ветки, упорядоченные по ступени.
    /// </summary>
    public IEnumerable<TalentNodeDefinition> GetBranchNodes(string branchId) =>
        Nodes.Where(x => x.BranchId == branchId).OrderBy(x => x.Tier);
}

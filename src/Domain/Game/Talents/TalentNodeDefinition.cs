namespace Domain.Game.Talents;

/// <summary>
/// Ветка (стезя) талантов внутри класса.
/// </summary>
/// <param name="Id">Слаг-идентификатор ветки, уникален в пределах класса.</param>
/// <param name="Name">Название ветки.</param>
/// <param name="Description">Краткое описание концепции ветки.</param>
public sealed record TalentBranchDefinition(
    string Id,
    string Name,
    string Description);

/// <summary>
/// Узел таланта в ветке. Каждый ранг узла применяется поверх предыдущего.
/// </summary>
/// <param name="Id">Слаг-идентификатор узла, уникален глобально.</param>
/// <param name="Name">Название таланта.</param>
/// <param name="Description">Описание эффекта для игрока.</param>
/// <param name="BranchId">Идентификатор ветки, которой принадлежит узел.</param>
/// <param name="Tier">Уровень узла в ветке (1–3). Узел уровня N доступен, если взят любой узел уровня N-1 той же ветки.</param>
/// <param name="MaxRank">Максимальный ранг узла.</param>
/// <param name="NodeType">Пассивный узел или открывающий активную способность.</param>
/// <param name="Bonuses">Бонусы, применяемые за каждый ранг узла.</param>
public sealed record TalentNodeDefinition(
    string Id,
    string Name,
    string Description,
    string BranchId,
    int Tier,
    int MaxRank,
    TalentNodeType NodeType,
    IReadOnlyList<TalentBonus> Bonuses)
{
    /// <summary>
    /// Уровень персонажа, с которого можно учить узел.
    /// Уровень узла × 2: первая ступень — со второго уровня.
    /// </summary>
    public int RequiredLevel => Tier * 2;
}

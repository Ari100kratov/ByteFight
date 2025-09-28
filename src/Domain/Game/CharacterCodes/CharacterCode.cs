namespace Domain.Game.CharacterCodes;

public class CharacterCode
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CodeLanguage Language { get; set; }
    public string? SourceCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Guid CharacterId { get; set; }
}

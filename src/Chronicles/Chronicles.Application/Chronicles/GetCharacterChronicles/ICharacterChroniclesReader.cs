using SharedKernel;

namespace Chronicles.Application.Chronicles.GetCharacterChronicles;

/// <summary>
/// Читает агрегированную хронику персонажа.
/// </summary>
public interface ICharacterChroniclesReader
{
    /// <summary>
    /// Возвращает хронику персонажа по его идентификатору.
    /// </summary>
    Task<Result<CharacterChroniclesResponse>> GetAsync(Guid characterId, CancellationToken cancellationToken);
}

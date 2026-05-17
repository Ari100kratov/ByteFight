using Shouldly;
using Xunit;

namespace Chronicles.Domain.UnitTests;

/// <summary>
/// Проверяет накопление агрегированной статистики персонажа для хроник.
/// </summary>
public sealed class CharacterChronicleStatsTests
{
    [Fact]
    public void ApplySession_ShouldAccumulateResultCountersAndLastSession()
    {
        var characterId = Guid.CreateVersion7();
        DateTime firstSessionEndedAt = new(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime secondSessionEndedAt = firstSessionEndedAt.AddMinutes(5);
        DateTime thirdSessionEndedAt = firstSessionEndedAt.AddMinutes(10);

        var stats = CharacterChronicleStats.Create(characterId, "Старое имя");

        stats.ApplySession("Первое имя", firstSessionEndedAt, totalTurns: 4, isVictory: true, isDraw: false);
        stats.ApplySession("Второе имя", secondSessionEndedAt, totalTurns: 7, isVictory: false, isDraw: true);
        stats.ApplySession("Новое имя", thirdSessionEndedAt, totalTurns: 9, isVictory: false, isDraw: false);

        stats.CharacterId.ShouldBe(characterId);
        stats.CharacterName.ShouldBe("Новое имя");
        stats.BattlesPlayed.ShouldBe(3);
        stats.Victories.ShouldBe(1);
        stats.Draws.ShouldBe(1);
        stats.Defeats.ShouldBe(1);
        stats.TotalTurnsPlayed.ShouldBe(20);
        stats.LastSessionAtUtc.ShouldBe(thirdSessionEndedAt);
    }
}

using Chronicles.Domain;
using Shouldly;
using Xunit;

namespace UnitTests.Chronicles;

public sealed class ChronicleNominationTests
{
    [Fact]
    public void CharacterNominationScore_ShouldAccumulateIncrementalValues()
    {
        var characterId = Guid.CreateVersion7();
        var sessionId = Guid.CreateVersion7();
        DateTime now = new(2026, 5, 15, 12, 0, 0, DateTimeKind.Utc);

        var score = CharacterNominationScore.Create(
            characterId,
            "Шустрый кабан",
            ChronicleNominationType.MostDamageDealt,
            now);

        score.Add(10.5m, "Шустрый кабан", "Иван", "Петров", "Воин", "Танк", sessionId, now, now);
        score.Add(2.25m, "Шустрый кабан", sessionId, now, now);

        score.Value.ShouldBe(12.75m);
        score.UserFirstName.ShouldBe("Иван");
        score.UserLastName.ShouldBe("Петров");
        score.CharacterClassName.ShouldBe("Воин");
        score.CharacterSpecName.ShouldBe("Танк");
        score.SessionId.ShouldBe(sessionId);
        score.OccurredAtUtc.ShouldBe(now);
    }

    [Fact]
    public void CharacterNominationScore_ShouldKeepLowerRecordForAscendingNomination()
    {
        var characterId = Guid.CreateVersion7();
        DateTime now = new(2026, 5, 15, 12, 0, 0, DateTimeKind.Utc);
        var slowSessionId = Guid.CreateVersion7();
        var fastSessionId = Guid.CreateVersion7();

        var score = CharacterNominationScore.Create(
            characterId,
            "Скоростной гриб",
            ChronicleNominationType.FastestVictory,
            now);

        score.ReplaceIfLower(8, "Скоростной гриб", slowSessionId, now, now);
        score.ReplaceIfLower(12, "Скоростной гриб", Guid.CreateVersion7(), now, now);
        score.ReplaceIfLower(5, "Скоростной гриб", fastSessionId, now, now);

        score.Value.ShouldBe(5);
        score.SessionId.ShouldBe(fastSessionId);
    }
}

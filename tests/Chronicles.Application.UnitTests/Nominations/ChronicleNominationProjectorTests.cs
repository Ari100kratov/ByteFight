using Chronicles.Application.Abstractions.Data;
using Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;
using Chronicles.Domain;
using Shouldly;
using Xunit;

namespace Chronicles.Application.UnitTests.Nominations;

/// <summary>
/// Проверяет стратегии расчёта номинаций по завершённым игровым сессиям.
/// </summary>
public sealed class ChronicleNominationProjectorTests
{
    private static readonly DateTime StartedAtUtc = new(2026, 5, 16, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime EndedAtUtc = new(2026, 5, 16, 10, 5, 0, DateTimeKind.Utc);

    [Fact]
    public void OutcomeNominationProjector_ShouldProjectVictoryAndDefeatOnlyForPlayers()
    {
        CompletedGameSessionParticipantData winner = CreatePlayer("Победитель");
        CompletedGameSessionParticipantData loser = CreatePlayer("Проигравший");
        CompletedGameSessionParticipantData npc = CreateNpc("Манекен");
        CompletedGameSessionData session = CreateSession(
            CompletedGameSessionOutcome.Victory,
            winner.UnitId,
            [winner, loser, npc]);

        var projections = new OutcomeNominationProjector()
            .Project(session, [winner, loser])
            .ToList();

        projections.Count.ShouldBe(4);
        projections.ShouldContain(x => x.CharacterId == winner.UnitId && x.NominationType == ChronicleNominationType.MostBattlesPlayed);
        projections.ShouldContain(x => x.CharacterId == winner.UnitId && x.NominationType == ChronicleNominationType.MostVictories);
        projections.ShouldContain(x => x.CharacterId == loser.UnitId && x.NominationType == ChronicleNominationType.MostBattlesPlayed);
        projections.ShouldContain(x => x.CharacterId == loser.UnitId && x.NominationType == ChronicleNominationType.MostDefeats);
        projections.ShouldNotContain(x => x.CharacterId == npc.UnitId);
    }

    [Fact]
    public void OutcomeNominationProjector_ShouldProjectDrawForEveryPlayerWhenSessionIsDraw()
    {
        CompletedGameSessionParticipantData first = CreatePlayer("Первый");
        CompletedGameSessionParticipantData second = CreatePlayer("Второй");
        CompletedGameSessionData session = CreateSession(
            CompletedGameSessionOutcome.Draw,
            winnerUnitId: null,
            [first, second]);

        IReadOnlyList<ChronicleNominationProjection> projections = new OutcomeNominationProjector()
            .Project(session, [first, second])
            .ToList();

        projections.Count(x => x.NominationType == ChronicleNominationType.MostBattlesPlayed).ShouldBe(2);
        projections.Count(x => x.NominationType == ChronicleNominationType.MostDraws).ShouldBe(2);
        projections.ShouldNotContain(x => x.NominationType == ChronicleNominationType.MostVictories);
        projections.ShouldNotContain(x => x.NominationType == ChronicleNominationType.MostDefeats);
    }

    [Fact]
    public void FastestVictoryProjector_ShouldProjectOnlyWhenWinnerIsPlayer()
    {
        CompletedGameSessionParticipantData player = CreatePlayer("Игрок");
        CompletedGameSessionParticipantData npc = CreateNpc("Босс");
        CompletedGameSessionData playerWonSession = CreateSession(
            CompletedGameSessionOutcome.Victory,
            player.UnitId,
            [player, npc],
            totalTurns: 3);
        CompletedGameSessionData npcWonSession = CreateSession(
            CompletedGameSessionOutcome.Defeat,
            npc.UnitId,
            [player, npc],
            totalTurns: 2);

        var projector = new FastestVictoryProjector();

        ChronicleNominationProjection playerProjection = projector.Project(playerWonSession, [player]).Single();
        IReadOnlyList<ChronicleNominationProjection> npcWinProjections = projector.Project(npcWonSession, [player]).ToList();

        playerProjection.NominationType.ShouldBe(ChronicleNominationType.FastestVictory);
        playerProjection.CharacterId.ShouldBe(player.UnitId);
        playerProjection.Value.ShouldBe(3);
        playerProjection.ScoreUpdateMode.ShouldBe(ChronicleNominationScoreUpdateMode.ReplaceIfLower);
        npcWinProjections.ShouldBeEmpty();
    }

    [Fact]
    public void LongestBattleProjector_ShouldProjectEveryPlayerWithTotalTurns()
    {
        CompletedGameSessionParticipantData first = CreatePlayer("Первый");
        CompletedGameSessionParticipantData second = CreatePlayer("Второй");
        CompletedGameSessionData session = CreateSession(
            CompletedGameSessionOutcome.Draw,
            winnerUnitId: null,
            [first, second],
            totalTurns: 42);

        var projections = new LongestBattleProjector()
            .Project(session, [first, second])
            .ToList();

        projections.Count.ShouldBe(2);
        projections.ShouldAllBe(x => x.NominationType == ChronicleNominationType.LongestBattle);
        projections.ShouldAllBe(x => x.Value == 42);
        projections.ShouldAllBe(x => x.ScoreUpdateMode == ChronicleNominationScoreUpdateMode.ReplaceIfGreater);
        projections.ShouldAllBe(x => x.CreateRecord);
    }

    [Fact]
    public void CombatLogNominationProjector_ShouldProjectCombatAndUtilityMetrics()
    {
        CompletedGameSessionParticipantData actor = CreatePlayer("Автор аргументов");
        CompletedGameSessionParticipantData target = CreatePlayer("Получатель аргументов");
        CompletedGameSessionData session = CreateSession(
            CompletedGameSessionOutcome.Victory,
            actor.UnitId,
            [actor, target],
            logs:
            [
                CreateAbilityLog(actor.UnitId, target.UnitId, "Damage", 40),
                CreateAbilityLog(actor.UnitId, actor.UnitId, "Healing", 10),
                CreateItemLog(actor.UnitId, "HealingPotion", 5),
                CreateItemLog(actor.UnitId, "Gold", 0),
                CreateWalkLog(actor.UnitId),
                CreateWalkLog(actor.UnitId),
                CreateIdleLog(actor.UnitId)
            ]);

        IReadOnlyList<ChronicleNominationProjection> projections = new CombatLogNominationProjector()
            .Project(session, [actor, target])
            .ToList();

        FindProjection(projections, actor.UnitId, ChronicleNominationType.MostDamageDealt).Value.ShouldBe(40);
        FindProjection(projections, actor.UnitId, ChronicleNominationType.MostHealingDone).Value.ShouldBe(15);
        FindProjection(projections, actor.UnitId, ChronicleNominationType.MostMovementActions).Value.ShouldBe(2);
        FindProjection(projections, actor.UnitId, ChronicleNominationType.MostItemsPickedUp).Value.ShouldBe(2);
        FindProjection(projections, actor.UnitId, ChronicleNominationType.MostIdleTurns).Value.ShouldBe(1);
        FindProjection(projections, target.UnitId, ChronicleNominationType.MostDamageTaken).Value.ShouldBe(40);
        projections.ShouldAllBe(x => x.ScoreUpdateMode == ChronicleNominationScoreUpdateMode.Increment);
        projections.ShouldAllBe(x => x.CreateRecord);
    }

    private static ChronicleNominationProjection FindProjection(
        IEnumerable<ChronicleNominationProjection> projections,
        Guid characterId,
        ChronicleNominationType nominationType) =>
        projections.Single(x => x.CharacterId == characterId && x.NominationType == nominationType);

    private static CompletedGameSessionData CreateSession(
        CompletedGameSessionOutcome outcome,
        Guid? winnerUnitId,
        IReadOnlyList<CompletedGameSessionParticipantData> participants,
        int totalTurns = 8,
        IReadOnlyList<CompletedGameSessionLogEntryData>? logs = null) =>
        new(
            Guid.CreateVersion7(),
            totalTurns,
            StartedAtUtc,
            EndedAtUtc,
            outcome,
            winnerUnitId,
            participants,
            logs ?? []);

    private static CompletedGameSessionParticipantData CreatePlayer(string displayName) =>
        new(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            CompletedParticipantUnitType.Player,
            displayName,
            "Иван",
            "Петров",
            "Воин",
            "Танк");

    private static CompletedGameSessionParticipantData CreateNpc(string displayName) =>
        new(
            Guid.CreateVersion7(),
            null,
            CompletedParticipantUnitType.Npc,
            displayName,
            null,
            null,
            null,
            null);

    private static CompletedGameSessionLogEntryData CreateAbilityLog(
        Guid actorId,
        Guid targetId,
        string effectType,
        decimal value) =>
        new(actorId, CompletedGameSessionLogEntryKind.AbilityUsed, "AbilityUsed", 1, targetId, value, effectType, null);

    private static CompletedGameSessionLogEntryData CreateItemLog(Guid actorId, string itemType, decimal value) =>
        new(actorId, CompletedGameSessionLogEntryKind.ItemPickedUp, "ItemPickedUp", 1, null, value, null, itemType);

    private static CompletedGameSessionLogEntryData CreateWalkLog(Guid actorId) =>
        new(actorId, CompletedGameSessionLogEntryKind.Walk, "Walk", 1, null, 0, null, null);

    private static CompletedGameSessionLogEntryData CreateIdleLog(Guid actorId) =>
        new(actorId, CompletedGameSessionLogEntryKind.Idle, "Idle", 1, null, 0, null, null);
}

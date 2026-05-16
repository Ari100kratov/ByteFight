using Chronicles.Application.Abstractions.Data;
using Chronicles.Domain;

namespace Chronicles.Application.Chronicles.ProcessCompletedGameSessions.Nominations;

/// <summary>
/// Рассчитывает номинации, основанные на действиях из боевого журнала.
/// </summary>
internal sealed class CombatLogNominationProjector : IChronicleNominationProjector
{
    private const string DamageEffectType = "Damage";
    private const string HealingEffectType = "Healing";
    private const string HealingPotionItemType = "HealingPotion";

    /// <inheritdoc />
    public IEnumerable<ChronicleNominationProjection> Project(
        CompletedGameSessionData session,
        IReadOnlyList<CompletedGameSessionParticipantData> playerParticipants)
    {
        if (playerParticipants.Count == 0 || session.Logs.Count == 0)
        {
            yield break;
        }

        foreach (CompletedGameSessionParticipantData participant in playerParticipants)
        {
            decimal damageDealt = session.Logs
                .Where(x =>
                    x.ActorId == participant.UnitId &&
                    x.Kind == CompletedGameSessionLogEntryKind.AbilityUsed &&
                    string.Equals(x.EffectType, DamageEffectType, StringComparison.Ordinal))
                .Sum(x => x.Value);

            if (damageDealt > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostDamageDealt,
                    participant,
                    session,
                    damageDealt);
            }

            decimal healingDone = session.Logs
                .Where(x =>
                    x.ActorId == participant.UnitId &&
                    x.Kind == CompletedGameSessionLogEntryKind.AbilityUsed &&
                    string.Equals(x.EffectType, HealingEffectType, StringComparison.Ordinal))
                .Sum(x => x.Value);

            healingDone += session.Logs
                .Where(x =>
                    x.ActorId == participant.UnitId &&
                    x.Kind == CompletedGameSessionLogEntryKind.ItemPickedUp &&
                    string.Equals(x.ItemType, HealingPotionItemType, StringComparison.Ordinal))
                .Sum(x => x.Value);

            if (healingDone > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostHealingDone,
                    participant,
                    session,
                    healingDone);
            }

            int movementActions = session.Logs.Count(x =>
                x.ActorId == participant.UnitId &&
                x.Kind == CompletedGameSessionLogEntryKind.Walk);

            if (movementActions > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostMovementActions,
                    participant,
                    session,
                    movementActions);
            }

            int pickedItems = session.Logs.Count(x =>
                x.ActorId == participant.UnitId &&
                x.Kind == CompletedGameSessionLogEntryKind.ItemPickedUp);

            if (pickedItems > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostItemsPickedUp,
                    participant,
                    session,
                    pickedItems);
            }

            int idleTurns = session.Logs.Count(x =>
                x.ActorId == participant.UnitId &&
                x.Kind == CompletedGameSessionLogEntryKind.Idle);

            if (idleTurns > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostIdleTurns,
                    participant,
                    session,
                    idleTurns);
            }

            decimal damageTaken = session.Logs
                .Where(x =>
                    x.TargetId == participant.UnitId &&
                    x.Kind == CompletedGameSessionLogEntryKind.AbilityUsed &&
                    string.Equals(x.EffectType, DamageEffectType, StringComparison.Ordinal))
                .Sum(x => x.Value);

            if (damageTaken > 0)
            {
                yield return CreateIncrement(
                    ChronicleNominationType.MostDamageTaken,
                    participant,
                    session,
                    damageTaken);
            }
        }
    }

    private static ChronicleNominationProjection CreateIncrement(
        ChronicleNominationType nominationType,
        CompletedGameSessionParticipantData participant,
        CompletedGameSessionData session,
        decimal value) =>
        ChronicleNominationProjection.Increment(
            nominationType,
            participant,
            value,
            session.SessionId,
            session.EndedAtUtc,
            createRecord: true);
}

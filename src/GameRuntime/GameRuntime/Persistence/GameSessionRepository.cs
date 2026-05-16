using Application.Abstractions.Data;
using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameActionLogs.Entries;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using GameRuntime.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace GameRuntime.Persistence;

internal sealed class GameSessionRepository(
    IServiceScopeFactory scopeFactory,
    IDateTimeProvider dateTimeProvider)
    : IGameSessionRepository
{
    public async Task<GameSession> Create(
        Guid id,
        GameInitModel initModel,
        string characterName,
        IEnumerable<GameSessionParticipantInitModel> arenaEnemies,
        CancellationToken ct)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameRuntimeDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameRuntimeDbContext>();
        IGameDbContext gameDbContext = scope.ServiceProvider.GetRequiredService<IGameDbContext>();
        IAuthDbContext authDbContext = scope.ServiceProvider.GetRequiredService<IAuthDbContext>();

        PlayerParticipantMetadata playerMetadata = await LoadPlayerMetadataAsync(
            initModel,
            gameDbContext,
            authDbContext,
            ct);

        var gameSession = GameSession.New(
            id,
            initModel.Mode,
            initModel.ArenaId,
            initModel.CharacterId,
            characterName,
            initModel.UserId,
            playerMetadata.UserFirstName,
            playerMetadata.UserLastName,
            playerMetadata.CharacterClassName,
            playerMetadata.CharacterSpecName,
            arenaEnemies.Select(x => (x.UnitId, x.Name)),
            dateTimeProvider);

        dbContext.GameSessions.Add(gameSession);
        await dbContext.SaveChangesAsync(ct);

        return gameSession;
    }

    private static async Task<PlayerParticipantMetadata> LoadPlayerMetadataAsync(
        GameInitModel initModel,
        IGameDbContext gameDbContext,
        IAuthDbContext authDbContext,
        CancellationToken cancellationToken)
    {
        var character = await gameDbContext.Characters
            .AsNoTracking()
            .Where(x => x.Id == initModel.CharacterId)
            .Select(x => new
            {
                ClassName = x.Spec.Class.Name,
                SpecName = x.Spec.Name
            })
            .SingleOrDefaultAsync(cancellationToken);

        var user = await authDbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == initModel.UserId)
            .Select(x => new
            {
                x.FirstName,
                x.LastName
            })
            .SingleOrDefaultAsync(cancellationToken);

        return new PlayerParticipantMetadata(
            user?.FirstName,
            user?.LastName,
            character?.ClassName,
            character?.SpecName);
    }

    public async Task Save(IEnumerable<GameActionLogEntry> gameActionLogEntries)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameRuntimeDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameRuntimeDbContext>();

        await dbContext.GameActionLogEntries.AddRangeAsync(gameActionLogEntries);
        await dbContext.SaveChangesAsync();
    }

    public async Task<GameSession> MarkStarted(Guid id)
        => await Update(id, static (session, provider) => session.Start(provider), false);

    public async Task<GameSession> CompleteSuccess(Guid id, GameResult gameResult, int turns)
        => await Update(id, (session, provider) => session.CompleteSuccess(gameResult, turns, provider), true);

    public async Task<GameSession> CompleteWithError(Guid id, string? reason, int turns)
        => await Update(id, (session, provider) => session.Fail(reason, turns, provider), false);

    public async Task<GameSession> Abort(Guid id, int turns)
        => await Update(id, (session, provider) => session.Abort(turns, provider), false);

    private async Task<GameSession> Update(
        Guid id,
        Action<GameSession, IDateTimeProvider> update,
        bool shouldEnqueueCompletedEvent)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameRuntimeDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameRuntimeDbContext>();
        IGameSessionCompletedOutboxWriter outboxWriter = scope.ServiceProvider.GetRequiredService<IGameSessionCompletedOutboxWriter>();

        GameSession session = await dbContext.GameSessions
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new DomainException("SESSION_NOT_FOUND", "Game session not found.");

        bool wasOverBeforeUpdate = session.IsOver;

        update(session, dateTimeProvider);

        if (shouldEnqueueCompletedEvent && !wasOverBeforeUpdate && session.Status == GameStatus.Completed && session.Result is not null)
        {
            await outboxWriter.EnqueueAsync(session, CancellationToken.None);
        }

        await dbContext.SaveChangesAsync();
        return session;
    }
}

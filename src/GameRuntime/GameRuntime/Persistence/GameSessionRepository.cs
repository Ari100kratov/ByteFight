using Application.Abstractions.Data;
using Application.Abstractions.GameRuntime;
using Domain.GameRuntime.GameResults;
using Domain.GameRuntime.GameSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace GameRuntime.Persistence;

internal sealed class GameSessionRepository(IServiceScopeFactory scopeFactory, IDateTimeProvider dateTimeProvider)
    : IGameSessionRepository
{
    public async Task<GameSession> Create(
        GameInitModel initModel,
        IEnumerable<Guid> arenaEnemyIds,
        CancellationToken ct)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameRuntimeDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameRuntimeDbContext>();

        var gameSession = GameSession.New(
            initModel.Mode,
            initModel.ArenaId,
            initModel.CharacterId,
            initModel.UserId,
            arenaEnemyIds,
            dateTimeProvider);

        dbContext.GameSessions.Add(gameSession);
        await dbContext.SaveChangesAsync(ct);

        return gameSession;
    }

    public async Task<GameSession> MarkStarted(Guid id)
        => await Update(id, s => s.Start(dateTimeProvider));

    public async Task<GameSession> CompleteSuccess(Guid id, GameResult gameResult, int turns)
        => await Update(id, s => s.CompleteSuccess(gameResult, turns, dateTimeProvider));

    public async Task<GameSession> CompleteWithError(Guid id, string? reason, int turns)
        => await Update(id, s => s.Fail(reason, turns, dateTimeProvider));

    public async Task<GameSession> Abort(Guid id, int turns)
        => await Update(id, s => s.Abort(turns, dateTimeProvider));

    private async Task<GameSession> Update(Guid id, Action<GameSession> update)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IGameRuntimeDbContext dbContext = scope.ServiceProvider.GetRequiredService<IGameRuntimeDbContext>();

        GameSession session = await dbContext.GameSessions
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new DomainException("SESSION_NOT_FOUND", "Game session not found.");

        update(session);

        await dbContext.SaveChangesAsync();
        return session;
    }
}

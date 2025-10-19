using Application.Abstractions.Messaging;

namespace Application.Game.GameModes.Get;

public sealed record GetGameModesQuery : IQuery<IReadOnlyList<GameModeResponse>>;

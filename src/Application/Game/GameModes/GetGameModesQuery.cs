using Application.Abstractions.Messaging;

namespace Application.Game.GameModes;

public sealed record GetGameModesQuery : IQuery<IReadOnlyList<GameModeResponse>>;

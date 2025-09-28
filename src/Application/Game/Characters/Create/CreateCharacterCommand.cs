using Application.Abstractions.Messaging;

namespace Application.Game.Characters.Create;

public record CreateCharacterCommand(string Name) : ICommand<Guid>;

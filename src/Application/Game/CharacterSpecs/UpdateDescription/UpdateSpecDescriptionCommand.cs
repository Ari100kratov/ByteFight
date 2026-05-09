using Application.Abstractions.Messaging;

namespace Application.Game.CharacterSpecs.UpdateDescription;

public sealed record UpdateSpecDescriptionCommand(Guid Id, string? Description) : ICommand;

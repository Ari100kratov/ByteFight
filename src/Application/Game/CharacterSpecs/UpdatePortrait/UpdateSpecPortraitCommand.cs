using Application.Abstractions.Messaging;

namespace Application.Game.CharacterSpecs.UpdatePortrait;

public sealed record UpdateSpecPortraitCommand(Guid Id, Uri PortraitUrl) : ICommand;

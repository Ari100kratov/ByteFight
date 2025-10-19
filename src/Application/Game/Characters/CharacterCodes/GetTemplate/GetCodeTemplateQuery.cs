using Application.Abstractions.Messaging;

namespace Application.Game.Characters.CharacterCodes.GetTemplate;

public sealed record GetCodeTemplateQuery : IQuery<CodeTemplateResponse>;

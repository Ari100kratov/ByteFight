using Application.Abstractions.Messaging;

namespace Application.Game.CharacterCodes.GetTemplate;

public sealed record GetCodeTemplateQuery : IQuery<CodeTemplateResponse>;

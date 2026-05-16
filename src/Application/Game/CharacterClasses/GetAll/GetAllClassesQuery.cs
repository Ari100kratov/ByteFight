
using SharedKernel.Messaging;

namespace Application.Game.CharacterClasses.GetAll;

public sealed record GetAllClassesQuery() : IQuery<IReadOnlyList<ClassResponse>>;

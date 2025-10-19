using SharedKernel;

namespace Domain.Game.Arenas;

public static class ArenaErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Arena.NotFound", $"����� � Id = {id} �� �������.");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Arenas.NameNotUnique",
        "�������� ����� ��� ������");
}

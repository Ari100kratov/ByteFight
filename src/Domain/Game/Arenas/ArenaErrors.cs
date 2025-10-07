using SharedKernel;

namespace Domain.Game.Arenas;

public static class ArenaErrors
{
    public static Error NotFound(Guid arenaId) => Error.NotFound(
        "Arenas.NotFound",
        $"����� � Id = '{arenaId}' �� �������");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Arenas.NameNotUnique",
        "�������� ����� ��� ������");
}

using SharedKernel;

namespace Domain.Game.Characters;

public static class CharacterErrors
{
    public static Error NotFound(Guid characterId) => Error.NotFound(
        "Characters.NotFound",
        $"�������� � Id = '{characterId}' �� ������");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Characters.NameNotUnique",
        "��� ��������� ��� ������");

    public static Error Unauthorized() => Error.Failure(
        "Characters.Unauthorized",
        "�� �� ������������ ��� ���������� ������� ��������");
}

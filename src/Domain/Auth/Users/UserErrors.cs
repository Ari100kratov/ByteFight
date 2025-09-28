using SharedKernel;

namespace Domain.Auth.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"Пользователь с Id = '{userId}' не найден");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "Вы не авторизованы для выполнения данного действия.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "Пользователь с указанным email не найден");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "Указанный email уже используется");
}

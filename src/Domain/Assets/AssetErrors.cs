using SharedKernel;

namespace Domain.Assets;

public static class AssetErrors
{
    public static Error NotFound(string bucket, string objectName) => Error.NotFound(
        "Assets.NotFound",
        $"Файл '{objectName}' не найден в бакете '{bucket}'.");

    public static readonly Error InvalidKeyFormat = Error.Problem(
        "Assets.InvalidKeyFormat",
        "Некорректный формат ключа ассета. Ожидается 'bucket/objectName'.");

    public static readonly Error KeyRequired = Error.Problem(
        "Assets.KeyRequired",
        "Ключ ассета обязателен.");

    public static Error ReadFailed(string reason) => Error.Failure(
        "Assets.ReadFailed",
        $"Не удалось получить файл из хранилища: {reason}");

    public static readonly Error Unauthorized = Error.Failure(
        "Assets.Unauthorized",
        "Неверные учётные данные для доступа к хранилищу.");
}

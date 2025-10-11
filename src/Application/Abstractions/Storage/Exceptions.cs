namespace Application.Abstractions.Storage;

public abstract class StorageException : Exception
{
    protected StorageException(string message) : base(message) { }
}

public sealed class StorageBucketNotFoundException(string bucket)
    : StorageException($"Бакет '{bucket}' не найден");

public sealed class StorageObjectNotFoundException(string bucket, string key)
    : StorageException($"Файл '{key}' не найден в бакете '{bucket}'");

public sealed class StorageAuthorizationException()
    : StorageException("Ошибка авторизации при доступе к хранилищу");

public sealed class StorageInvalidNameException(string name)
    : StorageException($"Некорректное имя: {name}");

public sealed class StorageReadException(string reason)
    : StorageException($"Ошибка при чтении из хранилища: {reason}");

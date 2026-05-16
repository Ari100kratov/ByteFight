namespace Application.Auth.Users.GetCurrent;

/// <summary>
/// Данные текущего пользователя, необходимые клиентскому приложению.
/// </summary>
public sealed record UserResponse
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Роли пользователя.
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = [];
}

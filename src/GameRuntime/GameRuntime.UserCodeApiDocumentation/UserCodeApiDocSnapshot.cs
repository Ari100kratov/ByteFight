namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Хранилище уже собранной документации пользовательского API.
/// </summary>
public sealed class UserCodeApiDocSnapshot
{
    /// <summary>
    /// Текущая документация пользовательского API.
    /// </summary>
    public UserCodeApiDoc Value
    {
        get =>
        field ?? throw new InvalidOperationException("User code API documentation has not been initialized."); private set;
    }

    /// <summary>
    /// Сохраняет документацию пользовательского API.
    /// </summary>
    public void Set(UserCodeApiDoc doc)
    {
        Value = doc;
    }
}

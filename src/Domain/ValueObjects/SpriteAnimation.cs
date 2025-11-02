namespace Domain.ValueObjects;

public sealed record SpriteAnimation
{
    /// <summary>
    /// Относительный путь к файлу ассета (например, URL спрайтшита).
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Количество кадров в анимации (для разбиения спрайтшита).
    /// </summary>
    public int FrameCount { get; init; }

    /// <summary>
    /// Скорость воспроизведения анимации.
    /// Значение 1 — нормальная скорость, меньше 1 — медленнее, больше 1 — быстрее.
    /// </summary>
    public float AnimationSpeed { get; init; }

    /// <summary>
    /// Масштаб
    /// </summary>
    public Scale Scale { get; init; }

    private SpriteAnimation() { } // Для EF

    public SpriteAnimation(Uri url, int frameCount, float animationSpeed, float scaleX = 1f, float scaleY = 1f)
    {
        string urlString = url.ToString().Trim();
        if (string.IsNullOrWhiteSpace(urlString))
        {
            throw new ArgumentException("Url не может быть пустым.");
        }

        if (urlString.Length > 256)
        {
            throw new ArgumentException("Url не может превышать 256 символов");
        }

        if (frameCount <= 0)
        {
            throw new ArgumentException("FrameCount должен быть больше 0.");
        }

        if (animationSpeed <= 0)
        {
            throw new ArgumentException("AnimationSpeed должен быть > 0.");
        }

        Url = urlString;
        FrameCount = frameCount;
        AnimationSpeed = animationSpeed;
        Scale = new Scale(scaleX, scaleY);
    }
}

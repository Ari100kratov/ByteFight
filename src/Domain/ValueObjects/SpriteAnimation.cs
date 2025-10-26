namespace Domain.ValueObjects;

public sealed record SpriteAnimation
{
    private const float Epsilon = 0.0001f;

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
    /// Масштаб по оси X.
    /// 1.0 — смотрит право, -1.0 — смотрит влево,
    /// меньше 1.0 — уменьшение, больше 1.0 — увеличение.
    /// </summary>
    public float ScaleX { get; init; }

    /// <summary>
    /// Масштаб по оси Y.
    /// 1.0 — нормальное положение, -1.0 — отзеркален по вертикали,
    /// меньше 1.0 — уменьшение, больше 1.0 — увеличение.
    /// </summary>
    public float ScaleY { get; init; }

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

        if (Math.Abs(scaleX) < Epsilon)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleX), "ScaleX не может быть 0");
        }

        if (Math.Abs(scaleY) < Epsilon)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleY), "ScaleY не может быть 0");
        }

        Url = urlString;
        FrameCount = frameCount;
        AnimationSpeed = animationSpeed;
        ScaleX = scaleX;
        ScaleY = scaleY;
    }
}

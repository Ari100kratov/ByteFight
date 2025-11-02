namespace Domain.ValueObjects;

public sealed record Scale
{
    private const float Epsilon = 0.0001f;

    /// <summary>
    /// Масштаб по оси X.
    /// 1.0 — смотрит право, -1.0 — смотрит влево,
    /// меньше 1.0 — уменьшение, больше 1.0 — увеличение.
    /// </summary>
    public float X { get; init; }

    /// <summary>
    /// Масштаб по оси Y.
    /// 1.0 — нормальное положение, -1.0 — отзеркален по вертикали,
    /// меньше 1.0 — уменьшение, больше 1.0 — увеличение.
    /// </summary>
    public float Y { get; init; }

    private Scale() { } // Для EF

    public Scale(float scaleX, float scaleY)
    {
        if (Math.Abs(scaleX) < Epsilon)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleX), "ScaleX не может быть 0");
        }

        if (Math.Abs(scaleY) < Epsilon)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleY), "ScaleY не может быть 0");
        }

        X = scaleX;
        Y = scaleY;
    }
}

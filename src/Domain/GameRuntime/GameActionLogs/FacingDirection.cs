namespace Domain.GameRuntime.GameActionLogs;

/// <summary>
/// Направление взгляда юнита на гексагональном поле.
/// Восемь направлений соответствуют набору спрайтов персонажа,
/// что даёт плавный поворот при перемещении и применении способностей.
/// </summary>
public enum FacingDirection
{
    /// <summary>Восток (вправо).</summary>
    Right = 1,

    /// <summary>Юго-восток.</summary>
    DownRight = 2,

    /// <summary>Юг (вниз).</summary>
    Down = 3,

    /// <summary>Юго-запад.</summary>
    DownLeft = 4,

    /// <summary>Запад (влево).</summary>
    Left = 5,

    /// <summary>Северо-запад.</summary>
    UpLeft = 6,

    /// <summary>Север (вверх).</summary>
    Up = 7,

    /// <summary>Юго-восток (вверх-вправо).</summary>
    UpRight = 8
}

/// <summary>
/// Вспомогательные операции над направлениями взгляда.
/// </summary>
public static class FacingDirectionExtensions
{
    /// <summary>
    /// Приводит угол в градусах (экранная система координат: 0° — восток,
    /// положительное направление — по часовой стрелке) к ближайшему
    /// из восьми направлений взгляда.
    /// </summary>
    public static FacingDirection FromAngleDegrees(double degrees)
    {
        // Нормализуем в диапазон [0, 360).
        degrees = (degrees % 360 + 360) % 360;

        // Каждый сектор — 45°, границы проходят через 22.5°, 67.5° и т.д.
        int sector = (int)Math.Round(degrees / 45, MidpointRounding.AwayFromZero) % 8;

        return sector switch
        {
            0 => FacingDirection.Right,
            1 => FacingDirection.DownRight,
            2 => FacingDirection.Down,
            3 => FacingDirection.DownLeft,
            4 => FacingDirection.Left,
            5 => FacingDirection.UpLeft,
            6 => FacingDirection.Up,
            _ => FacingDirection.UpRight
        };
    }

    /// <summary>
    /// Возвращает противоположное направление.
    /// </summary>
    public static FacingDirection Opposite(this FacingDirection direction) => direction switch
    {
        FacingDirection.Right => FacingDirection.Left,
        FacingDirection.DownRight => FacingDirection.UpLeft,
        FacingDirection.Down => FacingDirection.Up,
        FacingDirection.DownLeft => FacingDirection.UpRight,
        FacingDirection.Left => FacingDirection.Right,
        FacingDirection.UpLeft => FacingDirection.DownRight,
        FacingDirection.Up => FacingDirection.Down,
        _ => FacingDirection.DownLeft
    };

    /// <summary>
    /// Указывает, относится ли направление к правой половине экрана
    /// (включая строго вертикальные — они рисуются в фас).
    /// Клиент рисует правосторонний набор спрайтов, левый получается зеркалом.
    /// </summary>
    public static bool IsRightSide(this FacingDirection direction) => direction switch
    {
        FacingDirection.Right => true,
        FacingDirection.DownRight => true,
        FacingDirection.UpRight => true,
        FacingDirection.Down => true,
        FacingDirection.Up => true,
        _ => false
    };
}

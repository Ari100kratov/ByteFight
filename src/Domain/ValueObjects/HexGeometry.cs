using Domain.GameRuntime.GameActionLogs;

namespace Domain.ValueObjects;

/// <summary>
/// Геометрия гексагонального поля с плоской вершиной (flat-top),
/// хранение в смещённых координатах «odd-q»: X — колонка, Y — строка,
 /// нечётные колонки сдвинуты на полгекса вниз.
///
/// Все арены игры используют эту раскладку: и серверная логика
/// (дальность, соседи, области действия), и отрисовка на клиенте.
/// </summary>
public static class HexGeometry
{
    /// <summary>Смещения шести соседей для чётных колонок.</summary>
    private static readonly (int Dx, int Dy)[] EvenColumnNeighbors =
    [
        (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (0, 1)
    ];

    /// <summary>Смещения шести соседей для нечётных колонок.</summary>
    private static readonly (int Dx, int Dy)[] OddColumnNeighbors =
    [
        (1, 1), (1, 0), (0, -1), (-1, 0), (-1, 1), (0, 1)
    ];

    /// <summary>
    /// Переводит смещённые координаты в кубические
    /// (x + y + z = 0), удобные для расчёта расстояний.
    /// </summary>
    public static (int X, int Y, int Z) ToCube(Position position)
    {
        int x = position.X;
        int z = position.Y - (position.X - (position.X & 1)) / 2;
        return (x, -x - z, z);
    }

    /// <summary>
    /// Расстояние между гексами в гексах (минимальное число шагов).
    /// </summary>
    public static int Distance(Position from, Position to)
    {
        (int x1, int y1, int z1) = ToCube(from);
        (int x2, int y2, int z2) = ToCube(to);

        return (Math.Abs(x1 - x2) + Math.Abs(y1 - y2) + Math.Abs(z1 - z2)) / 2;
    }

    /// <summary>
    /// Возвращает координаты шести соседних гексов.
    /// Гексы с отрицательными координатами пропускаются:
    /// позиция не может быть отрицательной, поэтому такие соседи
    /// заведомо вне арены.
    /// </summary>
    public static IEnumerable<Position> GetNeighbors(Position position)
    {
        (int dx, int dy)[] offsets = (position.X & 1) == 0
            ? EvenColumnNeighbors
            : OddColumnNeighbors;

        foreach ((int dx, int dy) in offsets)
        {
            int x = position.X + dx;
            int y = position.Y + dy;

            if (x >= 0 && y >= 0)
            {
                yield return new Position(x, y);
            }
        }
    }

    /// <summary>
    /// Возвращает все гексы в радиусе от центра включительно.
    /// Радиус 0 — сам центр. Порядок обхода — от ближних к дальним.
    /// </summary>
    public static IEnumerable<Position> GetCellsInRange(Position center, int radius)
    {
        if (radius < 0)
        {
            yield break;
        }

        for (int r = 0; r <= radius; r++)
        {
            foreach (Position cell in GetRing(center, r))
            {
                yield return cell;
            }
        }
    }

    /// <summary>
    /// Возвращает кольцо гексов на заданном расстоянии от центра.
    /// </summary>
    public static IEnumerable<Position> GetRing(Position center, int radius)
    {
        if (radius < 0)
        {
            yield break;
        }

        if (radius == 0)
        {
            yield return center;
            yield break;
        }

        // Начинаем с соседа в направлении (0, +1) и обходим кольцо,
        // поворачивая кубические координаты по шести направлениям.
        (int x, int y, int z) = ToCube(center);
        x += radius;
        z += 0;
        y -= radius;

        foreach ((int dx, int dy, int dz) in CubeDiagonalsClockwise)
        {
            for (int step = 0; step < radius; step++)
            {
                int offsetX = x;
                int offsetY = CubeToOffsetZ(x, z);

                // Кольцо может выходить за пределы сетки (например, над
                // первой строкой) — такие гексы пропускаем.
                if (offsetX >= 0 && offsetY >= 0)
                {
                    yield return new Position(offsetX, offsetY);
                }

                x += dx;
                y += dy;
                z += dz;
            }
        }
    }

    private static readonly (int Dx, int Dy, int Dz)[] CubeDiagonalsClockwise =
    [
        (0, +1, -1), (-1, +1, 0), (-1, 0, +1), (0, -1, +1), (+1, -1, 0), (+1, 0, -1)
    ];

    private static int CubeToOffsetZ(int x, int z) => z + (x - (x & 1)) / 2;

    /// <summary>
    /// Рисует прямую гекс-линию между двумя гексами (алгоритм lerp + округление
    /// кубических координат). Обе конечные точки включены.
    /// </summary>
    public static List<Position> GetLine(Position from, Position to)
    {
        if (from == to)
        {
            return [from];
        }

        (int fx, int fy, int fz) = ToCube(from);
        (int tx, int ty, int tz) = ToCube(to);

        int distance = Distance(from, to);

        // Небольшой сдвиг стартовой точки (nudge) предотвращает
        // «сползание» линии ровно посередине между двумя гексами.
        double ax = fx + 1e-6;
        double ay = fy + 1e-6;
        double az = fz - 2e-6;

        var result = new List<Position>(distance + 1) { from };

        for (int i = 1; i <= distance; i++)
        {
            double t = (double)i / distance;

            double x = ax + (tx - ax) * t;
            double y = ay + (ty - ay) * t;
            double z = az + (tz - az) * t;

            (int cx, _, int cz) = CubeRound(x, y, z);

            int offsetZ = cz + (cx - (cx & 1)) / 2;

            // Промежуточный гекс может оказаться за пределами сетки
            // (например, над первой строкой) — такие клетки отбрасываем,
            // вызывающий код всё равно фильтрует гексы по границам арены.
            if (cx >= 0 && offsetZ >= 0)
            {
                result.Add(new Position(cx, offsetZ));
            }
        }

        return result;
    }

    private static (int X, int Y, int Z) CubeRound(double x, double y, double z)
    {
        double rx = Math.Round(x, MidpointRounding.AwayFromZero);
        double ry = Math.Round(y, MidpointRounding.AwayFromZero);
        double rz = Math.Round(z, MidpointRounding.AwayFromZero);

        double dx = Math.Abs(rx - x);
        double dy = Math.Abs(ry - y);
        double dz = Math.Abs(rz - z);

        if (dx > dy && dx > dz)
        {
            rx = -ry - rz;
        }
        else if (dy > dz)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        return ((int)rx, (int)ry, (int)rz);
    }

    /// <summary>
    /// Переводит гекс в пиксельные координаты центра (flat-top, odd-q).
    /// Используется для расчёта направления взгляда и на клиенте.
    /// </summary>
    public static (double X, double Y) ToPixel(Position position, double hexSize)
    {
        double x = hexSize * 1.5 * position.X;
        double y = Math.Sqrt(3) * hexSize * (position.Y + 0.5 * (position.X & 1));

        return (x, y);
    }

    /// <summary>
    /// Вычисляет направление взгляда из одного гекса на другой
    /// с точностью до восьми сторон света.
    /// </summary>
    public static FacingDirection GetFacing(Position from, Position to)
    {
        if (from == to)
        {
            // Направление не меняется — оставляем вызывающему коду.
            return FacingDirection.Right;
        }

        (double x1, double y1) = ToPixel(from, 1);
        (double x2, double y2) = ToPixel(to, 1);

        double angle = Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI;

        return FacingDirectionExtensions.FromAngleDegrees(angle);
    }
}

namespace GridDefense;

public static class GridMap
{
    public const int Width = 20;
    public const int Height = 20;

    public static IReadOnlyList<(int X, int Y)> Path { get; } =
        new (int X, int Y)[]
        {
            (2, 3), (3, 3), (4, 3), (5, 3), (6, 3), (7, 3), (8, 3),
            (8, 4), (8, 5),
            (8, 6), (7, 6), (6, 6), (5, 6), (4, 6), (3, 6), (2, 6),
            (2, 7), (2, 8),
            (2, 9), (3, 9), (4, 9), (5, 9), (6, 9), (7, 9), (8, 9)
        };
}
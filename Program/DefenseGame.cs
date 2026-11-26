using ConsoleEngine;

namespace GridDefense;

public sealed class DefenseGame : GameBase
{
    private readonly Enemy _enemy = new();

    public DefenseGame() : base(GridMap.Width, GridMap.Height + 2)
    {
    }

    protected override void Input(ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
            Quit();
    }

    protected override void Update(float deltaTime)
    {
        _enemy.Update(deltaTime);
    }

    protected override void Render(Screen screen)
    {
        for (int y = 0; y < GridMap.Height; y++)
        {
            for (int x = 0; x < GridMap.Width; x++)
            {
                screen.Draw(x, y, '.');
            }
        }

        foreach (var tile in GridMap.Path)
        {
            screen.Draw(tile.X, tile.Y, '#');
        }

        var start = GridMap.Path[0];
        var goal = GridMap.Path[GridMap.Path.Count - 1];
        screen.Draw(start.X, start.Y, 'S');
        screen.Draw(goal.X, goal.Y, 'G');

        if (!_enemy.ReachedGoal)
        {
            var position = GridMap.Path[_enemy.PathIndex];
            screen.Draw(position.X, position.Y, 'E');
        }

        string status = _enemy.ReachedGoal ? "Enemy reached goal" : "Enemy moving";
        screen.DrawText(0, GridMap.Height, status);
        screen.DrawText(0, GridMap.Height + 1, "Esc: Quit");
    }
}
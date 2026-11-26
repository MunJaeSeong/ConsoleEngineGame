using ConsoleEngine;

namespace GridDefense;

public sealed class DefenseGame : GameBase
{
    private readonly List<Enemy> _enemies = new();
    private readonly List<Unit> _units = new();
    private readonly Random _random = new();

    private int _playerHp = WaveRules.StartingPlayerHp;
    private int _diamonds = 500;
    private int _wave = 1;
    private int _remainingSpawns;
    private float _timeUntilSpawn;
    private int _cursorX;
    private int _cursorY;
    private UnitType _selectedUnit = UnitType.Archer;
    private bool _waveActive;
    private string _message = "Press P to start Wave 1";

    public DefenseGame() : base(GridMap.Width, GridMap.Height + 7)
    {
    }

    protected override void Input(ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
        {
            Quit();
            return;
        }

        if (_playerHp == 0)
        {
            if (key == ConsoleKey.R)
            {
                ResetGame();
            }
            return;
        }

        switch (key)
        {
            case ConsoleKey.LeftArrow: _cursorX = Math.Max(0, _cursorX - 1); break;
            case ConsoleKey.RightArrow: _cursorX = Math.Min(GridMap.Width - 1, _cursorX + 1); break;
            case ConsoleKey.UpArrow: _cursorY = Math.Max(0, _cursorY - 1); break;
            case ConsoleKey.DownArrow: _cursorY = Math.Min(GridMap.Height - 1, _cursorY + 1); break;
            case ConsoleKey.D1: _selectedUnit = UnitType.Archer; break;
            case ConsoleKey.D2: _selectedUnit = UnitType.Sniper; break;
            case ConsoleKey.D3: _selectedUnit = UnitType.Cannon; break;
            case ConsoleKey.D4: _selectedUnit = UnitType.Frost; break;
            case ConsoleKey.D5: _selectedUnit = UnitType.Storm; break;
            case ConsoleKey.D6: _selectedUnit = UnitType.Bomb; break;
            case ConsoleKey.D7: _selectedUnit = UnitType.Volt; break;
            case ConsoleKey.D8: _selectedUnit = UnitType.Toxin; break;
            case ConsoleKey.D9: _selectedUnit = UnitType.Aura; break;
            case ConsoleKey.D0: _selectedUnit = UnitType.Guardian; break;
            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                SummonSelectedUnit();
                break;
            case ConsoleKey.U:
                UpgradeSelectedUnit();
                break;
            case ConsoleKey.S:
                SellSelectedUnit();
                break;
            case ConsoleKey.P:
                if (!_waveActive)
                    StartWave();
                break;
        }
    }

    private void ResetGame()
    {
        _enemies.Clear();
        _units.Clear();
        _playerHp = WaveRules.StartingPlayerHp;
        _diamonds = 500;
        _wave = 1;
        _remainingSpawns = 0;
        _timeUntilSpawn = 0f;
        _cursorX = 0;
        _cursorY = 0;
        _selectedUnit = UnitType.Archer;
        _waveActive = false;
        _message = "Press P to start Wave 1";
    }

    private bool IsBuildable(int x, int y)
    {
        return !GridMap.Path.Contains((x, y));
    }

    private Unit? GetUnitAtCursor()
    {
        return _units.FirstOrDefault(unit => unit.X == _cursorX && unit.Y == _cursorY);
    }

    private void SummonSelectedUnit()
    {
        if (GetUnitAtCursor() != null)
        {
            _message = "Tile occupied";
            return;
        }
        if (!IsBuildable(_cursorX, _cursorY))
        {
            _message = "Cannot build on path";
            return;
        }
        if (_diamonds < 50)
        {
            _message = "Need 50 diamonds";
            return;
        }

        _diamonds -= 50;
        _units.Add(new Unit(_selectedUnit, _cursorX, _cursorY));
        _message = $"Summoned {Unit.GetName(_selectedUnit)}";
    }

    private void UpgradeSelectedUnit()
    {
        Unit? unit = GetUnitAtCursor();
        if (unit == null)
        {
            _message = "No unit to upgrade";
            return;
        }
        if (unit.Level >= 10)
        {
            _message = "Unit is at level 10";
            return;
        }

        int cost = unit.GetUpgradeCost();
        if (_diamonds < cost)
        {
            _message = $"Upgrade needs {cost} diamonds";
            return;
        }

        _diamonds -= cost;
        int successPercent = 100 - unit.Level * 8;
        if (_random.Next(100) < successPercent)
        {
            unit.SpendForUpgrade(cost);
            unit.Upgrade();
            _message = $"Upgrade success! LV {unit.Level} ({successPercent}% chance)";
        }
        else
        {
            unit.SpendForUpgrade(cost);
            _units.Remove(unit);
            _message = $"Upgrade failed! Unit destroyed ({successPercent}% chance)";
        }
    }

    private void SellSelectedUnit()
    {
        Unit? unit = GetUnitAtCursor();
        if (unit == null)
        {
            _message = "No unit to sell";
            return;
        }

        int value = unit.GetSellValue();
        _units.Remove(unit);
        _diamonds += value;
        _message = $"Sold unit for {value} diamonds";
    }

    private void StartWave()
    {
        _remainingSpawns = WaveRules.GetEnemyCount(_wave);
        _timeUntilSpawn = 0f;
        _waveActive = true;
        _message = $"Wave {_wave} started";
    }

    protected override void Update(float deltaTime)
    {
        if (_playerHp == 0)
            return;

        if (_waveActive)
        {
            SpawnEnemies(deltaTime);
            UpdateEnemies(deltaTime);
            UpdateUnits(deltaTime);

            if (_remainingSpawns == 0 && _enemies.Count == 0)
            {
                _waveActive = false;
                int bonus = 25 + _wave * 5;
                _diamonds += bonus;
                _message = $"Wave {_wave} cleared! Bonus {bonus}";
                _wave++;
            }
        }
    }

    private void SpawnEnemies(float deltaTime)
    {
        _timeUntilSpawn -= deltaTime;
        while (_remainingSpawns > 0 && _timeUntilSpawn <= 0f)
        {
            _enemies.Add(new Enemy(_wave, _random));
            _remainingSpawns--;
            _timeUntilSpawn += 0.28f;
        }
    }

    private void UpdateEnemies(float deltaTime)
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _enemies[i];
            enemy.Update(deltaTime);

            if (enemy.Hp <= 0)
            {
                _diamonds += enemy.Reward;
                _enemies.RemoveAt(i);
            }
            else if (enemy.ReachedGoal)
            {
                _enemies.RemoveAt(i);
                _playerHp = Math.Max(0, _playerHp - WaveRules.DamagePerArrival);
                if (_playerHp == 0)
                {
                    _waveActive = false;
                    _message = "GAME OVER - Press R to retry";
                    return;
                }
            }
        }
    }

    private void UpdateUnits(float deltaTime)
    {
        foreach (Unit unit in _units)
        {
            unit.AttackElapsed += deltaTime;
            var (damage, interval, range) = unit.GetStats();

            if (unit.AttackElapsed < interval)
                continue;

            Enemy? target = _enemies
                .Where(enemy => enemy.Hp > 0 && GetDistance(unit.X, unit.Y, enemy) <= range)
                .OrderByDescending(enemy => enemy.PathIndex)
                .FirstOrDefault();

            if (target == null)
                continue;

            unit.AttackElapsed = 0f;

            if (unit.Type == UnitType.Cannon || unit.Type == UnitType.Bomb || unit.Type == UnitType.Storm)
            {
                foreach (Enemy enemy in _enemies.Where(enemy => enemy.Hp > 0 && GetDistance(unit.X, unit.Y, enemy) <= range).ToArray())
                {
                    int splashDamage = unit.Type == UnitType.Bomb ? damage * 2 : damage;
                    enemy.TakeDamage(splashDamage);
                }
            }
            else
            {
                target.TakeDamage(damage);
            }
        }
    }

    private int GetDistance(int x, int y, Enemy enemy)
    {
        var position = GridMap.Path[Math.Min(enemy.PathIndex, GridMap.Path.Count - 1)];
        return Math.Abs(x - position.X) + Math.Abs(y - position.Y);
    }

    protected override void Render(Screen screen)
    {
        for (int y = 0; y < GridMap.Height; y++)
        {
            for (int x = 0; x < GridMap.Width; x++)
                screen.Draw(x, y, '.', ConsoleColor.DarkGray);
        }

        foreach (var tile in GridMap.Path)
            screen.Draw(tile.X, tile.Y, '#', ConsoleColor.Gray);

        var start = GridMap.Path[0];
        var goal = GridMap.Path[^1];
        screen.Draw(start.X, start.Y, 'S', ConsoleColor.Green);
        screen.Draw(goal.X, goal.Y, 'G', ConsoleColor.Blue);

        foreach (Unit unit in _units)
            screen.Draw(unit.X, unit.Y, Unit.GetSymbol(unit.Type), ConsoleColor.Cyan);

        foreach (Enemy enemy in _enemies)
        {
            int index = Math.Min(enemy.PathIndex, GridMap.Path.Count - 1);
            var position = GridMap.Path[index];
            screen.Draw(position.X, position.Y, Enemy.GetSymbol(enemy.Type), ConsoleColor.Red);
        }

        screen.Draw(_cursorX, _cursorY, 'X', ConsoleColor.Yellow);

        ConsoleColor hpColor = _playerHp <= 10 ? ConsoleColor.Red : _playerHp <= 25 ? ConsoleColor.Yellow : ConsoleColor.Green;
        screen.DrawText(0, 20, $"W{_wave}  HP {_playerHp}/{WaveRules.StartingPlayerHp}  Diamonds {_diamonds}", hpColor);
        screen.DrawText(0, 21, $"Selected: {Unit.GetName(_selectedUnit)} (1-0)  Enemies {_enemies.Count}/{(_remainingSpawns == 0 ? 0 : WaveRules.GetEnemyCount(_wave))}", ConsoleColor.White);

        Unit? current = GetUnitAtCursor();
        if (current == null)
            screen.DrawText(0, 22, "Cursor: Empty tile - Enter summon - U upgrade - S sell", ConsoleColor.White);
        else
            screen.DrawText(0, 22, $"{Unit.GetName(current.Type)} LV {current.Level} Sell {current.GetSellValue()} Upgrade {current.GetUpgradeCost()}", ConsoleColor.Cyan);

        screen.DrawText(0, 23, _message, ConsoleColor.Yellow);
        screen.DrawText(0, 24, _waveActive ? "P: Wave running" : "P: Start wave", ConsoleColor.White);
        screen.DrawText(0, 25, "Arrows: move  Enter: summon  U: upgrade  S: sell  Esc: quit", ConsoleColor.White);

        if (_playerHp == 0)
            screen.DrawText(0, 26, "GAME OVER - R: Retry", ConsoleColor.Red);
    }
}
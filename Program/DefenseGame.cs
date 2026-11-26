using ConsoleEngine;

namespace GridDefense;

public sealed class DefenseGame : GameBase
{
    private readonly Random _random = new();
    private readonly List<Enemy> _enemies = new();
    private readonly List<Unit> _units = new();

    private int _diamonds = 500;
    private int _playerHp = 50;
    private int _wave = 1;
    private int _spawned;
    private int _cursorX = 10;
    private int _cursorY = 10;
    private float _spawnElapsed;
    private bool _waveInProgress;
    private bool _gameOver;
    private string _message = "Arrows: move | 1-0: unit | Enter: summon";
    private UnitKind _selectedKind = UnitKind.Archer;

    public DefenseGame() : base(GridMap.Width, GridMap.Height + 8)
    {
    }

    protected override void Input(ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
        {
            Quit();
            return;
        }

        if (_gameOver)
            return;

        if (key == ConsoleKey.LeftArrow) _cursorX = Math.Max(0, _cursorX - 1);
        if (key == ConsoleKey.RightArrow) _cursorX = Math.Min(GridMap.Width - 1, _cursorX + 1);
        if (key == ConsoleKey.UpArrow) _cursorY = Math.Max(0, _cursorY - 1);
        if (key == ConsoleKey.DownArrow) _cursorY = Math.Min(GridMap.Height - 1, _cursorY + 1);

        if (key >= ConsoleKey.D1 && key <= ConsoleKey.D9)
            _selectedKind = (UnitKind)(key - ConsoleKey.D1);
        if (key == ConsoleKey.D0)
            _selectedKind = (UnitKind)9;

        if (key is ConsoleKey.Enter or ConsoleKey.Spacebar)
        {
            if (key == ConsoleKey.Spacebar)
            {
                if (!_waveInProgress)
                    StartWave();
            }
            else
            {
                SummonUnit();
            }
        }

        if (key == ConsoleKey.U) UpgradeUnitAtCursor();
        if (key == ConsoleKey.S) SellUnitAtCursor();
    }

    private void StartWave()
    {
        _waveInProgress = true;
        _spawned = 0;
        _spawnElapsed = 0;
        _enemies.Clear();
        _message = $"Wave {_wave} started";
    }

    private void SummonUnit()
    {
        if (GridMap.Path.Any(p => p.X == _cursorX && p.Y == _cursorY))
        {
            _message = "Cannot build on the monster path";
            return;
        }
        if (_units.Any(u => u.X == _cursorX && u.Y == _cursorY))
        {
            _message = "A unit already occupies this tile";
            return;
        }
        if (_diamonds < 50)
        {
            _message = "Need 50 diamonds";
            return;
        }

        _diamonds -= 50;
        _units.Add(new Unit(_selectedKind, _cursorX, _cursorY));
        _message = $"{UnitSpecs.Get(_selectedKind).Name} summoned";
    }

    private Unit? GetUnitAtCursor()
        => _units.FirstOrDefault(u => u.X == _cursorX && u.Y == _cursorY);

    private void UpgradeUnitAtCursor()
    {
        var unit = GetUnitAtCursor();
        if (unit is null)
        {
            _message = "Move the cursor onto a unit";
            return;
        }
        if (unit.Stage >= 10)
        {
            _message = "Maximum stage reached";
            return;
        }

        int cost = Unit.GetUpgradeCost(unit.Stage);
        if (_diamonds < cost)
        {
            _message = $"Upgrade needs {cost} diamonds";
            return;
        }

        _diamonds -= cost;
        bool success = unit.Upgrade(_random);
        if (success)
            _message = $"Upgrade success: stage {unit.Stage}";
        else
        {
            _units.Remove(unit);
            _message = "Upgrade failed: unit destroyed";
        }
    }

    private void SellUnitAtCursor()
    {
        var unit = GetUnitAtCursor();
        if (unit is null)
        {
            _message = "Move the cursor onto a unit";
            return;
        }

        int value = unit.SellValue();
        _units.Remove(unit);
        _diamonds += value;
        _message = $"Unit sold: +{value} diamonds";
    }

    protected override void Update(float deltaTime)
    {
        if (_gameOver)
            return;

        if (_waveInProgress)
            UpdateSpawning(deltaTime);

        foreach (var enemy in _enemies.Where(e => !e.IsDead).ToList())
        {
            enemy.Update(deltaTime);
            if (enemy.ReachedGoal)
            {
                _playerHp--;
                _enemies.Remove(enemy);
                _message = "An enemy reached the goal: -1 HP";
                if (_playerHp <= 0)
                {
                    _playerHp = 0;
                    _gameOver = true;
                    _waveInProgress = false;
                    _message = "GAME OVER";
                    return;
                }
            }
        }

        UpdateUnits(deltaTime);

        if (_waveInProgress &&
            _spawned >= WaveRules.GetMonsterCount(_wave) &&
            _enemies.All(e => e.IsDead || e.ReachedGoal))
        {
            _waveInProgress = false;
            int bonus = 20 + _wave * 2;
            _diamonds += bonus;
            _wave++;
            _message = $"Wave cleared: +{bonus} diamonds";
        }
    }

    private void UpdateSpawning(float deltaTime)
    {
        int targetCount = WaveRules.GetMonsterCount(_wave);
        if (_spawned >= targetCount)
            return;

        _spawnElapsed += deltaTime;
        float interval = WaveRules.GetSpawnInterval(_wave);
        while (_spawnElapsed >= interval && _spawned < targetCount)
        {
            _spawnElapsed -= interval;
            EnemyKind kind;
            if (_spawned % 50 == 49)
                kind = _wave >= 20 ? EnemyKind.EliteBoss : EnemyKind.Boss;
            else
            {
                int value = _random.Next(100);
                kind = value < 35 ? EnemyKind.Normal :
                       value < 50 ? EnemyKind.Fast :
                       value < 62 ? EnemyKind.Tank :
                       value < 72 ? EnemyKind.Armored :
                       value < 80 ? EnemyKind.Swarm :
                       value < 86 ? EnemyKind.Regenerator :
                       value < 92 ? EnemyKind.Flying : EnemyKind.Giant;
            }

            _enemies.Add(new Enemy(kind, WaveRules.GetHpMultiplier(_wave), _wave));
            _spawned++;
        }
    }

    private void UpdateUnits(float deltaTime)
    {
        foreach (var unit in _units.ToList())
        {
            unit.AttackElapsed += deltaTime;
            var spec = unit.Spec;
            float attackInterval = spec.AttackInterval / (1f + (unit.Stage - 1) * 0.06f);
            if (unit.AttackElapsed < attackInterval)
                continue;

            var enemiesInRange = _enemies
                .Where(e => !e.IsDead && !e.ReachedGoal)
                .Select(e => (Enemy: e, Distance: Distance(unit.X, unit.Y, GridMap.Path[e.PathIndex].X, GridMap.Path[e.PathIndex].Y)))
                .Where(v => v.Distance <= spec.Range)
                .OrderBy(v => v.Distance)
                .ToList();

            if (enemiesInRange.Count == 0)
                continue;

            unit.AttackElapsed = 0;
            float damage = spec.Damage * (1f + (unit.Stage - 1) * 0.35f);

            if (unit.Kind is UnitKind.Cannon or UnitKind.BlastMage or UnitKind.MeteorMage)
            {
                var origin = enemiesInRange[0].Enemy;
                var originPos = GridMap.Path[origin.PathIndex];
                int splashRange = unit.Kind == UnitKind.MeteorMage ? 3 : 2;
                foreach (var enemy in _enemies.Where(e => !e.IsDead && !e.ReachedGoal))
                {
                    var pos = GridMap.Path[enemy.PathIndex];
                    if (Distance(originPos.X, originPos.Y, pos.X, pos.Y) <= splashRange)
                        enemy.TakeDamage(damage);
                }
            }
            else
            {
                var target = enemiesInRange[0].Enemy;
                target.TakeDamage(damage);
                if (unit.Kind is UnitKind.SlowTower or UnitKind.FrostMage)
                    target.ApplySlow(unit.Kind == UnitKind.FrostMage ? 1.2f : 2f);
                if (unit.Kind == UnitKind.StunTower)
                    target.ApplySlow(1f);
            }

            foreach (var enemy in _enemies.Where(e => e.IsDead).ToList())
            {
                _diamonds += enemy.Reward;
                _enemies.Remove(enemy);
            }
        }
    }

    private static int Distance(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    protected override void Render(Screen screen)
    {
        for (int y = 0; y < GridMap.Height; y++)
        for (int x = 0; x < GridMap.Width; x++)
            screen.Draw(x, y, '.');

        foreach (var tile in GridMap.Path)
            screen.Draw(tile.X, tile.Y, '#', ConsoleColor.DarkYellow);

        screen.Draw(GridMap.Path[0].X, GridMap.Path[0].Y, 'S', ConsoleColor.Green);
        screen.Draw(GridMap.Path[^1].X, GridMap.Path[^1].Y, 'G', ConsoleColor.Red);

        foreach (var unit in _units)
            screen.Draw(unit.X, unit.Y, unit.Symbol, ConsoleColor.Cyan);

        foreach (var enemy in _enemies)
        {
            if (enemy.ReachedGoal || enemy.IsDead) continue;
            var pos = GridMap.Path[enemy.PathIndex];
            screen.Draw(pos.X, pos.Y, enemy.Spec.Symbol, ConsoleColor.Magenta);
        }

        screen.Draw(_cursorX, _cursorY, '*', ConsoleColor.Yellow);

        int statusY = GridMap.Height;
        screen.DrawText(0, statusY, $"Wave:{_wave} HP:{_playerHp} Dia:{_diamonds} Alive:{_enemies.Count(e => !e.IsDead && !e.ReachedGoal)}");
        screen.DrawText(0, statusY + 1, $"Selected:{_selectedKind} | cursor:{_cursorX},{_cursorY}");
        screen.DrawText(0, statusY + 2, _message);
        screen.DrawText(0, statusY + 3, "Space: wave | Enter: summon | U: upgrade | S: sell");
        screen.DrawText(0, statusY + 4, "1-9,0: unit type | Arrows: move | Esc: quit");
        if (_gameOver)
            screen.DrawText(0, statusY + 6, "GAME OVER - Press Esc", ConsoleColor.Red);
    }
}
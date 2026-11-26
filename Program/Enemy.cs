namespace GridDefense;

public enum EnemyKind
{
    Normal, Fast, Tank, Armored, Swarm,
    Regenerator, Flying, Giant, Boss, EliteBoss
}

public sealed record EnemySpec(string Name, char Symbol, int Hp, float MoveInterval, int Reward);

public static class EnemySpecs
{
    private static readonly EnemySpec[] Specs =
    {
        new("Normal", 'e', 100, 0.30f, 4),
        new("Fast", 'f', 70, 0.18f, 5),
        new("Tank", 't', 240, 0.45f, 7),
        new("Armored", 'a', 180, 0.36f, 7),
        new("Swarm", 'w', 55, 0.22f, 3),
        new("Regenerator", 'r', 130, 0.32f, 8),
        new("Flying", 'v', 90, 0.20f, 6),
        new("Giant", 'g', 420, 0.55f, 12),
        new("Boss", 'B', 900, 0.42f, 25),
        new("Elite Boss", 'X', 1800, 0.38f, 50)
    };

    public static EnemySpec Get(EnemyKind kind) => Specs[(int)kind];
}

public sealed class Enemy
{
    private float _moveElapsed;

    public EnemyKind Kind { get; }
    public EnemySpec Spec { get; }
    public float MaxHp { get; }
    public float Hp { get; private set; }
    public int Reward { get; }
    public int PathIndex { get; private set; }
    public bool ReachedGoal { get; private set; }
    public bool IsDead => Hp <= 0;
    public float SlowElapsed { get; private set; }

    public Enemy(EnemyKind kind, float hpMultiplier, int wave)
    {
        Kind = kind;
        Spec = EnemySpecs.Get(kind);
        float bossBonus = kind is EnemyKind.Boss or EnemyKind.EliteBoss ? 1f + wave * 0.03f : 1f;
        MaxHp = Spec.Hp * hpMultiplier * bossBonus;
        Hp = MaxHp;
        Reward = Spec.Reward;
    }

    public void ApplySlow(float duration)
    {
        SlowElapsed = Math.Max(SlowElapsed, duration);
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
    }

    public void Update(float deltaTime)
    {
        if (ReachedGoal || IsDead)
            return;

        SlowElapsed = Math.Max(0f, SlowElapsed - deltaTime);
        float speedFactor = SlowElapsed > 0f ? 2f : 1f;
        _moveElapsed += deltaTime;

        while (_moveElapsed >= Spec.MoveInterval * speedFactor)
        {
            _moveElapsed -= Spec.MoveInterval * speedFactor;
            if (PathIndex < GridMap.Path.Count - 1)
                PathIndex++;

            if (PathIndex == GridMap.Path.Count - 1)
            {
                ReachedGoal = true;
                break;
            }
        }
    }
}
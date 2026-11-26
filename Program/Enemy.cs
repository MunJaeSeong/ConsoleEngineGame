namespace GridDefense;

public sealed class Enemy
{
    private float _moveElapsed;

    public Enemy(int wave, Random random)
    {
        int roll = random.Next(100);
        Type = roll switch
        {
            < 40 => 0,
            < 55 => 1,
            < 68 => 2,
            < 76 => 3,
            < 83 => 4,
            < 88 => 5,
            < 92 => 6,
            < 95 => 7,
            < 98 => 8,
            _ => 9
        };

        decimal hpScale = WaveRules.GetEnemyHpMultiplier(wave);
        int baseHp = Type switch
        {
            0 => 45,
            1 => 30,
            2 => 110,
            3 => 180,
            4 => 38,
            5 => 70,
            6 => 55,
            7 => 90,
            8 => 65,
            _ => 300
        };
        MaxHp = (int)(baseHp * hpScale);
        Hp = MaxHp;
        Speed = Type switch
        {
            1 => 0.20f,
            4 => 0.18f,
            6 => 0.24f,
            9 => 0.38f,
            _ => 0.30f
        };
        Reward = 5 + Type switch
        {
            2 => 5,
            3 => 10,
            9 => 30,
            _ => 0
        };
    }

    public int Type { get; }
    public int PathIndex { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; }
    public int Reward { get; }
    public float Speed { get; }
    public bool ReachedGoal { get; private set; }

    public static char GetSymbol(int type) => type switch
    {
        0 => 'e',
        1 => 'f',
        2 => 't',
        3 => 'H',
        4 => 'v',
        5 => 'b',
        6 => 'w',
        7 => 'p',
        8 => 'm',
        _ => 'B'
    };

    public static string GetName(int type) => type switch
    {
        0 => "Normal",
        1 => "Fast",
        2 => "Tank",
        3 => "Heavy",
        4 => "Runner",
        5 => "Brute",
        6 => "Wisp",
        7 => "Plague",
        8 => "Mage",
        _ => "Boss"
    };

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(0, Hp - damage);
    }

    public void Update(float deltaTime)
    {
        if (ReachedGoal || Hp <= 0)
            return;

        _moveElapsed += deltaTime;
        while (_moveElapsed >= Speed)
        {
            _moveElapsed -= Speed;
            PathIndex++;

            if (PathIndex >= GridMap.Path.Count)
            {
                ReachedGoal = true;
                break;
            }
        }
    }
}
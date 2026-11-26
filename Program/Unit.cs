namespace GridDefense;

public enum UnitType
{
    Archer,
    Sniper,
    Cannon,
    Frost,
    Storm,
    Bomb,
    Volt,
    Toxin,
    Aura,
    Guardian
}

public sealed class Unit
{
    public Unit(UnitType type, int x, int y)
    {
        Type = type;
        X = x;
        Y = y;
        Level = 1;
    }

    public UnitType Type { get; }
    public int X { get; }
    public int Y { get; }
    public int Level { get; private set; }
    public float AttackElapsed { get; set; }
    public int SpentUpgradeCost { get; private set; }

    public static string GetName(UnitType type) => type switch
    {
        UnitType.Archer => "Archer",
        UnitType.Sniper => "Sniper",
        UnitType.Cannon => "Cannon",
        UnitType.Frost => "Frost",
        UnitType.Storm => "Storm",
        UnitType.Bomb => "Bomb",
        UnitType.Volt => "Volt",
        UnitType.Toxin => "Toxin",
        UnitType.Aura => "Aura",
        _ => "Guardian"
    };

    public static char GetSymbol(UnitType type) => type switch
    {
        UnitType.Archer => 'A',
        UnitType.Sniper => 'N',
        UnitType.Cannon => 'C',
        UnitType.Frost => 'F',
        UnitType.Storm => 'T',
        UnitType.Bomb => 'B',
        UnitType.Volt => 'V',
        UnitType.Toxin => 'X',
        UnitType.Aura => 'R',
        _ => 'G'
    };

    public (int Damage, float Interval, int Range) GetStats()
    {
        (int baseDamage, float baseInterval, int baseRange) = Type switch
        {
            UnitType.Archer => (12, 0.80f, 3),
            UnitType.Sniper => (32, 1.60f, 7),
            UnitType.Cannon => (20, 1.25f, 4),
            UnitType.Frost => (8, 1.05f, 3),
            UnitType.Storm => (14, 1.10f, 4),
            UnitType.Bomb => (26, 1.70f, 4),
            UnitType.Volt => (10, 0.70f, 3),
            UnitType.Toxin => (11, 0.95f, 3),
            UnitType.Aura => (7, 0.90f, 5),
            _ => (18, 1.30f, 3)
        };

        float levelMultiplier = 1f + (Level - 1) * 0.22f;
        return ((int)(baseDamage * levelMultiplier), baseInterval * (1f - (Level - 1) * 0.012f), baseRange);
    }

    public int GetUpgradeCost() => 25 + Level * 20;

    public bool TryUpgrade(Random random)
    {
        if (Level >= 10)
            return false;

        int cost = GetUpgradeCost();
        // 비용은 DefenseGame에서 확인하고 여기서도 누적 소모액을 기록합니다.
        SpentUpgradeCost += cost;
        int successPercent = 100 - Level * 8;
        if (random.Next(100) < successPercent)
        {
            Level++;
            return true;
        }
        return false;
    }

    public int GetSellValue()
    {
        decimal levelWeight = 1m + (Level - 1) * 0.65m;
        return (int)((50m + SpentUpgradeCost * 0.55m) * levelWeight);
    }

    public void SpendForUpgrade(int cost)
    {
        SpentUpgradeCost += cost;
    }

    public void Upgrade()
    {
        if (Level < 10)
            Level++;
    }
}

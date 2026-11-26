namespace GridDefense;

public enum UnitKind
{
    Archer, Cannon, Sniper,
    BlastMage, MeteorMage, FrostMage,
    SlowTower, StunTower,
    ArmorBreaker, BattleCry
}

public sealed class Unit
{
    public UnitKind Kind { get; }
    public int X { get; }
    public int Y { get; }
    public int Stage { get; private set; } = 1;
    public int UpgradeCostSpent { get; private set; }
    public float AttackElapsed { get; set; }

    public Unit(UnitKind kind, int x, int y)
    {
        Kind = kind;
        X = x;
        Y = y;
    }

    public UnitSpec Spec => UnitSpecs.Get(Kind);

    public bool Upgrade(Random random)
    {
        int cost = GetUpgradeCost(Stage);
        int targetStage = Stage + 1;
        UpgradeCostSpent += cost;
        int successChance = 100 - (targetStage - 2) * 8;
        if (random.Next(100) >= successChance)
            return false;

        Stage = targetStage;
        return true;
    }

    public static int GetUpgradeCost(int currentStage) => currentStage switch
    {
        1 => 80, 2 => 100, 3 => 120, 4 => 145,
        5 => 175, 6 => 210, 7 => 250, 8 => 300,
        9 => 360, _ => 430
    };

    public int SellValue() => (50 + UpgradeCostSpent * 60 / 100) * Stage;

    public char Symbol => Spec.Symbol;
}

public readonly record struct UnitSpec(
    string Name, char Symbol, int Damage, float AttackInterval,
    int Range, string Skill);

public static class UnitSpecs
{
    private static readonly IReadOnlyDictionary<UnitKind, UnitSpec> Specs =
        new Dictionary<UnitKind, UnitSpec>
        {
            [UnitKind.Archer] = new("Archer", 'A', 18, 0.45f, 5, "Single target"),
            [UnitKind.Cannon] = new("Cannon", 'C', 32, 0.90f, 4, "Splash target"),
            [UnitKind.Sniper] = new("Sniper", 'N', 70, 1.40f, 8, "Single target"),
            [UnitKind.BlastMage] = new("Blast Mage", 'B', 25, 0.70f, 5, "Small splash"),
            [UnitKind.MeteorMage] = new("Meteor", 'M', 55, 1.50f, 7, "Large splash"),
            [UnitKind.FrostMage] = new("Frost Mage", 'F', 20, 0.80f, 5, "10% slow"),
            [UnitKind.SlowTower] = new("Slow Tower", 'L', 12, 0.60f, 4, "Slow all targets"),
            [UnitKind.StunTower] = new("Stun Tower", 'T', 14, 1.20f, 4, "Stun all targets"),
            [UnitKind.ArmorBreaker] = new("Breaker", 'D', 28, 0.75f, 5, "Armor reduction"),
            [UnitKind.BattleCry] = new("Battle Cry", 'H', 10, 1.00f, 6, "Global damage buff")
        };

    public static UnitSpec Get(UnitKind kind) => Specs[kind];
    public static IReadOnlyList<UnitKind> All { get; } = Enum.GetValues<UnitKind>();
}

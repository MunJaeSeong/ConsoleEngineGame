namespace GridDefense;

public static class WaveRules
{
    public const int StartingPlayerHp = 50;
    public const int DamagePerArrival = 1;

    public static int GetEnemyCount(int wave)
    {
        int count = 50 + (Math.Clamp(wave, 1, 46) - 1) * 10;
        return count;
    }

    public static decimal GetEnemyHpMultiplier(int wave)
    {
        int bonusPercent = Math.Max(0, wave - 46);
        return 1m + bonusPercent / 100m;
    }
}
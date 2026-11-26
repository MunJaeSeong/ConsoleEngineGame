namespace GridDefense;

public static class WaveRules
{
    public static int GetMonsterCount(int wave)
        => wave <= 46 ? Math.Min(50 + (wave - 1) * 10, 500) : 500;

    public static float GetHpMultiplier(int wave)
        => wave <= 46 ? 1f : 1f + (wave - 46) * 0.01f;

    public static float GetSpawnInterval(int wave)
        => wave >= 30 ? 0.06f : wave >= 15 ? 0.08f : 0.10f;
}

namespace GridDefense;

public sealed class Enemy
{
    private const float MoveInterval = 0.3f;
    private float _moveElapsed;

    public int PathIndex { get; private set; }
    public bool ReachedGoal { get; private set; }

    public void Update(float deltaTime)
    {
        if (ReachedGoal)
            return;

        _moveElapsed += deltaTime;

        while (_moveElapsed >= MoveInterval)
        {
            _moveElapsed -= MoveInterval;
            PathIndex++;

            if (PathIndex == GridMap.Path.Count - 1)
            {
                ReachedGoal = true;
                break;
            }
        }
    }
}
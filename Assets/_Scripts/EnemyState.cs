using UnityEngine;

public abstract class EnemyState
{
    public EnemyStateParameters EnemyStateParameters;
    public bool StateIsFinished;
    public abstract void Init(Transform transform);
    public abstract void Run();
    public abstract void StateEnd();
}
using UnityEngine;

public class EnemyStateIdle : EnemyState
{
    private Transform _targetTransform;
    private NeckBoss _neckBoss;
    private float _timer;
    private float _timeToWaite;
    private int _numbersOfLook;

    public EnemyStateIdle(Bootstrap bootstrap)
    {
        _neckBoss = bootstrap.NeckBoss;
    }

    public override void Init(Transform target)
    {
        _targetTransform = target;
        _numbersOfLook = Random.Range(2, 5);
        _neckBoss.SetStopDistance(2f);
        _neckBoss.SetTargetForHead(null);
        _neckBoss.SetTargetForMove(null);
    }
    public override void Run() {
        SetTargetToLook();
    }

    private void SetTargetToLook() {
        _timer += Time.deltaTime;
        if (_timer >= _timeToWaite) {
            Vector2 randomPoint = new Vector2(_targetTransform.position.x, _targetTransform.position.y) + Random.insideUnitCircle;
            _neckBoss.SetTargetForHead(randomPoint);
            _timeToWaite = Random.Range(1, 2);
            _timer = 0;
            _numbersOfLook--;
            if (_numbersOfLook <= 0)
                StateEnd();
        }
    }

    public override void StateEnd()
    {
        StateIsFinished = true;
    }
}

using DG.Tweening;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private Transform _targetTransform;
    private Transform _enemyTransform;
    private Rigidbody2D _enemyRb2D;
    private NeckBoss _neckBoss;
    private Sequence _sequence;
    private float _moveSpeed = 500f;

    public EnemyStateAttack(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _enemyRb2D = bootstrap.EnemyNeckData.EnemyRigidbody2D;
        //_targetTransform = bootstrap.GameData.PlayerBody.transform;
    }

    public override void Init(Transform target) {
        _targetTransform = target;
        _neckBoss.SetStopDistance(3f);
        _neckBoss.SetTargetForHead(target);
        AttackPlayer();
    }
    public override void Run() {
    }

    private void AttackPlayer() {
        if (_sequence != null && _sequence.IsActive()) return;
        
        _sequence = DOTween.Sequence();

        Vector2 directionToTarget = (_targetTransform.position - _enemyTransform.position);

        _sequence
            .AppendCallback(() => {
                Vector2 awayFromPlayer = (_enemyTransform.position - _targetTransform.position).normalized;
                float randomAngle = Random.Range(-90f, 90f);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * awayFromPlayer;
                Vector2 randomVelocity = randomDirection * 20f;
                _enemyRb2D.velocity = randomVelocity;
            })
            .AppendInterval(0.4f)
            .AppendCallback(() => directionToTarget = (_targetTransform.position - _enemyTransform.position))
            .AppendInterval(0.1f)
            .AppendCallback(() => {
                
                Vector2 velocityValue = directionToTarget * _moveSpeed;
                _enemyRb2D.AddForce(velocityValue, ForceMode2D.Impulse);
            })
            .AppendInterval(0.6f)
            .OnComplete(() => {
                StateEnd(); })
            .SetAutoKill(true);
    }


    public override void StateEnd() {
        StateIsFinished = true;
    }
}
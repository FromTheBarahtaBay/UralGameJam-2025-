using DG.Tweening;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private Transform _targetTransform;
    private Transform _enemyTransform;
    private Rigidbody2D _enemyRb2D;
    private NeckBoss _neckBoss;
    private Sequence _sequence;
    private float _moveSpeed = 650f;
    private LayerMask _playerMask;
    private bool _isTryAttack = false;

    public EnemyStateAttack(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _enemyRb2D = bootstrap.EnemyNeckData.EnemyRigidbody2D;
        _playerMask = bootstrap.GameData.PlayerMask;
        //_targetTransform = bootstrap.GameData.PlayerBody.transform;
    }

    public override void Init(Transform target) {
        _targetTransform = target;
        _neckBoss.SetStopDistance(3.1f);
        _neckBoss.SetTargetForHead(target);
        AttackPlayer();
    }
    public override void Run() {
    }

    private void AttackPlayer() {
        if (_sequence != null && _sequence.IsActive()) return;

        _sequence.Kill();

        _sequence = DOTween.Sequence();

        Vector2 offset = new Vector2(-.1f, 0f);

        Vector2 directionToTarget = (_targetTransform.position - _enemyTransform.position);

        _sequence
            .AppendCallback(() => {
                Vector2 awayFromPlayer = (_enemyTransform.position - _targetTransform.position).normalized;
                float randomAngle = Random.Range(-90f, 90f);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * awayFromPlayer;
                Vector2 randomVelocity = randomDirection * 30f;
                _enemyRb2D.velocity = randomVelocity;
            })
            .AppendInterval(0.4f)
            .AppendCallback(() => directionToTarget = (_targetTransform.position - _enemyTransform.position))
            .AppendCallback(() => _isTryAttack = true)
            .AppendInterval(0.1f)
            .AppendCallback(() => {
                
                Vector2 velocityValue = directionToTarget * _moveSpeed;
                _enemyRb2D.AddForce(velocityValue, ForceMode2D.Impulse);
            })
            .AppendInterval(0.6f)
            .OnUpdate(() => { CastHitBox(); })
            .OnComplete(() => {
                StateEnd(); })
            .SetAutoKill(true);
    }

    private void CastHitBox() {
        if (!_isTryAttack) return;

        Collider2D[] hits = new Collider2D[1];

        Vector3 checkPosition = (Vector2)_enemyTransform.position;
        float radius = 0.5f;

        int hitCount = Physics2D.OverlapCircleNonAlloc(checkPosition, radius, hits, _playerMask);

        if (hitCount > 0) {
            foreach(var hit in hits) {
                if (hit != null && hit.CompareTag("Player")) {
                    _isTryAttack = false;
                    EventsSystem.OnIsPlayerHited();
                }
            }
        }
    }

    public override void StateEnd() {
        StateIsFinished = true;
    }
}
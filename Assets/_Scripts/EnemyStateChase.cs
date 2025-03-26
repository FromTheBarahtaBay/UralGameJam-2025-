using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateChase : EnemyState
{
    private Transform _playerTransform;
    private Transform _targetTransform;
    private Transform _enemyTransform;
    private NeckBoss _neckBoss;
    private NavMeshPath _path;
    private List<Vector3> _waypoints;
    private float _speed;

    public EnemyStateChase(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _speed = bootstrap.GameData.SpeedToMove * 2.5f;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
    }

    public override void Init(Transform target) {
        if (target != null) {
            _targetTransform = target;
        } else {
            _targetTransform = _playerTransform;
        }
        _neckBoss.SetNoveSpeed(_speed);
        _neckBoss.SetStopDistance(0f);
        _neckBoss.SetTargetForHead(target);
    }

    public override void Run() {
        MakePathToPlayer();
        CheckStateDone();
    }

    private void MakePathToPlayer() {
        _path = new NavMeshPath();

        if (NavMesh.CalculatePath(_enemyTransform.position, _targetTransform.position, NavMesh.AllAreas, _path)) {
            if (_path.status != NavMeshPathStatus.PathComplete) {
                _waypoints = null;

                if (_targetTransform == _playerTransform) {
                    _neckBoss.SetTargetForMove(_playerTransform);
                } else {
                    NavMesh.SamplePosition(_enemyTransform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas);
                    _neckBoss.SetTargetForMove(hit.position);
                }
                return;
            }

            _waypoints = new List<Vector3>(_path.corners);

            for (int i = 0; i < _waypoints.Count - 1; i++) {
                Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.green);
                Debug.DrawRay(_waypoints[i], Vector2.up, Color.blue, 1f);
            }

            if (_waypoints.Count > 1) {
                _neckBoss.SetTargetForMove(_waypoints[1]);
            }
        }
    }

    private void CheckStateDone() {
        if (Vector3.Distance(_targetTransform.position, _enemyTransform.position) <= 2.5f) {
            StateEnd();
        }
    }

    public override void StateEnd() {
        StateIsFinished = true;
    }
}
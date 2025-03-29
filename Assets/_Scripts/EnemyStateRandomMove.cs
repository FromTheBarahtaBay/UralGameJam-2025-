using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateRandomMove : EnemyState
{
    private Vector3 _targetPosition;
    private Transform _playerTransform;
    private Transform _enemyTransform;
    private NeckBoss _neckBoss;
    private NavMeshPath _path;
    private List<Vector3> _waypoints;
    private int _moves;

    public EnemyStateRandomMove(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
    }

    public override void Init(Transform target) {
        _moves = Random.Range(1, 3);
        _targetPosition = target.position;
        _neckBoss.SetNoveSpeed(10f); //6
        _neckBoss.SetStopDistance(0f);
        _neckBoss.SetTargetForHead(target);
    }

    public override void Run() {
        MakePathToTarget();
        CheckStateDone();
    }

    private void MakePathToTarget() {
        _path = new NavMeshPath();

        NavMesh.CalculatePath(_enemyTransform.position, _targetPosition, NavMesh.AllAreas, _path);

        if (_path.status != NavMeshPathStatus.PathComplete) {
            _waypoints = null;
            if (NavMesh.SamplePosition(_enemyTransform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
                _enemyTransform.position = hit.position;
            }
            StateEnd();
            return;
        }

        _waypoints = new List<Vector3>(_path.corners);

        for (int i = 0; i < _waypoints.Count - 1; i++) {
            Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.green);
            Debug.DrawRay(_waypoints[i], Vector2.up, Color.blue, 1f);
        }

        if (_waypoints.Count > 1) {
            _neckBoss.SetTargetForMove(_waypoints[1]);
            _neckBoss.SetTargetForHead(_waypoints[1]);
        }
    }

    private void CheckStateDone() {
        if (Vector3.Distance(_targetPosition, _enemyTransform.position) < 2) {
            if (_moves-- > 0) {
                _targetPosition = FindRandomNavMeshPosition();
            } else
                StateEnd();
        }
    }

    private Vector3 FindRandomNavMeshPosition() {

        Vector3 randomPosition = _enemyTransform.position;

        for (int i = 0; i < 3; i++) {

            randomPosition = (Vector2)_playerTransform.position + Random.insideUnitCircle.normalized * Random.Range(7f, 20f);

            NavMesh.CalculatePath(_enemyTransform.position, randomPosition, NavMesh.AllAreas, _path);

            if (_path.status != NavMeshPathStatus.PathComplete) {
                return _enemyTransform.position;
            }
        }
        return randomPosition;
    }

    public override void StateEnd() {
        StateIsFinished = true;
    }
}
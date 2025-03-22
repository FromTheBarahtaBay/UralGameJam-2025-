using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateSneaking : EnemyState
{
    private Transform _playerTransform;
    private Transform _enemyTransform;
    private Transform _bodyTransform;
    private NeckBoss _neckBoss;
    private NavMeshPath _path;
    private List<Vector3> _waypoints;
    private int _layerMask;
    private float _timer;
    private float _timeToWaite;

    public EnemyStateSneaking(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _bodyTransform = bootstrap.EnemyNeckData.EnemyBody.transform;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _layerMask = LayerMask.GetMask("HighObjects");
    }

    public override void Init(Transform target) {
        _timer = 0;
        _timeToWaite = Random.Range(15, 45);
        _neckBoss.SetNoveSpeed(12f);
        _neckBoss.SetStopDistance(0f);
        _neckBoss.SetTargetForHead(_playerTransform);
    }

    public override void Run() {
        _timer += Time.deltaTime;
        MakePathToPlayer();
        CheckStateDone();
    }

    private void MakePathToPlayer() {
        _path = new NavMeshPath();

        if (NavMesh.CalculatePath(_enemyTransform.position, _playerTransform.position, NavMesh.AllAreas, _path)) {
            if (_path.status != NavMeshPathStatus.PathComplete) {
                _waypoints = null;
                _neckBoss.SetTargetForMove(_playerTransform);
                return;
            }

            _waypoints = new List<Vector3>(_path.corners);

            for (int i = 0; i < _waypoints.Count - 1; i++) {
                Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.green);
                Debug.DrawRay(_waypoints[i], Vector2.up, Color.blue, 1f);
            }

            RaycastHit2D hit = Physics2D.Linecast(_enemyTransform.position, _playerTransform.position, _layerMask);

            if (hit.collider == null) {
                _neckBoss.SetNoveSpeed(0f);
            }
            else {
                _neckBoss.SetNoveSpeed(8f);
            }

            if (_waypoints.Count > 1) {
                _neckBoss.SetTargetForMove(_waypoints[1]);
            }
        }
    }

    private void CheckStateDone() {

        float distance = Vector3.Distance(_playerTransform.position, _enemyTransform.position);

        if (distance < 3 || _timer > _timeToWaite) {
            _neckBoss.SetNoveSpeed(6f);
            StateEnd();
        }
    }

    public override void StateEnd() {
        StateIsFinished = true;
    }
}
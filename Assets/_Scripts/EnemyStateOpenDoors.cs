using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyStateOpenDoors : EnemyState
{
    private Transform _playerTransform;
    private Transform _enemyTransform;
    private NeckBoss _neckBoss;
    private NavMeshPath _path;
    private List<Vector3> _waypoints;
    private int _layerMask;
    private bool _isReady = false;
    private List<Transform> _listOfDoors = new List<Transform>();

    public EnemyStateOpenDoors(Bootstrap bootstrap) {
        _neckBoss = bootstrap.NeckBoss;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _layerMask = LayerMask.GetMask("Doors");
    }

    public override void Init(Transform transform) {
        _neckBoss.SetNoveSpeed(10f); //6
        _neckBoss.SetStopDistance(0f);
        RunLocator();
    }

    public override void Run() {
        MakePathToTarget();
        CheckStateDone();
    }

    private void RunLocator() {

        _listOfDoors.Clear();

        Collider2D[] hits = new Collider2D[10];

        Vector3 checkPosition = (Vector2)_enemyTransform.position;
        float radius = 100f;

        int hitCount = Physics2D.OverlapCircleNonAlloc(checkPosition, radius, hits, _layerMask);

        if (hitCount > 0) {
            foreach (var hit in hits) {
                if (hit != null && hit.CompareTag("Door")) {
                    _listOfDoors.Add(hit.transform);
                }
            }
        }
        _listOfDoors = _listOfDoors.OrderBy(x => Random.value).ToList();
    }

    private void MakePathToTarget() {

        if (_listOfDoors.Count <= 0) {
            StateEnd();
            return;
        }

        _path = new NavMeshPath();

        NavMesh.CalculatePath(_enemyTransform.position, _listOfDoors[0].position, NavMesh.AllAreas, _path);

        if (_path.status != NavMeshPathStatus.PathComplete) {
            _waypoints = null;
            if (NavMesh.SamplePosition(_enemyTransform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
                _enemyTransform.position = hit.position;
                _listOfDoors.RemoveAt(0);
                if (_listOfDoors.Count == 0) return;
            } else {
                return;
            }
        }

        _waypoints = new List<Vector3>(_path.corners);

        for (int i = 0; i < _waypoints.Count - 1; i++) {
            Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.green);
            Debug.DrawRay(_waypoints[i], Vector2.up, Color.blue, 1f);
        }

        if (Vector2.Distance(_enemyTransform.position, _listOfDoors[0].position) < 1.1f) {

            if (!_isReady) {
                _neckBoss.SetTargetForMove(_listOfDoors[0].position);
                _isReady = true;
            }
        } else if (_waypoints.Count > 1) {
            if (!_isReady) {
                _neckBoss.SetTargetForMove(_waypoints[1]);
                _neckBoss.SetTargetForHead(_waypoints[1]);
            } else {
                _listOfDoors.RemoveAt(0);
                _isReady = false;
            }
                
        }
    }

    private void CheckStateDone() {
        _path = new NavMeshPath();

        if (NavMesh.CalculatePath(_enemyTransform.position, _playerTransform.position, NavMesh.AllAreas, _path)) {
            if (_path.status == NavMeshPathStatus.PathComplete) {
                _listOfDoors.Clear();
                StateEnd();
            }
        }
    }

    public override void StateEnd() {
        StateIsFinished = true;
    }
}
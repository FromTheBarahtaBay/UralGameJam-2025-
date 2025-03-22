using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatesController
{
    private GameObject _mark;
    private Transform _playerTransform;
    private Transform _enemyTransform;
    private Transform _targetTransform;
    private EnemyStateChase _chaseState;
    private EnemyStateIdle _idleState;
    private EnemyStateAttack _attackState;
    private EnemyStateRandomMove _randomMoveState;
    private EnemyStateSneaking _sneakingState;
    private EnemyState _currentState;
    private NeckBoss _neckBoss;

    private float _timer;
    private float _timeToWaite;
    private int _layerMask;

    public EnemyStatesController(Bootstrap bootstrap) {
        _mark = new GameObject("mark");
        _mark.layer = 10;
        _mark.tag = "Mark";
        _chaseState = new EnemyStateChase(bootstrap);
        _attackState = new EnemyStateAttack(bootstrap);
        _idleState = new EnemyStateIdle(bootstrap);
        _randomMoveState = new EnemyStateRandomMove(bootstrap);
        _sneakingState = new EnemyStateSneaking(bootstrap);
        _neckBoss = bootstrap.NeckBoss;
        AssignStates(bootstrap.EnemyNeckData.States);
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _currentState = _sneakingState;
        _currentState.Init(_enemyTransform);
        _layerMask = LayerMask.GetMask("HighObjects");
        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void AssignStates(EnemyStateSO[] States) {
        foreach (var state in States) {
            if (state.EnemyStateParameters.TypeOfState == "chase")
                _chaseState.EnemyStateParameters = state.EnemyStateParameters;
            else { }
        }
    }

    private void OnUpdate() {
        CheckCurrentState();
        _currentState.Run();
    }

    private void OnFixedUpdate() {
        RunLockator();
    }

    private void CheckCurrentState() {
        if (_currentState.StateIsFinished) {
            _currentState.StateIsFinished = false;
            SetCurrentState();
        }
    }

    private void SetCurrentState() {

        if (_targetTransform == _playerTransform) {
            float distance = Vector2.Distance(_playerTransform.position, _enemyTransform.position);
            _currentState = (distance > 2) ? _chaseState : _attackState;
        } else {
            _targetTransform = _enemyTransform;

            switch (_currentState) {
                case EnemyStateIdle:
                    _currentState = ShouldSneak() ? _sneakingState : _randomMoveState;
                    break;
                case EnemyStateRandomMove:
                    _currentState = _idleState;
                    break;
                case EnemyStateSneaking:
                    _targetTransform = _playerTransform;
                    _currentState = _chaseState;
                    break;
                default:
                    _currentState = _randomMoveState;
                    break;
            }
        }

        _currentState.Init(_targetTransform);
    }

    private bool ShouldSneak() {
        return Random.value > 0.98f;
    }

    private void RunLockator() {
        _timer += Time.fixedDeltaTime;
        if (_timer > _timeToWaite) {

            RaycastHit2D hit = Physics2D.Linecast(_enemyTransform.position, _playerTransform.position, _layerMask);

            float distance = Vector2.Distance(_playerTransform.position, _enemyTransform.position);

            if (hit.collider == null && distance < 10) {

                if (_currentState is EnemyStateIdle ||
                    _currentState is EnemyStateRandomMove ||
                    _currentState is EnemyStateAttack) {
                    _targetTransform = _playerTransform;
                    _currentState.StateIsFinished = true;
                }  
            } else {
                switch (_currentState) {
                    case EnemyStateChase:
                        _mark.transform.position = _playerTransform.position;
                        _targetTransform = _mark.transform;
                        _currentState = _randomMoveState;
                        _currentState.Init(_targetTransform);
                        break;
                    default:
                        _targetTransform = _enemyTransform;
                        break;
                }
            }
            _timer = 0;
            _timeToWaite = Random.Range(.5f, 2.5f);
        }
    }
}
using UnityEngine;

public class NeckBossIKController
{
    private Transform[] _bodyparts;
    private Transform[] _targets;
    private Vector3[] _currentPositions;
    private Transform[] _spineBones;
    private Quaternion[] _defaultRotation;
    private float _moveDistance = 1.1f;
    private float _magnitude = 50;
    private float _speedAnimation = 25f;
    private int _inverse = -1;

    public NeckBossIKController(Bootstrap bootstrap) {
        _bodyparts = bootstrap.EnemyNeckData.Bodyparts;
        _targets = bootstrap.EnemyNeckData.Targets;
        _currentPositions = new Vector3[_targets.Length];
        _spineBones = bootstrap.EnemyNeckData.SpineBones;
        _magnitude = bootstrap.EnemyNeckData.Magnitude;
        _moveDistance = bootstrap.EnemyNeckData.MoveDistance;
        _speedAnimation = bootstrap.EnemyNeckData.SpeedAnimation;
        SetDefaultRotations();
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void SetDefaultRotations() {
        _defaultRotation = new Quaternion[_spineBones.Length];
        for (int i = 0; i < _spineBones.Length; i++) {
            _defaultRotation[i] = _spineBones[i].localRotation;
        }
    }

    private void OnFixedUpdate() {
        MoveLimbs();
        RotateTorso();
    }

    private void MoveLimbs() {

        for (int i = 0; i < _bodyparts.Length; i++) {

            float distance = Vector3.Distance(_currentPositions[i], _targets[i].position);

            if (distance > _moveDistance) {
                _currentPositions[i] = new Vector2(_targets[i].position.x, _targets[i].position.y) + Random.insideUnitCircle * Random.Range(0, .3f);
                if (i == 0) _inverse *= -1;
            }

            _bodyparts[i].position = Vector3.Lerp(_bodyparts[i].position, _currentPositions[i], Time.deltaTime * _speedAnimation);
        }
    }

    private void RotateTorso() {
        for (int i = 0; i < _spineBones.Length; i++) {

            var targetAngle = _defaultRotation[i] * Quaternion.Euler(0, 0, _magnitude * ((i % 2 == 0) ? _inverse  : -_inverse * 2));

            _spineBones[i].localRotation = Quaternion.Lerp(_spineBones[i].localRotation, targetAngle, Time.deltaTime * _speedAnimation / 5);
        }
    }
}
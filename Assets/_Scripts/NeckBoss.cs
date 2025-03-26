using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckBoss
{
    private Transform _currentTargertHead;
    private Transform _currentTargertMove;
    private Transform _defaultTargetMove;
    public Transform DefaultTargetMove { get { return _defaultTargetMove; } }
    private float _stopDistance = 100f;

    // Looking and Moving
    private Transform _transformHead;
    private float _rotationSpeed = 10f;
    private float _moveSpeed = 5f;
    private Vector2 _direction;
    private Camera _camera;
    private Rigidbody2D _rigidbody2D;
    Vector3 _mousePosition;

    // Neck
    private Transform _tarGetDir;
    private int _lengthOfNeck = 15;
    private LineRenderer _lineRend;
    private Vector3[] _segmentPoses;
    private Vector3[] _segmentV;
    private float _targetDist = 0.2f;
    private float _smoothSpeed = 0.001f;
    private float _wiggleSpeed = 100f;
    private float _wiggleMagnitude = 500f;
    private Transform _wiggleDir;
    private Transform _tailEnd;

    public NeckBoss(Bootstrap bootstrap) {
        _defaultTargetMove = new GameObject("target").transform;
        _transformHead = bootstrap.EnemyNeckData.EnemyHead.transform;
        _camera = bootstrap.GameData.Camera;
        _rigidbody2D = bootstrap.EnemyNeckData.EnemyRigidbody2D;

        _lineRend = bootstrap.EnemyNeckData.NeckLine;
        _lineRend.positionCount = _lengthOfNeck;
        _segmentPoses = new Vector3[_lengthOfNeck];
        _segmentV = new Vector3[_lengthOfNeck];
        _wiggleDir = bootstrap.EnemyNeckData.WiggleDir.transform;
        _tarGetDir = bootstrap.EnemyNeckData.TargetDir.transform;
        _tailEnd = bootstrap.EnemyNeckData.EnemyBody.transform;

        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);

        SetTargetForHead(bootstrap.GameData.PlayerBody.transform);
        SetTargetForMove(bootstrap.GameData.PlayerBody.transform);
    }

    public void SetNoveSpeed(float value) {
        _moveSpeed = value;
    }

    public void SetTargetForHead(Vector3 targetPosition) {
        _defaultTargetMove.position = targetPosition;
        _currentTargertHead = _defaultTargetMove;
    }

    public void SetTargetForHead(Transform target) {
        _currentTargertHead = target;
    }

    public void SetTargetForMove(Transform target) {
        _currentTargertMove = target;
    }

    public void SetTargetForMove(Vector3 targetPosition) {
        _defaultTargetMove.position = targetPosition;
        _currentTargertMove = _defaultTargetMove;
    }

    public void SetStopDistance(float value) {
        _stopDistance = value;
    }

    private void OnUpdate() {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        Looking();
        NeckMoving();
    }

    private void OnFixedUpdate() {
        Moving();
    }

    private void Looking() {
        if (_currentTargertHead == null) return;
        _direction = _currentTargertHead.position - _transformHead.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        _transformHead.rotation = Quaternion.Slerp(_transformHead.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }

    private void Moving() {
        if (_currentTargertMove == null) return;
        if (Vector2.Distance(_currentTargertMove.position, _transformHead.position) > _stopDistance) {
            Vector3 directionToTarget = (_currentTargertMove.position - _transformHead.position).normalized;
            Vector2 velocity = directionToTarget * _moveSpeed;
            _rigidbody2D.velocity = velocity;
        }
    }

    private void NeckMoving() {

        _wiggleDir.localRotation = Quaternion.Euler(0,0, Mathf.Sin(Time.time * _wiggleSpeed) * _wiggleMagnitude);

        _segmentPoses[0] = _transformHead.position;

        for (int i = 1; i < _segmentPoses.Length; i++) {
            Vector3 targetPos = _segmentPoses[i-1] + (_segmentPoses[i] - _segmentPoses[i-1]).normalized * _targetDist;
            _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i], targetPos, ref _segmentV[i], _smoothSpeed);
        }

        _lineRend.SetPositions(_segmentPoses);

        _tailEnd.position = _segmentPoses[_segmentPoses.Length - 1];

        Vector3 direction = _segmentPoses[_segmentPoses.Length - 2] - _tailEnd.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        _tailEnd.rotation = Quaternion.Slerp(_tailEnd.rotation, rotation, 50 * Time.deltaTime);
    }

    public void ResetPos() {
        _segmentPoses[0] = _tarGetDir.position;

        for (int i = 1; i < _lengthOfNeck; i++) {
            _segmentPoses[i] = _segmentPoses[i - 1] + _tarGetDir.right * _targetDist;
        }
        _lineRend.SetPositions(_segmentPoses);
    }
}
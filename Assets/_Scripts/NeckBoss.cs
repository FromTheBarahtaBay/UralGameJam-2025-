using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckBoss
{
    private Transform _currentTargertHead;
    private Transform _currentTargertMove;

    // Looking and Moving
    private Transform _transformHead;
    private float _rotationSpeed = 20f;
    private float _moveSpeed = 4f;
    private Vector2 _direction;
    private Camera _camera;
    private Rigidbody2D _rigidbody2D;
    Vector3 _mousePosition;

    // Neck
    private Transform _tarGetDir;
    private int _lengthOfNeck = 30;
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

        _transformHead = bootstrap.GameData.EnemyHead.transform;
        _camera = bootstrap.GameData.Camera;
        _rigidbody2D = bootstrap.GameData.EnemyRigidbody2D;

        _lineRend = bootstrap.GameData.NeckLine;
        _lineRend.positionCount = _lengthOfNeck;
        _segmentPoses = new Vector3[_lengthOfNeck];
        _segmentV = new Vector3[_lengthOfNeck];
        _wiggleDir = bootstrap.GameData.WiggleDir.transform;
        _tarGetDir = bootstrap.GameData.TargetDir.transform;
        _tailEnd = bootstrap.GameData.EnemyBody.transform;

        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);

        //ResetPos();
        SetTargetForHead(bootstrap.GameData.PlayerBody.transform);
        SetTargetForMove(bootstrap.GameData.PlayerBody.transform);
    }

    public void SetTargetForHead(Transform target) {
        _currentTargertHead = target;
    }

    public void SetTargetForMove(Transform target) {
        _currentTargertMove = target;
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
        
        _direction = _currentTargertHead.position - _transformHead.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        _transformHead.rotation = Quaternion.Slerp(_transformHead.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }

    private void Moving() {
        if (Vector2.Distance(_currentTargertMove.position, _transformHead.position) > 2f) {
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

    private void ResetPos() {
        _segmentPoses[0] = _tarGetDir.position;

        for (int i = 1; i < _lengthOfNeck; i++) {
            _segmentPoses[i] = _segmentPoses[i - 1] + _tarGetDir.right * _targetDist;
        }
        _lineRend.SetPositions(_segmentPoses);
    }
}
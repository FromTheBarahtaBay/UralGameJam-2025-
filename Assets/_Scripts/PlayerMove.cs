using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    private Transform _playerTransform;
    private Camera _camera;
    private Rigidbody2D _playerRigidbody2D;
    private float _speed;
    private float _currentSpeed;
    float _h;
    float _v;

    public PlayerMove(Bootstrap bootstrap) {
        _playerRigidbody2D = bootstrap.GameData.PlayerRigidbody2D;
        _playerRigidbody2D.centerOfMass = Vector2.zero;
        _speed = bootstrap.GameData.SpeedToMove;
        _currentSpeed = _speed;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _camera = bootstrap.GameData.Camera;
        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void OnUpdate() {
        Control();
    }

    private void OnFixedUpdate()
    {
        Move();
    }

    private void Control() {

        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        if (_h == 0 && _v == 0) return;

        Vector3 directionToTarget = (_camera.ScreenToWorldPoint(Input.mousePosition) - _playerTransform.position).normalized;
        Vector3 playerVelocity = _playerRigidbody2D.velocity.normalized;
        float dot = Vector3.Dot(playerVelocity, directionToTarget);

        float targetSpeed = (dot < 0) ? _speed / 3 : _speed;

        if (Input.GetKey(KeyCode.LeftShift)) {
            targetSpeed *= 2f;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            _currentSpeed = _speed * 3.2f;
        } else {
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * 2f);
        }
    }

    private void Move() {

        Vector2 velocity = new Vector2(_h, _v);

        if (velocity.magnitude > 1f) {
            velocity.Normalize();
        }

        _playerRigidbody2D.velocity = velocity * _currentSpeed;
    }
}
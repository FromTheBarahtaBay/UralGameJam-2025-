using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    private Transform _playerTransform;
    private Camera _camera;
    private Rigidbody2D _playerRigidbody2D;
    private float _speed = 5f;
    private float _currentSpeed;

    public PlayerMove(Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnFixedUpdate, false);
        _playerRigidbody2D = bootstrap.GameData.PlayerRigidbody2D;
        _playerRigidbody2D.centerOfMass = Vector2.zero;
        _speed = bootstrap.GameData.SpeedToMove;
        _currentSpeed = _speed;
        _playerTransform = bootstrap.GameData.Player.transform;
        _camera = bootstrap.GameData.Camera;
    }

    private void OnFixedUpdate()
    {
        Move();
    }

    private void Move() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 directionToTarget = (_camera.ScreenToWorldPoint(Input.mousePosition) - _playerTransform.position).normalized;
        Vector3 playerVelocity = _playerRigidbody2D.velocity.normalized;
        float dot = Vector3.Dot(playerVelocity, directionToTarget);

        float targetSpeed = (dot < 0) ? _speed / 3 : _speed;

        if (Input.GetKey(KeyCode.LeftShift)) {
            targetSpeed *= 2f;
        }

        if (Input.GetKey(KeyCode.Space)) {
            _currentSpeed = _speed * 2.5f;
        } else {
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * 2f);
        }

        Vector2 velocity = new Vector2(h, v) * _currentSpeed;

        _playerRigidbody2D.velocity = velocity;
    }
}
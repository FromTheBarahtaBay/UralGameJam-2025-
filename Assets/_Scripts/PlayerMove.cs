using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    private Transform _playerTransform;
    private Rigidbody2D _playerRigidbody2D;
    private float _speed = 5f;

    public PlayerMove(Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnFixedUpdate, false);
        _playerRigidbody2D = bootstrap.GameData.PlayerRigidbody2D;
        _playerRigidbody2D.centerOfMass = Vector2.zero;
        _speed = bootstrap.GameData.SpeedToMove;
        _playerTransform = bootstrap.GameData.Player.transform;
    }

    private void OnFixedUpdate()
    {
        Move();
    }

    private void Move() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 velocity = new Vector2(h, v) * _speed;

        _playerRigidbody2D.velocity = velocity;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame
{
    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Transform _playerTransform;
    private Transform _enemyTransform;

    public NewGame(Bootstrap bootstrap) {
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _playerStartPosition = _playerTransform.position;

        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _enemyStartPosition = _enemyTransform.position;

        EventsSystem.IsNewGame += StartNewGame;
        bootstrap.AddActionOnDisable(OnDisable);
    }

    private void StartNewGame() {

        _playerTransform.position = _playerStartPosition;
        _enemyTransform.position = _enemyStartPosition;
    }

    private void OnDisable() {
        EventsSystem.IsNewGame -= StartNewGame;
    }
}
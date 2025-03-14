using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameData _gameData;
    public GameData GameData { get { return _gameData; } }

    private List<Action> _actionsOnUpdate = new();

    private void Awake()
    {
        GameIsReady(false);
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame() {
        StartCoroutine(LoadGameData());
        yield return StartCoroutine(LoadGameBar());
        GameIsReady(true);
    }

    private IEnumerator LoadGameData() {
        yield return LoadHeavyData(() => { _gameData.TryFindNullFields(); });
        yield return LoadHeavyData(() => { new MouseTracker(this); });
    }

    private IEnumerator LoadHeavyData(Action action) {
        action.Invoke();
        yield return null;
    }

    private IEnumerator LoadGameBar() {
        float progress = 0f;
        while (progress <= 1) {
            progress += Time.deltaTime / .1f;
            _gameData.ProgressBar.value = progress;
            yield return null;
        }
    }

    private void Update()
    {
        foreach (var action in _actionsOnUpdate)
            action?.Invoke();
    }

    public void AddActionToList(Action action) {
        _actionsOnUpdate.Add(action);
    }

    private void GameIsReady(bool value) {
        _gameData.Canvas.gameObject.SetActive(!value);
        _gameData.ProgressBar.value = !value ? 1 : 0;
    }
}
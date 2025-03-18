using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameData _gameData;
    public GameData GameData { get { return _gameData; } }

    private List<Action> _actionsOnUpdate = new();
    private List<Action> _actionsOnFixedUpdate = new();

    private void Awake()
    {
        GameIsReady(false);
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame() {
        var coroutinData = StartCoroutine(LoadGameData());
        var coroutineBar = StartCoroutine(LoadGameBar());
        yield return coroutinData;
        yield return coroutineBar;
        GameIsReady(true);
    }

    private IEnumerator LoadGameData() {
        yield return LoadHeavyData(() => { _gameData.TryFindNullFields(_gameData); });
        yield return LoadHeavyData(() => { new MouseTracker(this); });
        yield return LoadHeavyData(() => { new TurnPlayerToMouse(this); });
        yield return LoadHeavyData(() => { new PlayerMove(this); });
        yield return LoadHeavyData(() => { new CameraMove(this); });
        yield return LoadHeavyData(() => { new FieldOfView(this); });
        yield return LoadHeavyData(() => { new NeckBoss(this); });
    }

    private IEnumerator LoadHeavyData(Action action) {
        action.Invoke();
        yield return null;
    }

    private IEnumerator LoadGameBar() {
        float progress = 0f;
        while (progress <= 1) {
            progress += Time.deltaTime / .4f;
            _gameData.ProgressBar.value = progress;
            yield return null;
        }
    }

    private void Update()
    {
        foreach (var action in _actionsOnUpdate)
            action?.Invoke();
    }

    private void FixedUpdate() {
        foreach (var action in _actionsOnFixedUpdate)
            action?.Invoke();
    }

    public void AddActionToList(Action action, bool toUpdate) {
        if (toUpdate)
            _actionsOnUpdate.Add(action);
        else
            _actionsOnFixedUpdate.Add(action);
    }

    private void GameIsReady(bool value) {
        _gameData.Canvas.gameObject.SetActive(!value);
        _gameData.ProgressBar.value = !value ? 1 : 0;
    }
}
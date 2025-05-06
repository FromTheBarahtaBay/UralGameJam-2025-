using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameData _gameData;
    public GameData GameData { get { return _gameData; } }

    [SerializeField] private EnemyNeckData _enemyNeckData;
    public EnemyNeckData EnemyNeckData { get { return _enemyNeckData; } }

    private List<Action> _actionsOnUpdate = new();
    private List<Action> _actionsOnFixedUpdate = new();
    private List<Action> _actionsOnDisable = new();

    public NeckBoss NeckBoss { get; private set; }
    private UIController _uIController;
    private bool _gameOnPause = true;

    private void Awake()
    {
        PauseGame();
        GameIsReady(false);
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame() {
        var coroutineData = StartCoroutine(LoadGameData());
        var coroutineBar = StartCoroutine(LoadGameBar());
        yield return coroutineData;
        yield return coroutineBar;
        GameIsReady(true);
    }

    private IEnumerator LoadGameData() {
        yield return LoadHeavyData(() => { _gameData.TryFindNullFields(_gameData); });
        yield return LoadHeavyData(() => { _uIController = new UIController(this); });
        yield return LoadHeavyData(() => { NeckBoss = new NeckBoss(this); });
        yield return LoadHeavyData(() => { new EnemyStatesController(this); });
        yield return LoadHeavyData(() => { new MouseTracker(this); });
        yield return LoadHeavyData(() => { new TurnPlayerToMouse(this); });
        yield return LoadHeavyData(() => { new FieldOfView(this); });
        yield return LoadHeavyData(() => { new NeckBossIKController(this); });
        yield return LoadHeavyData(() => { new PlayerMove(this); });
        yield return LoadHeavyData(() => { new CameraMove(this); });
        yield return LoadHeavyData(() => { new PlayerHealthSystem(this); });
        yield return LoadHeavyData(() => { new NewGame(this); });
        yield return LoadHeavyData(() => { new PlayerIKController(this); });
        yield return LoadHeavyData(() => { new GamePickToys(this); });
        yield return LoadHeavyData(() => { new DialogSystem(this); });
        yield return LoadHeavyData(() => { new EnemyTrigger(this); });
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

    private void Update() {
        if (_gameOnPause) return;

        foreach (var action in _actionsOnUpdate)
            action?.Invoke();
    }

    private void FixedUpdate() {
        if (_gameOnPause) return;

        foreach (var action in _actionsOnFixedUpdate)
            action?.Invoke();
    }

    public void AddActionToList(Action action, bool toUpdate = true) {
        if (toUpdate)
            _actionsOnUpdate.Add(action);
        else
            _actionsOnFixedUpdate.Add(action);
    }

    public void AddActionOnDisable(Action action) {
        _actionsOnDisable.Add(action);
    }

    private void GameIsReady(bool value) {
        if (!value) _gameData.CanvasCommon.gameObject.SetActive(!value);
        Cursor.visible = value;
        _gameData.CanvasMenu.gameObject.SetActive(value);
        _gameData.ProgressBar.value = !value ? 1 : 0;
        _gameData.ProgressBar.gameObject.SetActive(!value);
        if (_uIController != null) _uIController.Init();
    }

    private void OnDisable() {
        foreach (var action in _actionsOnDisable) {
            action?.Invoke();
        }    
    }

    public void PauseGame() {
        _gameOnPause = true;
    }

    public void RunGame() {
        _gameOnPause = false;
    }
}
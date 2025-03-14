using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private MouseTracker _mouseTracker;
    private List<Action> _actionsOnUpdate = new();

    private void Awake()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame() {
        StartCoroutine(LoadGameBar());
        yield return StartCoroutine(LoadGameData());
    }

    private IEnumerator LoadGameData() {
        yield return LoadHeavyData(() => { _mouseTracker = new MouseTracker(this); });
    }

    private IEnumerator LoadHeavyData(Action action) {
        action.Invoke();
        yield return null;
    }

    private IEnumerator LoadGameBar() {
        float progress = 0f;
        while (progress <= 1) {
            progress += Time.deltaTime / 1f;
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
}
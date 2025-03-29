using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePickToys
{
    private List<GameObject> _createdToys = new List<GameObject>();
    private HashSet<int> _toysList = new HashSet<int>();
    private Transform _triggerTransform;
    private LayerMask _layerMask;
    private float _timer;

    private bool _isWin = false;

    private GameObject[] _toys;
    private Transform[] _spawners;

    public GamePickToys(Bootstrap bootstrap) {
        _toys = bootstrap.GameData.Toys;
        _spawners = bootstrap.GameData.Spawners;
        _triggerTransform = bootstrap.GameData.Trigger;
        _layerMask = LayerMask.GetMask("BehindMask");
        bootstrap.AddActionToList(OnFixedUpdate, false);
        EventsSystem.IsNewGame += SetToysOnSpawners;
        bootstrap.AddActionOnDisable(OnDisable);
    }

    private void OnFixedUpdate() {
        HandleTrigger();
    }

    private void HandleTrigger() {
        if (Time.time - _timer < 2f) return;
        Collider2D[] hits = Physics2D.OverlapBoxAll(_triggerTransform.position, new Vector2(10f, 11f), 0f, _layerMask);
        foreach (var hit in hits) {
            if (hit.CompareTag("Toys") && !_toysList.Contains(hit.gameObject.GetInstanceID())) {
                _toysList.Add(hit.gameObject.GetInstanceID());
                EventsSystem.OnIsToyFind(_toysList.Count);
                if (hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D body)) {
                    body.mass = 10;
                    body.drag = 10;
                    body.angularDrag = 10;
                }
            }
        }
        _timer = Time.time;
        if (_toys.Length <= _toysList.Count && !_isWin) {
            EventsSystem.OnIsPlayerWin();
            _isWin = true;
        }
    }

    private void SetToysOnSpawners() {
        ClearListOfToys();
        foreach (var toy in _toys ) {
            int randomIndex = Random.Range(0, _spawners.Length);
            _createdToys.Add(GameObject.Instantiate(toy, _spawners[randomIndex].position, Quaternion.identity));
        }
    }

    private void ClearListOfToys() {
        foreach (var toy in _createdToys) {
            GameObject.Destroy(toy);
        }
        _createdToys.Clear();
        _toysList.Clear();
    }

    private void OnDisable() {
        EventsSystem.IsNewGame -= SetToysOnSpawners;
    }
}
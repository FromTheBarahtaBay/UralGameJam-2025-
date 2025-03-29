using UnityEngine;

public class EnemyTrigger
{
    private Transform _triggerTransform;
    private GameObject _enemyTransform;
    private float _timer;
    private LayerMask _layerMask;
    private bool _isPlayerWasFind = false;

    public EnemyTrigger (Bootstrap bootstrap) {
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead;
        _triggerTransform = bootstrap.EnemyNeckData.Trigger.transform;
        _layerMask = LayerMask.GetMask("Player");
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void OnFixedUpdate() {
        if (_isPlayerWasFind) return;
        HandleTrigger();
    }

    private void HandleTrigger() {
        if (Time.time - _timer < .5f) return;
        Collider2D[] hits = Physics2D.OverlapBoxAll(_triggerTransform.position, new Vector2(10f, 11f), 0f, _layerMask);
        foreach (var hit in hits) {
            if (hit.CompareTag("Player")) {
                _enemyTransform.SetActive(true);
                _isPlayerWasFind = true;
            }
        }
        _timer = Time.time;
    }
}
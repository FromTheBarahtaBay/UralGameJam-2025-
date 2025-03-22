using UnityEngine;

public class MouseTracker {
    private Transform _playerTransform;
    private Transform _mousePointPosition;
    private Camera _camera;
    private SpriteRenderer _mouseSpriteRenderer;
    private Vector3 _offsetForTracker = new Vector3(0, 0, 10);

    private readonly float _rangeForInteraction;
    private bool _isDragging = false;
    private Vector3 _offsetForDraggingObject = Vector3.zero;
    private Rigidbody2D _rigidbody2DOfDraggingObject;

    public MouseTracker(Bootstrap bootstrap) {

        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);
        CreateMousePoint(bootstrap);

        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _camera = bootstrap.GameData.Camera;
        _rangeForInteraction = bootstrap.GameData.RangeForInteraction;

        Cursor.visible = false;
    }

    private void OnUpdate() {

        MoveMousePoint();
    }
    
    private void OnFixedUpdate() {
        MoveObject();
    }

    private void CreateMousePoint (Bootstrap bootstrap) {

        var mousePoint = new GameObject("MousePoint");
        mousePoint.layer = 8;

        var mousePointSpriteRenderer = mousePoint.AddComponent<SpriteRenderer>();
        _mouseSpriteRenderer = mousePointSpriteRenderer;
        _mouseSpriteRenderer.sortingOrder = 1001;
        mousePointSpriteRenderer.sprite = bootstrap.GameData.MouseImage;
        _mousePointPosition = mousePoint.transform;
    }

    private void MoveMousePoint() {
        _mousePointPosition.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offsetForTracker;
        ClickForInteraction();
    }

    private void ClickForInteraction() {

        float distanceMousePlayer = Vector2.Distance(_mousePointPosition.position, _playerTransform.position);
        bool isNotFar = distanceMousePlayer < _rangeForInteraction;

        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (isNotFar && hit.collider != null) {
                Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                if (rb != null && rb.tag != "Player" && rb.bodyType != RigidbodyType2D.Static) {
                    _isDragging = true;
                    _offsetForDraggingObject = _camera.ScreenToWorldPoint(Input.mousePosition) - rb.transform.position;
                    _rigidbody2DOfDraggingObject = rb;
                    Debug.Log($"Gamer takes - {rb.name}");
                }
            }
        } else if (Input.GetMouseButtonUp(0)) {
            StopDragging();
            return;
        }

        if (_isDragging && isNotFar) {
            float distancePlayerObject = Vector2.Distance(_rigidbody2DOfDraggingObject.transform.position + _offsetForDraggingObject, _playerTransform.position);
            if (distancePlayerObject > _rangeForInteraction)
                _isDragging = false;
            _mouseSpriteRenderer.color = Color.green;
        } else {
            StopDragging();
            ChangeTrackerColor(isNotFar);
        }
    }

    private void MoveObject() {
        if (!_isDragging) return;
        if (_rigidbody2DOfDraggingObject != null) {

            Vector3 targetPosition = _camera.ScreenToWorldPoint(Input.mousePosition) - _offsetForDraggingObject;
            Vector2 direction = (targetPosition - _rigidbody2DOfDraggingObject.transform.position);

            if (direction.magnitude < 0.1f) {
                _rigidbody2DOfDraggingObject.velocity = Vector2.zero;
            } else
                _rigidbody2DOfDraggingObject.velocity = direction * 1.5f;
        }
    }

    private void StopDragging() {
        if (_rigidbody2DOfDraggingObject != null) {
            _rigidbody2DOfDraggingObject.velocity = Vector2.zero;
            _rigidbody2DOfDraggingObject = null;
        }
        _isDragging = false;
        _mouseSpriteRenderer.color = Color.white;
    }

    private void ChangeTrackerColor(bool isNotFar) {

        if (isNotFar) {
            _mouseSpriteRenderer.color = Color.white;
        } else
            _mouseSpriteRenderer.color = Color.gray;
    }
}
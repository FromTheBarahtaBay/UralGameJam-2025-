using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MouseTracker {
    private Transform _playerTransform;
    private Transform _mousePointPosition;
    private Camera _camera;
    private SpriteRenderer _mouseSpriteRenderer;
    private Vector3 _offsetForTracker = new Vector3(0, 0, -10);

    private readonly float _rangeForInteraction;
    private bool _isDragging = false;
    private bool _isHolding = false;
    private Vector3 _offsetForDraggingObject = Vector3.zero;
    private Rigidbody2D _rigidbody2DOfDraggingObject;
    private NavMeshObstacle _navMeshObstacle;

    public MouseTracker(Bootstrap bootstrap) {

        bootstrap.AddActionToList(OnUpdate, true);
        bootstrap.AddActionToList(OnFixedUpdate, false);
        CreateMousePoint(bootstrap);

        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _camera = bootstrap.GameData.Camera;
        _rangeForInteraction = bootstrap.GameData.RangeForInteraction;
    }

    private void OnUpdate() {

        MoveMousePoint();
    }
    
    private void OnFixedUpdate() {
        MoveObject();
    }

    private void CreateMousePoint (Bootstrap bootstrap) {

        var mousePoint = new GameObject("MousePoint");
        mousePoint.layer = 13;
        mousePoint.transform.localScale = Vector2.one * 1.5f;

        var mousePointSpriteRenderer = mousePoint.AddComponent<SpriteRenderer>();
        _mouseSpriteRenderer = mousePointSpriteRenderer;
        _mouseSpriteRenderer.sortingOrder = 3001;
        mousePointSpriteRenderer.sprite = bootstrap.GameData.MouseImage;
        _mousePointPosition = mousePoint.transform;
    }

    private void MoveMousePoint() {
        _mousePointPosition.position = CurrentMousePosition();
        ClickForInteraction();
    }

    private Vector3 CurrentMousePosition() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_camera.transform.position.z);
        return _camera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void ClickForInteraction() {

        float distanceMousePlayer = Vector2.Distance(_mousePointPosition.position, _playerTransform.position);
        bool isNotFar = distanceMousePlayer < _rangeForInteraction;

        bool leftButton = Input.GetMouseButtonDown(0);
        bool rightButton = Input.GetMouseButtonDown(1);

        if (leftButton || rightButton) {
            Vector2 mousePos = CurrentMousePosition();
            //RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, .4f);
            Collider2D nearestHit = hits.OrderBy(h => Vector2.Distance(mousePos, h.transform.position)).FirstOrDefault();

            if (isNotFar && nearestHit != null) {

                Rigidbody2D rb = nearestHit.GetComponent<Rigidbody2D>();

                if (rb != null) {

                    if (rightButton)
                        if (rb.tag == "Door") {
                            if (rb.TryGetComponent<NavMeshObstacle>(out _navMeshObstacle)) {
                                _isHolding = true;
                                _navMeshObstacle.carving = true;
                                _rigidbody2DOfDraggingObject = rb;
                                rb.velocity = Vector3.zero;
                                rb.freezeRotation = true;
                            }
                        }

                    if (leftButton)
                        if (rb.tag != "Player" && rb.bodyType != RigidbodyType2D.Static) {
                            _isDragging = true;
                            _offsetForDraggingObject = CurrentMousePosition() - rb.transform.position;
                            _rigidbody2DOfDraggingObject = rb;
                        }
                }
            }
        } else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
            StopDragging();
            return;
        }

        if ((_isDragging || _isHolding) && isNotFar) {
            float distancePlayerObject = Vector2.Distance(_rigidbody2DOfDraggingObject.transform.position + _offsetForDraggingObject, _playerTransform.position);
            if (distancePlayerObject > _rangeForInteraction) {
                _isDragging = false;
                _isHolding = false;
            }
            if (_isDragging)
                _mouseSpriteRenderer.color = Color.green;
            else if (_isHolding)
                _mouseSpriteRenderer.color = Color.red;
        } else {
            StopDragging();
            ChangeTrackerColor(isNotFar);
        }
    }

    private void MoveObject() {
        if (!_isDragging) return;
        if (_rigidbody2DOfDraggingObject != null) {

            Vector3 targetPosition = CurrentMousePosition() - _offsetForDraggingObject;
            Vector2 direction = (targetPosition - _rigidbody2DOfDraggingObject.transform.position);

            if (direction.magnitude < 0.05f) {
                _rigidbody2DOfDraggingObject.velocity = Vector2.zero;
            } else
                _rigidbody2DOfDraggingObject.velocity = direction * 1.3f;
        }
    }

    private void StopDragging() {
        if (_rigidbody2DOfDraggingObject != null) {
            _rigidbody2DOfDraggingObject.velocity = Vector2.zero;
            _rigidbody2DOfDraggingObject.freezeRotation = false;
            _rigidbody2DOfDraggingObject = null;
        }
        if (_navMeshObstacle != null) {
            _navMeshObstacle.carving = false;
            _navMeshObstacle = null;
        }
        _isDragging = false;
        _isHolding = false;
        _mouseSpriteRenderer.color = Color.white;
    }

    private void ChangeTrackerColor(bool isNotFar) {

        if (isNotFar) {
            _mouseSpriteRenderer.color = Color.white;
        } else
            _mouseSpriteRenderer.color = Color.gray;
    }
}
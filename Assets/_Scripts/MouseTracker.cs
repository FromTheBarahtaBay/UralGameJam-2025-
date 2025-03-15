using UnityEngine;

public class MouseTracker {
    private Transform _mousePointPosition;
    private Camera _camera;
    private Vector3 _offset = new Vector3(0, 0, 10);

    public MouseTracker(Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnUpdate, true);
        CreateMousePoint(bootstrap);
        _camera = bootstrap.GameData.Camera;
        Cursor.visible = false;
    }

    private void OnUpdate() {
        MoveMousePoint();
    }

    private void CreateMousePoint (Bootstrap bootstrap) {
        var mousePoint = new GameObject("MousePoint");
        mousePoint.layer = 7;
        var mousePointSpriteRenderer = mousePoint.AddComponent<SpriteRenderer>();
        mousePointSpriteRenderer.sprite = bootstrap.GameData.MouseImage;
        _mousePointPosition = mousePoint.transform;
    }

    private void MoveMousePoint() {
        _mousePointPosition.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
    }
}
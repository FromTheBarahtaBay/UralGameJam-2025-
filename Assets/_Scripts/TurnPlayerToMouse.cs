using UnityEngine;

public class TurnPlayerToMouse
{
    private Transform _playerTransform;
    private Camera _camera;

    public TurnPlayerToMouse(Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnUpdate, true);
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _camera = bootstrap.GameData.Camera;
    }

    private void OnUpdate()
    {
        TurnToMouse();
    }

    private Vector3 MoveMousePoint() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_camera.transform.position.z);
        return _camera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void TurnToMouse() {

        Vector3 mousePosition = MoveMousePoint();

        mousePosition.z = _playerTransform.position.z;

        Vector3 direction = (mousePosition - _playerTransform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        _playerTransform.rotation = Quaternion.RotateTowards(_playerTransform.rotation, targetRotation, 360f * 2f * Time.deltaTime);
    }
}
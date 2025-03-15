using UnityEngine;

public class TurnPlayerToMouse
{
    private Transform _playerTransform;
    private Camera _camera;

    public TurnPlayerToMouse(Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnUpdate, true);
        _playerTransform = bootstrap.GameData.Player.transform;
        _camera = bootstrap.GameData.Camera;
    }

    private void OnUpdate()
    {
        TurnToMouse();
    }

    private void TurnToMouse() {

        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = _playerTransform.position.z;

        Vector3 direction = (mousePosition - _playerTransform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        _playerTransform.rotation = Quaternion.RotateTowards(_playerTransform.rotation, targetRotation, 360f * 2f * Time.deltaTime);
    }
}
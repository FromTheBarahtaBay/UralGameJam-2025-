using UnityEngine;

public class CameraMove
{
    private Transform _cameraTransform;
    private Transform _playerTransform;

    public CameraMove (Bootstrap bootstrap) {
        bootstrap.AddActionToList(OnFixedUpdate, false);
        _cameraTransform = bootstrap.GameData.Camera.transform;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
    }

    private void OnFixedUpdate() {
        Vector3 newPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _cameraTransform.position.z);
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, newPosition, 10f * Time.deltaTime);
    }
}
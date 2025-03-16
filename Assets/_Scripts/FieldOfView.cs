using UnityEngine;

public class FieldOfView
{
    private Mesh _mesh;
    private LayerMask _layerMask;
    private Vector3 _origin;
    private float _startingAngle;
    private float _fov = 90f;
    private int _rayCount = 50;
    private float _viewDIstance = 10f;

    private Transform _playerTransform;
    private GameObject _fieldOfView;
    private Camera _camera;

    public FieldOfView(Bootstrap bootstrap) {
        _mesh = new Mesh();
        _layerMask = bootstrap.GameData.LayerMaskWalls;
        _playerTransform = bootstrap.GameData.Player.transform;
        _camera = bootstrap.GameData.Camera;
        _rayCount = bootstrap.GameData.RayCount;
        _viewDIstance = bootstrap.GameData.ViewDistance;
        _fov = bootstrap.GameData.Fov;
        _origin = Vector3.zero;
        CreateFieldOfViewObject(bootstrap);
        bootstrap.AddActionToList(OnUpdate, true);
    }

    private void OnUpdate() {
        var aimDir = AimDirection();
        SetAimDirection(aimDir);
        SetOrigin(_playerTransform.position);
        CreateMeshOfView();
        //_fieldOfView.transform.position = _playerTransform.position;
    }

    private void CreateFieldOfViewObject(Bootstrap bootstrap) {
        GameObject fieldOfView = new GameObject("FieldOfView");
        _fieldOfView = fieldOfView;
        fieldOfView.layer = 7;
        fieldOfView.transform.position = Vector3.zero;
        fieldOfView.transform.SetParent(null);
        var meshFilter = fieldOfView.AddComponent<MeshFilter>();
        var meshRenderer = fieldOfView.AddComponent<MeshRenderer>();
        meshRenderer.material = bootstrap.GameData.Material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.allowOcclusionWhenDynamic = false;

        meshFilter.mesh = _mesh;
    }

    private void CreateMeshOfView() {

        float angle = _startingAngle + 100f;
        float angleIncrease = _fov / _rayCount;

        Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        vertices[0] = _origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= _rayCount; i++) {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_origin, GetVectorFromAngle(angle), _viewDIstance, _layerMask);
            if (raycastHit2D.collider == null) {
                vertex = _origin + GetVectorFromAngle(angle) * _viewDIstance;
            } else {
                vertex = raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
        _mesh.RecalculateBounds();
    }

    private Vector3 GetVectorFromAngle(float angle) {
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private void SetOrigin(Vector3 origin) {
        _origin = origin;
    }

    private void SetAimDirection(Vector3 aimDirection) {
        _startingAngle = GetAngleFromVectorFloat(aimDirection) - _fov / 2f;
    }

    private Vector3 AimDirection() {

        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _playerTransform.position.z;

        return (mousePosition - _playerTransform.position).normalized;
    }

    private float GetAngleFromVectorFloat(Vector3 dir) {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
using UnityEngine;

public class PlayerIKController {
    private Transform[] _bodyparts;
    private Transform[] _targets;
    private Vector3[] _currentPositions;
    private Transform _head;
    private Transform _torso;
    private Transform[] _arms;
    private Transform _targetsObj;
    private Quaternion[] _defaultRotation;
    private float _moveDistance = 1.7f;
    private float _speedAnimation = 20f;
    private Camera _camera;
    private int _currentStep = 1;
    private Vector3 _startHeadPosition;
    int _inverse = 1;

    public PlayerIKController(Bootstrap bootstrap) {
        _bodyparts = bootstrap.GameData.Bodyparts;
        _targets = bootstrap.GameData.Targets;
        _currentPositions = new Vector3[_bodyparts.Length];
        _head = bootstrap.GameData.Head;
        _torso = bootstrap.GameData.Torso;
        _arms = bootstrap.GameData.ArmBones;
        _targetsObj = bootstrap.GameData.targetsObj;
        _camera = bootstrap.GameData.Camera;
        _startHeadPosition = _head.localPosition;
        SetDefaultRotations();
        bootstrap.AddActionToList(OnUpdate);
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void OnUpdate() {
        CheckInputs();
        MoveLimbs();
    }

    private void OnFixedUpdate() {

    }

    private void MoveLimbs() {

        for (int i = 0; i < _bodyparts.Length; i++) {

            _bodyparts[i].position = Vector3.Lerp(_bodyparts[i].position, _currentPositions[i], Time.deltaTime * _speedAnimation);

            if (i == _currentStep) continue;

            float distance = Vector3.Distance(_currentPositions[i], _targets[i].position);

            if (distance > _moveDistance) {
                _currentPositions[i] = new Vector2(_targets[i].position.x, _targets[i].position.y) + Random.insideUnitCircle * Random.Range(0, 0.5f);
                if (i == 0 ) _inverse *= -1;
            }

            if (Vector2.Distance(_bodyparts[i].position, _currentPositions[i]) < 0.3) {
                _currentStep = (_currentStep + 1) % _bodyparts.Length;
                //_inverse *= -1;
            } 
        }
    }

    private void ReturnLegsToDefault() {
        _targetsObj.localPosition = Vector3.zero;
        for (int i = 0; i < _bodyparts.Length; i++) {
            _currentPositions[i] = _targets[i].position;
        }
    }

    private void CheckInputs() {
        float horizontal = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 (A, D)
        float vertical = Input.GetAxisRaw("Vertical"); // -1, 0, 1 (S, W)

        if (horizontal == 0 && vertical == 0) {
            _head.localPosition = Vector3.Lerp(_head.localPosition, _startHeadPosition, 5f * Time.deltaTime);
            ReturnLegsToDefault();
            return; // Никакого движения – ничего не делаем
        }

        RotateArms();
        RotateTorso();

        // Получаем направление к курсору
        Vector2 mousePos = MoveMousePoint();
        Vector2 playerPos = _torso.position;
        Vector2 lookDirection = (mousePos - playerPos).normalized;

        // Направление ввода (движения)
        Vector2 inputDirection = new Vector2(horizontal, vertical).normalized;

        float dot = Vector2.Dot(lookDirection, inputDirection);

        if (dot > 0) {
            Vector3 offset = _startHeadPosition + new Vector3(-0.1f, 0, 0);
            _head.localPosition = Vector3.Lerp(_head.localPosition, offset, 10f * Time.deltaTime);
        } else {
            Vector3 offset = new Vector3(0.1f, 0, 0);
            _head.localPosition = Vector3.Lerp(_head.localPosition, 5 * offset, 10f * Time.deltaTime);
        }

        // Вычисляем угол между направлением взгляда и направлением ввода
        float angle = Vector2.SignedAngle(lookDirection, inputDirection);

        // Смещаем _targetsObj в направлении inputDirection на заданное расстояние
        float offsetDistance = _moveDistance / 2f; // Можно менять для подстройки
        _targetsObj.localPosition = _torso.localPosition + new Vector3(0, -.1f, 0) + (Vector3)(Quaternion.Euler(0, 0, angle) * Vector2.up * offsetDistance);
    }

    private Vector3 MoveMousePoint() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_camera.transform.position.z);
        return _camera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void RotateArms() {

        float magnitude = 20f;

        if (Input.GetKey(KeyCode.LeftShift)) {
            magnitude = 40;
        }

        for (int i = 0; i < _arms.Length; i++) {

            var targetAngle = _defaultRotation[i] * Quaternion.Euler(0, magnitude * ((i % 2 == 0) ? _inverse : -_inverse), 0);

            _arms[i].localRotation = Quaternion.Lerp(_arms[i].localRotation, targetAngle, Time.deltaTime * _speedAnimation);
        }
    }

    private void RotateTorso() {
        int gradus = 6;

        if (Input.GetKey(KeyCode.LeftShift)) {
            gradus = 20;
        }

        var targetAngle = Quaternion.Euler(0, 0, gradus * _inverse);
        _torso.localRotation = Quaternion.Lerp(_torso.localRotation, targetAngle, Time.deltaTime * 5);

        var targetAngleHead = Quaternion.Euler(0, 0, -gradus * _inverse);
        _head.localRotation = Quaternion.Lerp(_head.localRotation, targetAngleHead, Time.deltaTime * 5);
    }

    private void SetDefaultRotations() {
        _defaultRotation = new Quaternion[_arms.Length];
        for (int i = 0; i < _arms.Length; i++) {
            _defaultRotation[i] = _arms[i].localRotation;
        }
    }
}
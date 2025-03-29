using UnityEngine;

public class NeckBossIKController
{
    private Transform[] _bodyparts;
    private Transform[] _targets;
    private Vector3[] _currentPositions;
    private Transform[] _spineBones;
    private Quaternion[] _defaultRotation;
    private float _moveDistance = 1.1f;
    private float _magnitude = 50;
    private float _speedAnimation = 25f;
    private int _inverse = -1;
    private Transform _playerTransform;
    private Transform _enemyTransform;
    private float _distanceToHEar = 100f;
    public AudioSource _audioSource;
    private AudioClip[] _soundClips;

    public NeckBossIKController(Bootstrap bootstrap) {
        _bodyparts = bootstrap.EnemyNeckData.Bodyparts;
        _targets = bootstrap.EnemyNeckData.Targets;
        _currentPositions = new Vector3[_targets.Length];
        _spineBones = bootstrap.EnemyNeckData.SpineBones;
        _magnitude = bootstrap.EnemyNeckData.Magnitude;
        _moveDistance = bootstrap.EnemyNeckData.MoveDistance;
        _speedAnimation = bootstrap.EnemyNeckData.SpeedAnimation;
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _enemyTransform = bootstrap.EnemyNeckData.EnemyHead.transform;
        _audioSource = bootstrap.GameData.AudioSource;
        _soundClips = bootstrap.EnemyNeckData.StepsAudio;

        SetDefaultRotations();
        bootstrap.AddActionToList(OnFixedUpdate, false);
    }

    private void SetDefaultRotations() {
        _defaultRotation = new Quaternion[_spineBones.Length];
        for (int i = 0; i < _spineBones.Length; i++) {
            _defaultRotation[i] = _spineBones[i].localRotation;
        }
    }

    private void OnFixedUpdate() {
        MoveLimbs();
        RotateTorso();
    }

    private void MoveLimbs() {

        for (int i = 0; i < _bodyparts.Length; i++) {

            float distance = Vector3.Distance(_currentPositions[i], _targets[i].position);

            if (distance > _moveDistance) {
                _currentPositions[i] = new Vector2(_targets[i].position.x, _targets[i].position.y) + Random.insideUnitCircle * Random.Range(0, .3f);
                if (i == 0) _inverse *= -1;
                MakeNoise();
            }

            _bodyparts[i].position = Vector3.Lerp(_bodyparts[i].position, _currentPositions[i], Time.deltaTime * _speedAnimation);
        }
    }

    private void RotateTorso() {
        for (int i = 0; i < _spineBones.Length; i++) {

            var targetAngle = _defaultRotation[i] * Quaternion.Euler(0, 0, _magnitude * ((i % 2 == 0) ? _inverse  : -_inverse * 2));

            _spineBones[i].localRotation = Quaternion.Lerp(_spineBones[i].localRotation, targetAngle, Time.deltaTime * _speedAnimation / 5);
        }
    }

    private void MakeNoise() {
        float distance = Vector2.Distance(_enemyTransform.position, _playerTransform.position);
        float volume = Mathf.Clamp01(.2f - (distance / _distanceToHEar)); // Чем ближе, тем громче

        _audioSource.volume = volume;

        PlayRandomSound();
    }

    private void PlayRandomSound() {
        if (_soundClips.Length == 0) return;

        int randomIndex = Random.Range(0, _soundClips.Length);
        _audioSource.PlayOneShot(_soundClips[randomIndex]);
    }
}
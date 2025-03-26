using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHealthSystem
{
    private Sequence _sequence;
    private CanvasGroup _canvasAlarm;
    private const float _playerDamageMax = 1f;
    private float _currentDamage;

    public PlayerHealthSystem(Bootstrap bootstrap) {
        EventsSystem.IsPlayerHited += PlayerHealthDecrease;
        bootstrap.AddActionOnDisable(OnDisable);
        _currentDamage = 0;
        _canvasAlarm = bootstrap.GameData.CanvasAlarm;
    }

    private void PlayerHealthDecrease() {
        float addDamage = Random.Range(.2f, .25f);
        PlayAnimationAlarmDown(addDamage);
        if (_currentDamage >= _playerDamageMax) {
            EventsSystem.OnIsPlayerDead();
        }
    }

    private void PlayAnimationAlarmDown(float damage) {
        KillSequence();

        _sequence = DOTween.Sequence();

        float targetAlpha = Mathf.Clamp(_currentDamage + damage, 0, 1f);
        float oldValue = _currentDamage;
        _currentDamage = targetAlpha;

        _sequence
            .Append(_canvasAlarm.DOFade(targetAlpha, .3f).From(oldValue))
            .OnComplete(() => { PlayAnimationAlarmUp(); })
            .Play()
            .SetAutoKill(true);
    }

    private void PlayAnimationAlarmUp() {
        KillSequence();

        _sequence = DOTween.Sequence();

        float duration = _currentDamage * 15;

        _sequence
            .Append(_canvasAlarm.DOFade(0, duration))
            .OnUpdate(() => {
                _currentDamage = Mathf.Lerp(_currentDamage, 0, Time.deltaTime / duration);
            })
            .Play()
            .OnComplete(() => {  })
            .SetAutoKill(true);
    }

    private void KillSequence() {
        if (_sequence.IsActive() && _sequence.IsPlaying())
            _sequence.Kill();
    }

    private void OnDisable() {
        EventsSystem.IsPlayerHited -= PlayerHealthDecrease;
    }
}
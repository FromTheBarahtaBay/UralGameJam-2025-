using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIController
{
    private CanvasGroup _canvasCommon;
    private CanvasGroup _canvasGroupMenu;
    private CanvasGroup _canvasRotating;
    private CanvasGroup _canvasGameOver;
    private CanvasGroup _canvasGameWin;
    private RectTransform _canvasMenuRectTrans;
    private Sequence _sequenceRoration;
    private Sequence _sequenceAnim;
    private RectTransform _panelLoading;
    private bool _isOpen = false;
    private bool _isExit = false;
    private bool _isGame = false;
    private GameData _gameData;
    private Button _startButton;
    private Button _exitButton;
    private Button _mainMenuButton;
    private Button _goTOMenuButtonWin;
    private RectTransform _header;
    private Bootstrap _bootstrap;
    private Button _goToMenuButton;
    private TextMeshProUGUI _textCountToys;
    private AudioSource _audioSource;
    private AudioClip _audioClip;

    public UIController(Bootstrap bootstrap) {
        _bootstrap = bootstrap;
        _canvasCommon = bootstrap.GameData.CanvasCommon;
        _canvasRotating = bootstrap.GameData.CanvasRotating;
        _canvasGroupMenu = bootstrap.GameData.CanvasMenu;
        _canvasMenuRectTrans = _canvasGroupMenu.gameObject.GetComponent<RectTransform>();
        _canvasGameOver = bootstrap.GameData.CanvasGameOver;
        _panelLoading = bootstrap.GameData.PanelLoad;
        _startButton = bootstrap.GameData.StartButton;
        _exitButton = bootstrap.GameData.ExitButton;
        _mainMenuButton = bootstrap.GameData.MainMenuButton;
        _header = bootstrap.GameData.Header;
        _gameData = bootstrap.GameData;
        _goToMenuButton = bootstrap.GameData.GoTOMenuButton;
        _canvasGameWin = bootstrap.GameData.CanvasGameWin;
        _goTOMenuButtonWin = bootstrap.GameData.GoTOMenuButtonWin;
        _textCountToys = bootstrap.GameData.ToysCount;
        _audioSource = bootstrap.GameData.AudioSource;
        _audioClip = bootstrap.GameData.MusicEnterence;

        _startButton.onClick.AddListener(StartGame);
        _exitButton.onClick.AddListener(ExitGame);
        _mainMenuButton.onClick.AddListener(ReturnToMenu);
        _goToMenuButton.onClick.AddListener(GoToMenu);
        _goTOMenuButtonWin.onClick.AddListener(ReturnToMenu);

        EventsSystem.IsPlayerDead += RunGameOver;
        EventsSystem.IsPlayerWin += RunGameWin;
        EventsSystem.IsToyFind += IncreesCounterOfToys;
        bootstrap.AddActionOnDisable(OnDisable);

        RotateLoadingScene();
    }

    public void Init() {
        RunAnimationMenu();
        PlayMusic(true);
    }

    private void PlayMusic(bool value) {
        if (value) {
            _audioSource.loop = true;
            _audioSource.clip = _audioClip;
            _audioSource.Play();
        }
        else {
            _audioSource.loop = false;
            _audioSource.clip = null;
            _audioSource.Stop();
        }
            
    }

    private void RotateLoadingScene() {

        _sequenceRoration = DOTween.Sequence();

        _sequenceRoration
            .Append(_panelLoading.DORotate(new Vector3(0, 0, -360), 40f, RotateMode.FastBeyond360).From(Vector3.zero).SetEase(Ease.Linear))
            .Play()
            .SetLoops(-1, LoopType.Restart)
            .OnUpdate(CheckObjectState)
            .SetAutoKill(false);
    }

    private void CheckObjectState() {
        if (_panelLoading == null || !(_panelLoading as Transform).gameObject.activeInHierarchy) {
            _sequenceRoration.Kill();
        }
    }

    private void RunAnimationMenu() {
        _isOpen = !_isOpen;

        KillAnimation();
        

        float duration = 1f;
        _startButton.interactable = _isOpen;
        _exitButton.interactable = _isOpen;

        _canvasGameWin.gameObject.SetActive(false);
        _sequenceAnim = DOTween.Sequence();

        if (_isOpen) {
            _sequenceAnim
                .Append(_canvasGroupMenu.DOFade(1, duration).From(0))
                .Join(_canvasMenuRectTrans.DOScale(1, duration).From(0))
                .Append(_startButton.transform.DOScale(1, duration / 5).From(0))
                .Append(_exitButton.transform.DOScale(1, duration / 5).From(0))
                .AppendInterval(.1f)
                .Append(_header.DOScale(1, duration / 5).From(0))
                .JoinCallback(() => Cursor.visible = true);
        } else {
            _sequenceAnim
                .Append(_header.DOScale(0, duration / 5).From(1))
                .Append(_exitButton.transform.DOScale(0, duration / 5).From(1))
                .Append(_startButton.transform.DOScale(0, duration / 5).From(1))
                .Append(_canvasMenuRectTrans.DOScale(0, duration).From(1))
                .Join(_canvasGroupMenu.DOFade(0, duration).From(1))
                .AppendInterval(.2f)
                .Append(_canvasRotating.DOFade(0, 3f).From(1))
                .AppendCallback(() => _bootstrap.RunGame());
        }

        _sequenceAnim
            .SetEase(Ease.Linear)
            .Play()
            .OnComplete(() => {
                if (_isExit) Quit();
                if (_isGame) {
                    PlayMusic(false);
                    Cursor.visible = false;
                    _isGame = false;
                    _canvasRotating.gameObject.SetActive(_isOpen);
                    
                }
            })
            .SetAutoKill(true);
    }

    private void KillAnimation() {
        if (_sequenceAnim.IsActive() && _sequenceAnim.IsPlaying()) {
            _sequenceAnim.Kill();
        }
    }

    private void StartGame() {
        _isGame = true;
        EventsSystem.OnIsNewGame();
        RunAnimationMenu();
    }

    private void ExitGame() {
        _isExit = true;
        RunAnimationMenu();
    }

    private void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RunGameOver() {
        KillAnimation();
        _bootstrap.PauseGame();
        _canvasGameOver.gameObject.SetActive(true);
        Cursor.visible = true;

        _sequenceAnim = DOTween.Sequence();

        _sequenceAnim
            .Append(_canvasGameOver.DOFade(1, 1f).From(0))
            .Play()
            .OnComplete(() => { _bootstrap.PauseGame(); })
            .SetAutoKill(true);
    }

    private void ReturnToMenu() {
        KillAnimation();

        _canvasRotating.gameObject.SetActive(true);
        RotateLoadingScene();
        PlayMusic(true);
        _canvasRotating.alpha = 1f;

        _sequenceAnim = DOTween.Sequence();

        _sequenceAnim
            .Append(_canvasGameOver.DOFade(0, .5f).From(1))
            .Play()
            .OnComplete(() => { RunAnimationMenu(); })
            .SetAutoKill(true);
    }

    private void OnDisable() {
        EventsSystem.IsPlayerDead -= RunGameOver;
        EventsSystem.IsPlayerWin -= RunGameWin;
        EventsSystem.IsToyFind -= IncreesCounterOfToys;
    }

    private void GoToMenu() {
        KillAnimation();
        _bootstrap.PauseGame();
        _sequenceAnim = DOTween.Sequence();

        _sequenceAnim
            .Append(_goToMenuButton.transform.DORotate(new Vector3(0, 0, -72), 1f, RotateMode.FastBeyond360).From(Vector3.zero).SetEase(Ease.Linear))
            .Play()
            .OnComplete(() => { ReturnToMenu(); })
            .SetAutoKill(true);
    }

    private void RunGameWin() {
        KillAnimation();
        _bootstrap.PauseGame();
        _canvasGameWin.gameObject.SetActive(true);
        Cursor.visible = true;

        _sequenceAnim = DOTween.Sequence();

        _sequenceAnim
            .Append(_canvasGameWin.DOFade(1, 1f).From(0))
            .Play()
            .OnComplete(() => { _bootstrap.PauseGame(); })
            .SetAutoKill(true);
    }

    private void IncreesCounterOfToys(int value) {
        _textCountToys.text = $"{value}";
    }
}
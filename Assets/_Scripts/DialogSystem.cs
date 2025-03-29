using System.Collections;
using TMPro;
using UnityEngine;

public class DialogSystem 
{
    private Transform _playerTransform;
    private GameObject _horn;
    private CanvasGroup _dialogBubble;
    private TextMeshProUGUI _textField;
    private float _typingSpeed = 0.05f;

    private string[] _sentences;
    private int _currentSentenceIndex = 0;
    private bool _isTyping = false;
    private IEnumerator _currentCorutine;
    private Bootstrap _bootstrap;
    private Camera _camera;
    RectTransform _rectTransformBubble;
    private float _timer = 0f;
    private float _random;

    private string[] _tutorText = new string[] {
        " Пролистывать диалог E",
        " Ваня, просыпайся...",
        " Ты что-нибудь помнишь?",
        " Осмотрись мышкой.",
        " Передвижение WASD",
        " Пробегись-ка на Shift",
        " Пробел - это рывок",
        " Перетаскивай предметы зажав ЛКМ",
        " Индикатор будет гореть зелёным",
        " Ты можешь удерживать двери зажав ПКМ",
        " Найди футбольные мячи в окресностях и принести их к своей кровати",
        " А сейчас...",
        " Иди к восточному входу...",
        " Ваня, к тебе пришли."
    };

    private string[] _randomText = new string[] {
        " Я тут..",
        " Ты где?.",
        " Подойди..",
        " Осторожно...",
        " Он прячется?"
    };

    private void OnUpdate() {
        _timer += Time.deltaTime;
    }

    private Vector3 MoveMousePoint() {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_camera.transform.position.z);
        return _camera.ScreenToWorldPoint(mouseScreenPos);
    }

    public DialogSystem(Bootstrap bootstrap) {
        _playerTransform = bootstrap.GameData.PlayerBody.transform;
        _bootstrap = bootstrap;
        _horn = bootstrap.GameData.CanvasHorn;
        _dialogBubble = bootstrap.GameData.DialogBubble;
        _textField = bootstrap.GameData.TextField;
        _camera = bootstrap.GameData.Camera;
        _rectTransformBubble = _dialogBubble.GetComponent<RectTransform>();
        _random = Random.Range(10, 25);
        StartDialog(_tutorText);
        bootstrap.AddActionToList(OnUpdate);
    }

    public void StartDialog(string[] dialogSentences) {
        _dialogBubble.gameObject.SetActive(true);
        _sentences = dialogSentences;
        _currentSentenceIndex = 0;
        _currentCorutine = TypeSentence(_sentences[_currentSentenceIndex]);
        _bootstrap.StartCoroutine(_currentCorutine);
        _bootstrap.StartCoroutine(ClickListener());
    }

    private IEnumerator TypeSentence(string sentence) {
        RepositionHorn();
        _isTyping = true;
        _textField.text = "";
        _timer = 0f;

        foreach (char letter in sentence) {
            _textField.text += letter;
            ResizeBubble(_rectTransformBubble);
            yield return new WaitForSeconds(_typingSpeed);
        }
        _isTyping = false;

    }

    void ResizeBubble(RectTransform rectTransform) {
        Vector2 textSize = _textField.GetPreferredValues();
        Vector2 padding = new Vector2(0.5f, 0.1f);

        rectTransform.sizeDelta = textSize + padding;
    }

    private IEnumerator ClickListener() {
        while (true) {
            if ((Input.GetKeyDown(KeyCode.E)) || _timer > _random) {
                _timer = 0f;
                _random = Random.Range(10, 25);
                if (!_isTyping) {
                    if (_sentences != null && _currentSentenceIndex < _sentences.Length - 1) {
                        _currentSentenceIndex++;
                        //StopCoroutine(_currentCorutine);
                        _currentCorutine = TypeSentence(_sentences[_currentSentenceIndex]);
                        _bootstrap.StartCoroutine(_currentCorutine);
                    } else {
                        CloseDialog();
                        yield break;
                    }
                } else {
                    _isTyping = false;
                    _bootstrap.StopCoroutine(_currentCorutine);
                    _textField.text = _sentences[_currentSentenceIndex];
                    ResizeBubble(_rectTransformBubble);
                    RepositionHorn();
                }
            }
            yield return null;
        }
    }

    private void CloseDialog() {
        StartDialog(_randomText);
    }

    private void RepositionHorn() {
        // Получаем направление от игрока к мыши
        Vector3 lookDirection = (MoveMousePoint() - _playerTransform.position).normalized;

        // Разворачиваем вектор в противоположную сторону (за спину)
        Vector3 behindDirection = -lookDirection;

        // Случайное расстояние от 5 до 10
        float distance = Random.Range(5f, 10f);

        // Смещаем `horn` за спину игрока
        _horn.transform.position = _playerTransform.position + behindDirection * distance;
    }
}
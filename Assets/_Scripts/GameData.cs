using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData {

    [Header("Player")]
    public GameObject PlayerBody;
    public float SpeedToMove = 5f;
    [HideInInspector] public Rigidbody2D PlayerRigidbody2D;
    public float RangeForInteraction = 3.5f;
    public LayerMask PlayerMask;

    [Header("PlayerIK")]
    public Transform[] Bodyparts;
    public Transform targetsObj;
    public Transform[] Targets;
    public Transform Head;
    public Transform Torso;
    public Transform[] ArmBones;

    [Header("Camera")]
    public Camera Camera;
    public AudioSource AudioSource;

    [Header("UI (Loading)")]
    public CanvasGroup CanvasCommon;
    public CanvasGroup CanvasMenu;
    public CanvasGroup CanvasRotating;
    public CanvasGroup CanvasAlarm;
    public CanvasGroup CanvasGameOver;
    public CanvasGroup CanvasGameWin;
    public RectTransform PanelLoad;
    public float SpeedOfRotationUI;
    public Slider ProgressBar;
    public Button StartButton;
    public Button ExitButton;
    public Button MainMenuButton;
    public Button GoTOMenuButton;
    public Button GoTOMenuButtonWin;
    public RectTransform Header;
    public TextMeshProUGUI ToysCount;

    [Header("Mouse")]
    public Sprite MouseImage;

    [Header("FieldOfView")]
    public Material Material;
    public LayerMask LayerMaskWalls;
    public int RayCount = 50;
    public float ViewDistance = 12f;
    public float Fov = 100f;

    [Header("Game")]
    public Transform Trigger;
    public Transform[] Spawners;
    public GameObject[] Toys;

    [Header("Music")]
    public AudioClip MusicEnterence;

    [Header ("Dialog")]
    public GameObject CanvasHorn;
    public CanvasGroup DialogBubble;
    public TextMeshProUGUI TextField;

    public void TryFindNullFields(GameData gameData) {

        Type type = typeof(GameData);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields) {
            var fieldValue = field.GetValue(gameData);
            if (fieldValue == null) {
                Debug.LogError($"EXCEPTION: Field {field.Name} is null!");
            }
        }

        if (!PlayerBody.TryGetComponent<Rigidbody2D>(out PlayerRigidbody2D))
            Debug.LogError("EXEPTION: no Rigidbody2D on Player!");
    }
}
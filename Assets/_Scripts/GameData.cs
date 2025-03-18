using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData {

    [Header("Player")]
    public GameObject PlayerBody;
    public float SpeedToMove = 5f;
    [HideInInspector] public Rigidbody2D PlayerRigidbody2D;
    public float RangeForInteraction = 3f;

    [Header("Camera")]
    public Camera Camera;

    [Header("UI (Loading)")]
    public Canvas Canvas;
    public Slider ProgressBar;

    [Header("Mouse")]
    public Sprite MouseImage;

    [Header("FieldOfView")]
    public Material Material;
    public LayerMask LayerMaskWalls;
    public int RayCount = 50;
    public float ViewDistance = 12f;
    public float Fov = 90f;

    [Header("EnemyNeckBoss")]
    public GameObject EnemyHead;
    public GameObject EnemyBody;
    public Rigidbody2D EnemyRigidbody2D;
    public LineRenderer NeckLine;
    public GameObject TargetDir;
    public GameObject WiggleDir;


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
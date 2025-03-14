using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData {

    //[Header("Player")]
    public GameObject Player;

    //[Header("Camera")]
    public Camera Camera;

    //[Header("UI (Loading)")]
    public Canvas Canvas;
    public Slider ProgressBar;

    //[Header("Mouse")]
    public Sprite MouseImage;

    public void TryFindNullFields() {

        Type type = this.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields) {
            var fieldValue = field.GetValue(this);
            if (fieldValue == null) {
                Debug.Log("WORK!");
                throw new Exception($"EXCEPTION: Field {field.Name} is null!");
            }
        }
    }
}
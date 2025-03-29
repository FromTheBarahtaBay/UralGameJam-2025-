using UnityEngine;

[System.Serializable]
public class EnemyNeckData
{
    [Header("EnemyNeckBoss")]
    public GameObject EnemyHead;
    public GameObject EnemyBody;
    public Rigidbody2D EnemyRigidbody2D;
    public LineRenderer NeckLine;
    public GameObject TargetDir;
    public GameObject WiggleDir;

    [Header("IK Elements")]
    public Transform[] Bodyparts;
    public Transform[] Targets;
    public Transform[] SpineBones;
    public float Magnitude;
    public float MoveDistance;
    public float SpeedAnimation;

    [Header("States")]
    public EnemyStateSO[] States;

    [Header("Sounds")]
    public AudioClip[] StepsAudio;

    [Header("Trigger")]
    public GameObject Trigger;
}
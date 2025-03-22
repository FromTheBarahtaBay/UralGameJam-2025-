using UnityEngine;

[System.Serializable]
public class EnemyStateParameters
{
    public string TypeOfState;
    public Transform TargetForHead;
    public Transform TargetForBody;
    public float Speed;
    public bool IsAttacking;
    public bool InPeace;
}
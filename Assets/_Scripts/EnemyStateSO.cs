using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyState", menuName = "ScriptableObjects/Enemy/State", order = 1)]
public class EnemyStateSO : ScriptableObject
{
    public EnemyStateParameters EnemyStateParameters;
}
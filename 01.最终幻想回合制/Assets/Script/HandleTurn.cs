using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手柄转动
/// </summary>
[System.Serializable]
public class HandleTurn
{
    /// <summary>
    /// 攻击者的名称
    /// </summary>
    public string Attacker;

    /// <summary>
    /// 攻击者的物体
    /// </summary>
    public GameObject AttackersGameObject;

    /// <summary>
    /// 攻击者的目标
    /// </summary>
    public GameObject AttackersTarget;
}

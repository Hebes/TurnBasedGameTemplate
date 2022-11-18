using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回合处理类
/// </summary>
[System.Serializable]
public class HandleTurn
{
    /// <summary>
    /// 攻击者的名称
    /// </summary>
    [Header("攻击者的名称")]
    public string Attacker;

    /// <summary>
    /// 类型
    /// </summary>
    [Header("攻击者的种类:Enemy 敌人 Hero:英雄")]
    public string Type;

    /// <summary>
    /// 攻击者的物体
    /// </summary>
    [Header("发动的攻击者")]
    public GameObject AttackersGameObject;

    /// <summary>
    /// 攻击者的目标
    /// </summary>
    [Header("攻击者的目标")]
    public GameObject AttackersTarget;

    //which attack is performed
    /// <summary>
    /// 执行哪一种攻击
    /// </summary>
    [Header("攻击方式")]
    public BaseAttack choosenAttack;
}

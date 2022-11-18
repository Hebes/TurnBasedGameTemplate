using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack :MonoBehaviour
{
    /// <summary>
    /// 名称
    /// </summary>
    public string attackName;//名称
    /// <summary>
    /// 攻击描述
    /// </summary>
    public string attackDescription;//攻击描述
    /// <summary>
    /// 伤害
    /// </summary>
    public float attackDamage;//伤害 Base Damage 15，mellee LvL 10 stamina 35 = basedmg + stamina + LvL = 60
    /// <summary>
    /// 法力值消耗
    /// </summary>
    public float attackCost;//ManaCost 法力值消耗

}


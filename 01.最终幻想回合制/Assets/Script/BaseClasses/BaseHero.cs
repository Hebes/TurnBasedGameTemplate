using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BaseHero : BaseClass
{
    /// <summary>
    /// 耐力
    /// </summary>
    [Header("耐力")]
    public int stamina;

    /// <summary>
    /// 智力
    /// </summary>
    [Header("智力")]
    public int intellect;

    /// <summary>
    /// 灵巧
    /// </summary>
    [Header("灵巧")]
    public int dexterity;

    /// <summary>
    /// 敏捷
    /// </summary>
    [Header("敏捷")]
    public int agility;

    public List<BaseAttack> MagicAttack = new List<BaseAttack>();
}

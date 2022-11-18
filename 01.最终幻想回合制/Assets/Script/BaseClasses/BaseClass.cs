using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

    public float baseHP;
    public float curHP;

    public float BaseMP;
    public float curMP;

    public float baeATK;
    public float curAtk;

    public float baseDEF;
    public float surDEF;

    /// <summary>
    /// 攻击方式
    /// </summary>
    public List<BaseAttack> attacks = new List<BaseAttack>(); 
}

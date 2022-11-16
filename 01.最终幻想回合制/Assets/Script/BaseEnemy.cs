using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BaseEnemy
{
    public string name;

    public enum EType
    {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC,
    }

    public enum ERarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE,
    }

    public EType EnemyType;
    public ERarity rarity;

    public float baseHP;
    public float curHP;

    public float BaseMP;
    public float curMP;

    public float baeATK;
    public float curAtk;
    public float baseDEF;
    public float surDEF;
}

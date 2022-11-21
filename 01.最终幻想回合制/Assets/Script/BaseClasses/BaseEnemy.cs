using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BaseEnemy : BaseClass
{

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


}

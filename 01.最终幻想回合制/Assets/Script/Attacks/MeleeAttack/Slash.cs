using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 砍
/// </summary>
public class Slash : BaseAttack
{
    public Slash()
    {
        attackName = "Slash";
        attackDescription = "用你的武器快速砍击攻击。";
        attackDamage = 10f;
        attackCost = 0;
    }
}

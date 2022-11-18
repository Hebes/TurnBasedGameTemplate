using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 巨锤挥击
/// </summary>
public class HammerSwing :BaseAttack
{
    public HammerSwing()
    {
        attackName = "Hammer Swing";
        attackDescription = "这是一个强大的挥锤攻击。";
        attackDamage = 15f;
        attackCost = 0;
    }
}

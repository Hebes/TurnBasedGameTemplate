using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack magicAttackToPerform;

    /// <summary>
    /// Ê©Õ¹Ä§·¨¹¥»÷
    /// </summary>
    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>().Input4(magicAttackToPerform);
    }
}

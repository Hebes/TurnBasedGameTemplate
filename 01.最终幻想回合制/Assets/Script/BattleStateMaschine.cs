using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗状态机
/// </summary>
public class BattleStateMaschine : MonoBehaviour
{
    /// <summary>
    /// 执行操作
    /// </summary>
    public enum PerfromAction
    {
        /// <summary>
        /// 等待
        /// </summary>
        WAIT,
        /// <summary>
        /// 采取行动
        /// </summary>
        TAKEACTION,
        /// <summary>
        /// 执行动作
        /// </summary>
        PERFROMACTION,
    }

    /// <summary>
    /// 战斗的状态
    /// </summary>
    public PerfromAction battleState;

    /// <summary>
    /// 执行行动的列表
    /// </summary>
    public List<HandleTurn> PerformList = new List<HandleTurn>();
    /// <summary>
    /// 英雄战斗
    /// </summary>
    public List<GameObject> HerosInBattle = new List<GameObject>();
    /// <summary>
    /// 敌人战斗
    /// </summary>
    public List<GameObject> EnemyssInBattle = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerfromAction.WAIT;
        EnemyssInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case PerfromAction.WAIT:
                if (PerformList.Count>0)//行动的执行列表大于0的时候
                {
                    battleState = PerfromAction.TAKEACTION;
                }
                break;
            case PerfromAction.TAKEACTION:
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if (PerformList[0].Type.Equals("Enemy"))
                {
                    EnemyStateMaschine ESM = performer.GetComponent<EnemyStateMaschine>();
                    ESM.HeroToAttAck = PerformList[0].AttackersTarget;
                    ESM.currentState = EnemyStateMaschine.TurnState.ACTION;
                }
                if (PerformList[0].Type.Equals("Hero"))
                {
                    //HeroStateMaschine HSM= performer.GetComponent<HeroStateMaschine>();
                }
                battleState = PerfromAction.PERFROMACTION;
                break;
            case PerfromAction.PERFROMACTION:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 搜集行动
    /// </summary>
    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }
}

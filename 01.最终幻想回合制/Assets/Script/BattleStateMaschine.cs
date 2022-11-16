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

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> HerosInBattle = new List<GameObject>();
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
                break;
            case PerfromAction.TAKEACTION:
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

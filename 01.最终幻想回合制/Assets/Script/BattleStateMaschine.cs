using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<GameObject> EnemysInBattle = new List<GameObject>();

    /// <summary>
    /// 英雄的GUI选择
    /// </summary>
    public enum HeroGUI
    {
        /// <summary>
        /// 激活
        /// </summary>
        ACTIOVATE,
        /// <summary>
        /// 等待
        /// </summary>
        waiting,
        /// <summary>
        /// 输入1
        /// </summary>
        INPUT1,
        /// <summary>
        /// 输入2
        /// </summary>
        INPUT2,
        /// <summary>
        /// 空
        /// </summary>
        DONE,
    }

    /// <summary>
    /// 英雄输入
    /// </summary>
    public HeroGUI HeroInput;

    public List<GameObject> HeroToManage = new List<GameObject>();
    private HandleTurn HeroChoise;
    public GameObject enemyButton;

    /// <summary>
    /// 生成敌人按钮的父物体
    /// </summary>
    public Transform Spacer;

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerfromAction.WAIT;
        EnemysInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        //生成敌人按钮
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case PerfromAction.WAIT:
                if (PerformList.Count > 0)//行动的执行列表大于0的时候
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

    /// <summary>
    /// 敌人的按钮
    /// </summary>
    private void EnemyButtons()
    {
        foreach (var enemy in EnemysInBattle)
        {
            GameObject newButton = Instantiate(enemyButton, Spacer) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMaschine cur_enemy = enemy.GetComponent<EnemyStateMaschine>();
            Text buttontext = newButton.transform.Find("Text").GetComponent<Text>();
            buttontext.text = cur_enemy.enemy.name;
            button.EnemyPrefab = enemy;
        }
    }
}

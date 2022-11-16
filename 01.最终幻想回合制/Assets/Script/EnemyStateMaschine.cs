using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMaschine : MonoBehaviour
{
    private BattleStateMaschine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        /// <summary>
        /// 进度条上升
        /// </summary>
        PROCESSING,
        /// <summary>
        /// 选择敌人行动  
        /// </summary>
        CHOOSEACTION,
        /// <summary>
        /// 等待
        /// </summary>
        WAITING,
        /// <summary>
        /// 行动
        /// </summary>
        ACTION,
        /// <summary>
        /// 死去的
        /// </summary>
        DEAD,
    }

    public TurnState currentState;
    public float cur_colldown = 0f;
    public float max_colldown = 5f;

    /// <summary>
    /// 这个物体的初始位置
    /// </summary>
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("敌人显示当前状态:" + currentState);
        switch (currentState)
        {
            case TurnState.PROCESSING:
                UpgradeProgressBar();
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.ACTION:
                break;
            case TurnState.DEAD:
                break;
            default:
                break;
        }
    }

    // <summary>
    /// 升级进度条  冷却版
    /// </summary>
    void UpgradeProgressBar()
    {
        cur_colldown = cur_colldown + Time.deltaTime;
        if (cur_colldown >= max_colldown)//如果冷却时间到了
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    /// <summary>
    /// 选择敌人行动
    /// </summary>
    private void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        Debug.Log(enemy.name);
        myAttack.Attacker = enemy.name;
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[Random.Range(0,BSM.HerosInBattle.Count)];
        BSM.CollectActions(myAttack);
    }
}

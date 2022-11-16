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

    /// <summary>
    /// 是时候行动了
    /// </summary>
    private bool actionStarted = false;
    /// <summary>
    /// 要攻击的英雄
    /// </summary>
    public GameObject HeroToAttAck;
    private float animSpeed = 5f;

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
                //idea state
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
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
    /// 现在是敌人状态 选择英雄行动
    /// </summary>
    private void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        Debug.Log(enemy.name);
        myAttack.Attacker = enemy.name;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];
        BSM.CollectActions(myAttack);
    }


    /// <summary>
    /// 行动的时间到了
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;//如果没到可以行动,直接跳出协程
        }
        actionStarted = true;
        //播放敌人接近英雄的攻击动画
        Vector3 heroPostion = new Vector3(
            HeroToAttAck.transform.position.x - 1.5f,
            HeroToAttAck.transform.position.y,
            HeroToAttAck.transform.position.z);
        while (MoveTowrdsEnemy(heroPostion))//循环等待1帧
            yield return null;//这个是等待1帧的意思
        //等待
        yield return new WaitForSeconds(0.5f);
        //伤害
        //回到起始位置的动画
        Vector3 firstPosition = startPosition;
        while (MoveTowrdsStart(firstPosition))//循环等待1帧
            yield return null;//这个是等待1帧的意思
        //从BSM的Performer列表移除
        //重置BSM->等待
        actionStarted = false;
        cur_colldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    /// <summary>
    /// 移动敌人 如果敌人没移动到玩家坐标的时候  返回的就是false
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool MoveTowrdsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    /// <summary>
    /// 回到原来的位置
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool MoveTowrdsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}

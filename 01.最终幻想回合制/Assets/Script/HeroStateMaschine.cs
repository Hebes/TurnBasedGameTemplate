using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 英雄状态机
/// </summary>
public class HeroStateMaschine : MonoBehaviour
{
    private BattleStateMaschine BSM;
    public BaseHero baseHero;

    public enum TurnState
    {
        /// <summary>
        /// 进度条上升
        /// </summary>
        PROCESSING,
        /// <summary>
        /// 添加到列表中
        /// </summary>
        ADDTOLIST,
        /// <summary>
        /// 等待
        /// </summary>
        WAITING,
        /// <summary>
        /// 选择
        /// </summary>
        SELECTING,
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
    /// 冷却版进度条
    /// </summary>
    public Image ProgressBar;
    public GameObject Selector;

    //英雄回跳
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    public Vector3 startPosition;
    private float animSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        cur_colldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
        currentState = TurnState.PROCESSING;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("英雄显示当前状态:" + currentState);
        switch (currentState)
        {
            case TurnState.PROCESSING:
                UpgradeProgressBar();
                break;
            case TurnState.ADDTOLIST:
                BSM.HeroToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                //空闲状态
                break;
            case TurnState.SELECTING:
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

    /// <summary>
    /// 升级进度条  冷却版
    /// </summary>
    void UpgradeProgressBar()
    {
        cur_colldown = cur_colldown + Time.deltaTime;
        float calc_cooldown = cur_colldown / max_colldown;
        // 将给定值限制在给定的最小浮点值和最大浮点值之间。
        // 如果给定值在最小值和最大值范围内，则返回给定值。
        ProgressBar.transform.localScale = new Vector3(
            Mathf.Clamp(calc_cooldown, 0, 1),
            ProgressBar.transform.localScale.y,
            ProgressBar.transform.localScale.z);
        if (cur_colldown >= max_colldown)//如果冷却时间到了
        {
            currentState = TurnState.ADDTOLIST;
        }
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
        Vector3 enemyPostion = new Vector3(
            EnemyToAttack.transform.position.x + 1.5f,
            EnemyToAttack.transform.position.y,
            EnemyToAttack.transform.position.z);
        while (MoveTowrdsEnemy(enemyPostion))//循环等待1帧
            yield return null;//这个是等待1帧的意思
        //等待
        yield return new WaitForSeconds(0.5f);
        //伤害
        //回到起始位置的动画
        Vector3 firstPosition = startPosition;
        while (MoveTowrdsStart(firstPosition))//循环等待1帧
            yield return null;//这个是等待1帧的意思
        //从BSM的Performer列表移除
        BSM.PerformList.RemoveAt(0);
        //重置BSM->等待
        BSM.battleState = BattleStateMaschine.PerfromAction.WAIT;
        //结束协程
        actionStarted = false;
        //重置敌人状态
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

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
    private float cur_colldown = 0f;
    private float max_colldown = 5f;

    /// <summary>
    /// 选择器物体 就是角色头上顶的黄色小物体
    /// </summary>
    public GameObject Selector;

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
        Selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("敌人显示当前状态:" + currentState);
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
        //Debug.Log(enemy.theName);
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];
        //选择攻击方式
        int num = Random.Range(0, enemy.attacks.Count);
        myAttack.choosenAttack = enemy.attacks[num];
        //伤害公式=emeny的enemy.curAtk+选择攻击方式的一种的伤害-对方的防御
        Debug.Log(this.gameObject.name + "选择了：" + myAttack.choosenAttack.attackName + "攻击方式,对" + myAttack.AttackersTarget.name + "造成" + (myAttack.choosenAttack.attackDamage + enemy.curAtk) + "伤害");

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
        DoDamage();
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

    /// <summary>
    /// 给与伤害
    /// </summary>
    private void DoDamage()
    {
        float calc_damage = enemy.curAtk + BSM.PerformList[0].choosenAttack.attackDamage;
        HeroToAttAck.GetComponent<HeroStateMaschine>().TakeDamage(calc_damage);
    }
}

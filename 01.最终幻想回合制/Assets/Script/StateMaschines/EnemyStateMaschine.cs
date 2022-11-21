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

    /// <summary>
    /// dead 是否存活
    /// </summary>
    private bool alive = true;


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
                if (!alive)
                    return;
                else
                {
                    //change tag 切换标签
                    this.gameObject.tag = "DeadEnemy";
                    //not attackable by enemy 不能被敌人攻击 从BattleStateMaschine 英雄战斗列表删除自己
                    BSM.EnemysInBattle.Remove(this.gameObject);
                    //deactivate the selector 停用选择器 就是黄色的小物体
                    Selector.SetActive(false);
                    //remove item from performlist 
                    if (BSM.EnemysInBattle.Count > 0)
                    {

                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (i != 0)
                            {
                                //如果被攻击的是这个已经死亡的角色
                                if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                                    BSM.PerformList[i].AttackersTarget = BSM.EnemysInBattle[Random.Range(0, BSM.EnemysInBattle.Count)];
                                //从执行列表中删除项目
                                if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                                    BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                        }
                    }
                    //change color / play animation 改变颜色/播放动画
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //设置为不存活
                    alive = false;
                    //重新生成敌人的按钮
                    BSM.EnemyButtons();
                    //check alive 检查是否存活
                    BSM.battleStates = BattleStateMaschine.PerformAction.CHECKALIVE;
                }
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
        //Debug.Log(this.gameObject.name + "选择了：" + myAttack.choosenAttack.attackName + "攻击方式,对" + myAttack.AttackersTarget.name + "造成" + (myAttack.choosenAttack.attackDamage + enemy.curAtk) + "伤害");

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
        BSM.battleStates = BattleStateMaschine.PerformAction.WAIT;
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
    /// 遭受伤害
    /// </summary>
    public void TakeDamage(float getDamageAmount)
    {

        enemy.curHP -= getDamageAmount;
        if (enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currentState = TurnState.DEAD;
        }
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

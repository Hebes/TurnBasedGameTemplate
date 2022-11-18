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
    public BaseHero hero;

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
    private float cur_colldown = 0f;
    private float max_colldown = 5f;

    /// <summary>
    /// 冷却版进度条
    /// </summary>
    public Image ProgressBar;
    /// <summary>
    /// 选择器物体 就是角色头上顶的黄色小物体
    /// </summary>
    public GameObject Selector;

    //英雄回跳
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    public Vector3 startPosition;
    private float animSpeed = 10f;

    /// <summary>
    /// dead 是否存活
    /// </summary>
    private bool alive = true;

    //进度条 heroPanel
    private HeroPanelStats stats;
    /// <summary>
    /// 玩家的行动冷却条 HeroBar
    /// </summary>
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    void Start()
    {
        //find spacer 
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel/HeroPanelSpacer/");
        //create paneL
        CreatHeroPanel();

        startPosition = transform.position;
        cur_colldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>();
        currentState = TurnState.PROCESSING;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("英雄显示当前状态:" + currentState);
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
                if (!alive)
                {
                    return;
                }
                else
                {
                    //change tag 切换标签
                    this.gameObject.tag = "DeadHero";
                    //not attackable by enemy 不能被敌人攻击 从BattleStateMaschine 英雄战斗列表删除自己
                    BSM.HerosInBattle.Remove(this.gameObject);
                    //not managable 不可以控制 从BattleStateMaschine 英雄列表的管理列表删除自己
                    BSM.HeroToManage.Remove(this.gameObject);
                    //deactivate the selector 停用选择器 就是黄色的小物体
                    Selector.SetActive(false);
                    //reset gui 全部重设 关闭所有选择面板
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //remove item from performlist 从执行列表中删除项目
                    for (int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    //change color / play animation 改变颜色/播放动画
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset heroinput 重置heroinput
                    BSM.HeroInput = BattleStateMaschine.HeroGUI.ACTIOVATE;
                    alive = false;
                }

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
        //显示进度条图片的进度
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

    /// <summary>
    /// 遭受伤害
    /// </summary>
    public void TakeDamage(float getDamageAmount)
    {
        hero.curHP -= getDamageAmount;
        Debug.Log(hero.theName + "受到：" + getDamageAmount + "点伤害,剩余生命值：" + hero.curHP);
        if (hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdataHeroPanel();
    }

    /// <summary>
    /// 创建英雄进度条面板
    /// </summary>
    private void CreatHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel, HeroPanelSpacer) as GameObject;
        HeroPanel.name = "HeroBar_" + hero.theName;//设置物体名称
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.OnSetInfo(
            hero.theName,
            "HP：" + hero.curHP + "/" + hero.baseHP,
            "MP：" + hero.curMP + "/" + hero.BaseMP);
        ProgressBar = stats.ProgressBar;
    }

    /// <summary>
    /// 更新英雄面板
    /// </summary>
    private void UpdataHeroPanel()
    {
        stats.OnSetInfo(
           "HP：" + hero.curHP + "/" + hero.baseHP,
           "MP：" + hero.curMP + "/" + hero.BaseMP);
    }
}

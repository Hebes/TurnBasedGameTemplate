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
    public enum PerformAction
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
        /// <summary>
        /// 检查敌人或者玩家是否存活
        /// </summary>
        CHECKALIVE,
        /// <summary>
        /// 赢
        /// </summary>
        WIN,
        /// <summary>
        /// 输
        /// </summary>
        LOSE

    }

    /// <summary>
    /// 战斗的状态
    /// </summary>
    public PerformAction battleStates;

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
        WAITING,
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

    /// <summary>
    /// 英雄列表的管理
    /// </summary>
    public List<GameObject> HerosToManage = new List<GameObject>();
    private HandleTurn HeroChoise;

    public GameObject enemyButton;
    /// <summary>
    /// 生成敌人按钮的父物体
    /// </summary>
    public Transform Spacer;

    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;


    //物理攻击
    public Transform actionSpacer;
    public GameObject actionButton;//物理攻击按钮模板
    //魔法攻击
    public GameObject magicButton;//魔法攻击按钮模板
    public Transform magicSpacer;
    private List<GameObject> atkBtns = new List<GameObject>();

    private List<GameObject> enemytBtns = new List<GameObject>();

    /// <summary>
    /// 敌人重生点
    /// </summary>
    public List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            Vector3 creatEnemyPoint = spawnPoints[i].position;
            GameObject NewEnemy = Instantiate(GameManager.instance.enemysToBattle[i], creatEnemyPoint, Quaternion.identity);
            NewEnemy.name = NewEnemy.GetComponent<EnemyStateMaschine>().enemy.theName + "_" + (i + 1);
            NewEnemy.GetComponent<EnemyStateMaschine>().enemy.theName = NewEnemy.name;
            EnemysInBattle.Add(NewEnemy);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        battleStates = PerformAction.WAIT;
        //EnemysInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        HeroInput = HeroGUI.ACTIOVATE;

        //面板设置关闭
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        MagicPanel.SetActive(false);

        //生成敌人按钮
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        //战斗状态
        switch (battleStates)
        {
            case PerformAction.WAIT:
                if (PerformList.Count > 0)//行动的执行列表大于0的时候
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;
            case PerformAction.TAKEACTION://采取行动
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if (PerformList[0].Type.Equals("Enemy"))
                {
                    EnemyStateMaschine ESM = performer.GetComponent<EnemyStateMaschine>();
                    //攻击前检查被攻击的英雄是否在列表中
                    for (int i = 0; i < HerosInBattle.Count; i++)
                    {
                        //存在的话
                        if (PerformList[0].AttackersTarget == HerosInBattle[i])
                        {
                            ESM.HeroToAttAck = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMaschine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = HerosInBattle[Random.Range(0, HerosInBattle.Count)];
                            ESM.HeroToAttAck = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyStateMaschine.TurnState.ACTION;
                        }
                    }
                }
                if (PerformList[0].Type.Equals("Hero"))
                {
                    //Debug.Log("英雄是表演的英雄");
                    HeroStateMaschine HSM = performer.GetComponent<HeroStateMaschine>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.currentState = HeroStateMaschine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFROMACTION;
                break;
            case PerformAction.PERFROMACTION:
                break;
            case PerformAction.CHECKALIVE:
                if (HerosInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                    //Lose game
                }
                else if (EnemysInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                    //win the battle
                }
                else
                {
                    //call function 
                    clearAttackPanel();
                    HeroInput = HeroGUI.ACTIOVATE;
                }
                break;
            case PerformAction.WIN:
                Debug.Log("你赢了");
                for (int i = 0; i < HerosInBattle.Count; i++)
                    HerosInBattle[i].GetComponent<HeroStateMaschine>().currentState = HeroStateMaschine.TurnState.WAITING;
                //显示结算面板 需要自己填写
                //加载进入战斗前的场景
                GameManager.instance.LoadSceneAfterBattle();
                GameManager.instance.gameState = GameManager.GameStates.WORLD_STATE;
                GameManager.instance.enemysToBattle.Clear();
                break;
            case PerformAction.LOSE:
                Debug.Log("你输了");
                break;
            default:
                break;
        }

        //英雄输入状态
        switch (HeroInput)
        {
            case HeroGUI.ACTIOVATE:
                if (HerosToManage.Count > 0)
                {
                    HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    HeroChoise = new HandleTurn();
                    AttackPanel.SetActive(true);

                    //populate action buttons 填充操作按钮
                    CreateAttackButtons();

                    HeroInput = HeroGUI.WAITING;
                }
                break;
            case HeroGUI.WAITING:
                break;
            case HeroGUI.INPUT1:
                break;
            case HeroGUI.INPUT2:
                break;
            case HeroGUI.DONE:
                HeroInputDone();
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
    /// 敌人按钮的生成
    /// </summary>
    public void EnemyButtons()
    {
        //cleanup 清理
        foreach (GameObject enemyBtn in enemytBtns)
        {
            Destroy(enemyBtn);
        }
        enemytBtns.Clear();
        //敌人按钮适配Unity 5 Tutorial: Turn Based Battle System #07 - Gui Improvements
        //创建按钮
        foreach (var enemy in EnemysInBattle)
        {
            GameObject newButton = Instantiate(enemyButton, Spacer) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMaschine cur_enemy = enemy.GetComponent<EnemyStateMaschine>();

            Text buttontext = newButton.transform.Find("Text").GetComponent<Text>();
            buttontext.text = cur_enemy.enemy.theName;

            button.EnemyPrefab = enemy;
            enemytBtns.Add(newButton);
        }
    }

    /// <summary>
    /// 玩家输入 物理攻击方式
    /// </summary>
    public void Input1()
    {
        HeroChoise.Attacker = HerosToManage[0].name;
        HeroChoise.AttackersGameObject = HerosToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = HerosToManage[0].GetComponent<HeroStateMaschine>().hero.attacks[0];
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    /// <summary>
    /// 物理攻击选择
    /// </summary>
    /// <param name="choosenEnemy"></param>
    public void Input2(GameObject choosenEnemy)
    {
        HeroChoise.AttackersTarget = choosenEnemy;
        HeroInput = HeroGUI.DONE;
    }

    /// <summary>
    /// 玩家输入完毕后
    /// </summary>
    private void HeroInputDone()
    {
        PerformList.Add(HeroChoise);
        clearAttackPanel();


        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HerosToManage.RemoveAt(0);
        HeroInput = HeroGUI.ACTIOVATE;
    }

    /// <summary>
    /// 清空攻击面板
    /// </summary>
    private void clearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(false);
        //clean the attackpanel 清理攻击面板
        foreach (GameObject atkBtn in atkBtns)
            Destroy(atkBtn);
        atkBtns.Clear();
    }


    /// <summary>
    /// 创建action buttons
    /// </summary>
    private void CreateAttackButtons()
    {
        //物理攻击
        GameObject AttackButton = Instantiate(actionButton, actionSpacer) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "攻击";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        atkBtns.Add(AttackButton);
        //魔法攻击
        GameObject MagicAttackButton = Instantiate(actionButton, actionSpacer) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "魔法";
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        atkBtns.Add(MagicAttackButton);

        if (HerosToManage[0].GetComponent<HeroStateMaschine>().hero.MagicAttack.Count > 0)//如果魔法攻击的技能大于0
        {
            foreach (BaseAttack magicAtk in HerosToManage[0].GetComponent<HeroStateMaschine>().hero.MagicAttack)
            {
                GameObject MagicButton = Instantiate(magicButton, magicSpacer) as GameObject;
                Text MagicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                MagicButtonText.text = magicAtk.attackName;
                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.magicAttackToPerform = magicAtk;
                ATB.GetComponent<Button>().onClick.AddListener(ATB.CastMagicAttack);
                atkBtns.Add(MagicButton);
            }
        }
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;

        }
    }

    /// <summary>
    /// 选择魔法攻击
    /// </summary>
    /// <param name="choosenMagic"></param>
    public void Input4(BaseAttack choosenMagic)//choosen magic attack
    {
        HeroChoise.Attacker = HerosToManage[0].name;
        HeroChoise.AttackersGameObject = HerosToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    /// <summary>
    /// 切换到魔法攻击
    /// </summary>
    public void Input3()//switching to magic attacks 
    {
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }
}




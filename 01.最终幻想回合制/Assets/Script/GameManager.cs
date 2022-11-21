using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 第一个场景是WorldMap  这个脚本是入口
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    /// <summary>
    /// 类随机怪物 CLASS RANDOM MONSTER
    /// </summary>
    [System.Serializable]
    public class RegionData
    {
        public string regionName;
        public int maxAmountEnemys = 4;
        public string BattleScene;
        public List<GameObject> possibleEnemys = new List<GameObject>();
    }
    /// <summary>
    /// 敌人数量
    /// </summary>
    public int enemyAmount;
    /// <summary>
    /// BATTLE 战斗的怪物列表
    /// </summary>
    public List<GameObject> enemysToBattle = new List<GameObject>();
    /// <summary>
    /// 当前怪物编号
    /// </summary>
    public int curRegions;
    /// <summary>
    /// 随机怪物列表
    /// </summary>
    public List<RegionData> Regions = new List<RegionData>();

    /// <summary>
    /// 重生点 SPAWNPOINTS
    /// </summary>
    public string nextSpawnPoint;

    /// <summary>
    /// 玩家角色预制体
    /// </summary>
    public GameObject heroCharacter;

    /// <summary>
    /// POSITIONS 下一个英雄的位置
    /// </summary>
    public Vector3 nextHeroPosition;
    /// <summary>
    /// 英雄最后的位置
    /// </summary>
    public Vector3 lastHeroPosition;

    //SCENES
    public string sceneToLoad;//下一个场景的名称
    public string lastScene;//BATTLE 最后一个场景的名称

    /// <summary>
    /// B0OLS  是否移动
    /// </summary>
    public bool isWalking = false;
    /// <summary>
    /// 可以遇到  就是战斗
    /// </summary>
    public bool canGetEncounter = false;
    /// <summary>
    /// 遭遇战斗
    /// </summary>
    public bool gotAttacked = false;

    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStates
    {
        /// <summary>
        /// 世界
        /// </summary>
        WORLD_STATE,
        /// <summary>
        /// 城镇
        /// </summary>
        TOWN_STATE,
        /// <summary>
        /// 战斗
        /// </summary>
        BATTLE_STATE,
        /// <summary>
        /// 闲置的
        /// </summary>
        IDLE,
    }
    /// <summary>
    /// 游戏状态
    /// </summary>
    public GameStates gameState;

    private void Awake()
    {
        //第一个场景是WorldMap 这个脚本是入口
        //check if instance exist
        if (instance == null)
            //if not set the instance to this .
            instance = this;
        //if it exist but is not this instance
        else if (instance != this)
            //destroy it
            Destroy(gameObject);
        //set this to be not destroyable
        DontDestroyOnLoad(gameObject);

        if (!GameObject.Find("HeroCharacter"))
        {
            GameObject Hero = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity);
            Hero.name = "HeroCharacter";
        }
    }
    private void Update()
    {
        switch (gameState)
        {
            case GameStates.WORLD_STATE:
                if (isWalking)
                    RandomEncounter();
                if (gotAttacked)
                    gameState = GameStates.BATTLE_STATE;
                break;
            case GameStates.TOWN_STATE:
                break;
            case GameStates.BATTLE_STATE:
                //加载战斗场景
                StartBattle();
                //去往IDLE状态
                gameState = GameStates.IDLE;
                break;
            case GameStates.IDLE:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 加载下一个场景
    /// </summary>
    public void LoadNextScene() => SceneManager.LoadScene(sceneToLoad);

    /// <summary>
    /// 加载最后一次场景  战斗完毕后
    /// </summary>
    public void LoadSceneAfterBattle() => SceneManager.LoadScene(lastScene);


    /// <summary>
    /// 随机遇敌
    /// </summary>
    void RandomEncounter()
    {
        if (isWalking && canGetEncounter)//正在移动并且是可以遇怪的区域
        {
            if (Random.Range(0, 1000) < 10)
            {
                Debug.Log("I got attacked");
                gotAttacked = true;
            }
        }
    }

    void StartBattle()
    {
        //AMOUNT OF ENEMYS
        enemyAmount = Random.Range(1, Regions[curRegions].maxAmountEnemys + 1);
        //WHICH ENEMYS
        for (int i = 0; i < enemyAmount; i++)
            enemysToBattle.Add(Regions[curRegions].possibleEnemys[Random.Range(0, Regions[curRegions].possibleEnemys.Count)]);
        //HERO
        lastHeroPosition = GameObject.Find("HeroCharacter").gameObject.transform.position;
        nextHeroPosition = lastHeroPosition;
        lastScene = SceneManager.GetActiveScene().name;
        //LOAD LEVEL
        SceneManager.LoadScene(Regions[curRegions].BattleScene);
        //RESET HERO
        isWalking = false;
        gotAttacked = false;
        canGetEncounter = false;
    }
}

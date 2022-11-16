using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 英雄状态机
/// </summary>
public class HeroStateMaschine : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
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
                break;
            case TurnState.WAITING:
                break;
            case TurnState.SELECTING:
                break;
            case TurnState.ACTION:
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
}

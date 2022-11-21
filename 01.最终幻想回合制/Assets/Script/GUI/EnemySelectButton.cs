using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;//敌人物体

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SelectEnemy);
        button.onClick.AddListener(HideSelector);

        //EventTrigger监听
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        //进入
        UnityAction<BaseEventData> Enter = new UnityAction<BaseEventData>((BaseEventData) => 
        {
            ShowSelector();
        });
        EventTrigger.Entry myEnter = new EventTrigger.Entry();
        myEnter.eventID = EventTriggerType.PointerEnter;
        myEnter.callback.AddListener(Enter);
        eventTrigger.triggers.Add(myEnter);
        //退出
        UnityAction<BaseEventData> Exit = new UnityAction<BaseEventData>((BaseEventData) =>
        {
            HideSelector();
        });
        EventTrigger.Entry myExit = new EventTrigger.Entry();
        myExit.eventID = EventTriggerType.PointerExit;
        myExit.callback.AddListener(Exit);
        eventTrigger.triggers.Add(myExit);
    }

    /// <summary>
    /// 选择敌人 拽托到自己的按钮上
    /// </summary>
    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>().Input2(EnemyPrefab);
    }

    /// <summary>
    /// 鼠标滑动显示头上黄色物体
    /// </summary>
    public void HideSelector()
    {
        //Debug.Log("进入EventTrigger监听代码");
        EnemyPrefab.transform.Find("Selector").gameObject.SetActive(false);
    }

    /// <summary>
    /// 鼠标滑动显示头上黄色物体
    /// </summary>
    public void ShowSelector()
    {
        //Debug.Log("进入EventTrigger监听代码");
        EnemyPrefab.transform.Find("Selector").gameObject.SetActive(true);

    }
}

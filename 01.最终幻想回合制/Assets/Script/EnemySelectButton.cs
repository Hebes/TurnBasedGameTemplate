using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SelectEnemy);
    }

    /// <summary>
    /// 选择敌人 拽托到自己的按钮上
    /// </summary>
    public void  SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMaschine>().Input2(EnemyPrefab);
    }
}

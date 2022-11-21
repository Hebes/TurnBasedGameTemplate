using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroPanelStats : MonoBehaviour
{
    public Text HeroName;
    public Text HeroHP;
    public Text HeroMP;
    public Image ProgressBar;

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="name"></param>
    /// <param name="HP"></param>
    /// <param name="MP"></param>
    public void OnSetInfo(string name, string HP, string MP)
    {
        HeroName.text = name;
        HeroHP.text = HP;
        HeroMP.text = MP;
    }

    public void OnSetInfo(string HP, string MP)
    {
        HeroHP.text = HP;
        HeroMP.text = MP;
    }
}

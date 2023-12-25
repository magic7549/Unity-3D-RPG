using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    [SerializeField] private Slider expBar;

    private void Start()
    {
        hpBar.maxValue = IngameManager.instance.stat.maxHp;
        mpBar.maxValue = IngameManager.instance.stat.maxMp;
        expBar.maxValue = IngameManager.instance.stat.requiredExp;
        levelText.text = IngameManager.instance.stat.level.ToString();
    }

    private void Update()
    {
        if (IngameManager.instance.stat != null)
        {
            hpBar.value = IngameManager.instance.stat.currHp;
            mpBar.value = IngameManager.instance.stat.currMp;
            expBar.value = IngameManager.instance.stat.exp;

            hpText.text = IngameManager.instance.stat.currHp + " / " + IngameManager.instance.stat.maxHp;
            mpText.text = IngameManager.instance.stat.currMp + " / " + IngameManager.instance.stat.maxMp;
            expText.text = IngameManager.instance.stat.exp + " / " + IngameManager.instance.stat.requiredExp;
        }
    }

    public void UpdateMax()
    {
        if (IngameManager.instance.stat != null)
        {
            hpBar.maxValue = IngameManager.instance.stat.maxHp;
            mpBar.maxValue = IngameManager.instance.stat.maxMp;
        }
    }

    public void UpdateExpMax()
    {
        if (IngameManager.instance.stat != null)
        {
            expBar.maxValue = IngameManager.instance.stat.requiredExp;
        }
    }

    public void UpdateLevelText()
    {
        if (IngameManager.instance.stat != null)
        {
            levelText.text = IngameManager.instance.stat.level.ToString();
        }
    }
}

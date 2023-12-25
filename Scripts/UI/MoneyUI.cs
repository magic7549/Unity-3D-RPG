using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    void Update()
    {
        moneyText.text = IngameManager.instance.stat.money.ToString();
    }
}

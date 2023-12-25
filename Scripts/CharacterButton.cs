using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;
using TMPro; 
using UnityEngine.UI;
using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    private TextMeshProUGUI text; 

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>(); 
        if (SystemManager.instance.dbManager.slot[int.Parse(gameObject.name.Substring(8, 1)) - 1] != 0)
        {
            text.text = "Load";
        }
        else
        {
            text.text = "New Start";
        }
    }

    public void OnClick(int slot_num)
    {
        if (SystemManager.instance.dbManager.slot[slot_num - 1] == 0)
        {
            SystemManager.instance.dbManager.OnCreateCharacter(slot_num);
        }

        SystemManager.instance.selectSlotNum = slot_num;
        SceneManager.LoadScene("Lobby");
    }
}
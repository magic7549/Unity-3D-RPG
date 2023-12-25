using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private GameObject levelUpPanel;

    public IEnumerator LevelUp()
    {
        Debug.Log("LevelUP");

        levelUpPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        levelUpPanel.SetActive(false);
    }
}

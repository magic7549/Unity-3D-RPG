using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject quickSlot_1;
    [SerializeField]
    private GameObject quickSlot_2;

    private Slot slot1;
    private Slot slot2;

    private void Start()
    {
        slot1 = quickSlot_1.gameObject.GetComponentInChildren<Slot>();
        slot2 = quickSlot_2.gameObject.GetComponentInChildren<Slot>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("QuickSlot 1"))
        {
            slot1.UseItem();
        }
        else if (Input.GetButtonDown("QuickSlot 2"))
        {
            slot2.UseItem();
        }
    }
}

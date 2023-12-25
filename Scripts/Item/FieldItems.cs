using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItems : MonoBehaviour
{
    public int code;
    public int count;

    public Item GetItem()
    {
        return IngameManager.instance.itemDatabase.itemDB[code];
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SavepointEnum
{
    start_point,
    forest_1,
    forest_2,
    badlands_enter,
    village,
    shingri_la,
    cars,
    mountain,
    Laboratory
}

public class SavePoint : MonoBehaviour
{
    [SerializeField]
    private SavepointEnum savepointEnum;

    public int GetSavepointNum()
    {
        return (int)savepointEnum;
    }
}

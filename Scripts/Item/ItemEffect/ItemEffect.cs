using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract bool ExecuteRole();

    // 장비템 해제를 위한 빈 함수
    public virtual void Unequip()
    {
        // 기본적으로 아무 동작도 하지 않음
    }
}

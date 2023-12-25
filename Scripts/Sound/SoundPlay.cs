using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    public void Play(int num)
    {
        SystemManager.instance.soundManager.PlaySound(num);
    }
}

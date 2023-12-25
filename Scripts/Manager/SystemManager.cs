using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public static SystemManager instance;
    private void Awake()
    {
        //게임 매니저 DontDestroyOnLoad 적용
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public int selectSlotNum;
    public DB_Manager dbManager;
    public SoundManager soundManager;
}

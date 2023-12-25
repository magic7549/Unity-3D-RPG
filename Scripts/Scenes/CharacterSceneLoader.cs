using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSceneLoader : MonoBehaviour
{
    public void LoadCharacterScene()
    {
        SceneManager.LoadScene("Menu");
    }
}

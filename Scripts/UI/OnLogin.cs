using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OnLogin : MonoBehaviour
{
    public TMP_InputField inputId, inputPassword;

    public void ClickLogin()
    {
        if (inputId.text.Trim().Equals(""))
        {
            Debug.Log("아이디를 입력");
            return;
        }
        if (inputPassword.text.Trim().Equals(""))
        {
            Debug.Log("비밀번호 입력");
            return;
        }

        if (SystemManager.instance.dbManager.OnLogin(inputId.text, inputPassword.text)) 
        {
            Debug.Log("로그인 성공");
            SceneManager.LoadScene("Menu");
        }
    }
}

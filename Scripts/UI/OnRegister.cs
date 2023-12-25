using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputId, inputPassword;
    [SerializeField] private TMP_Text infoText;

    public void ClickRegister()
    {
        if (inputId.text.Trim().Equals(""))
        {
            infoText.text = "아이디를 입력하세요.";
            return;
        }
        if (inputPassword.text.Trim().Equals(""))
        {
            infoText.text = "비밀번호를 입력하세요.";
            return;
        }

        switch (SystemManager.instance.dbManager.OnRegister(inputId.text, inputPassword.text))
        {
            case 200:
                infoText.text = "회원가입에 성공하였습니다.";
                break;
            case 400:
                infoText.text = "중복된 아이디입니다.";
                break;
            default:
                infoText.text = "error";
                break;
        }
    }
}

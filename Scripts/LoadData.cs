/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class TestPhp : MonoBehaviour
{
    [SerializeField]
    Text message = null;

    [SerializeField]
    Text m_UserData = null;

    public class ResUserData
    {
        // �迭�� �ƴϰ� �� ����Ʈ�� �ؾ��մϴ�.
        public List<UserData> UserData;
    }

    public class UserData
    {
        public string id;
        public string pw;
    }

    void Start()
    {
        StartCoroutine(GetUserData());
    }

    IEnumerator GetUserData()
    {
        // php�� �� �����ϱ�
        WWWForm form = new WWWForm();
        form.AddField("param1", "2");
        form.AddField("param1", "�����̸�2");

        // Local
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/helloworld.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
                message.text = data;
                ResUserData res = JsonFx.Json.JsonReader.Deserialize<ResUserData>(data);

                string szUser = "";
                foreach (UserData userData in res.UserData)
                {
                    szUser += userData.id + " ";
                    szUser += userData.pw + "\r\n";
                }

                m_UserData.text = szUser;
            }
        }
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerColor : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Material CharacterMaterial;
    private SkinnedMeshRenderer Character;

    private void Start()
    {
        ChangeColor();
        ChangeMatrial();
    }

    private void ChangeColor()
    {
        // 파티클 색상 변경, 마스터 클라이언트 제외 변경
        if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            ParticleSystem.MainModule main = particle.main;

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            // 랜덤 컬러 생성
            Color randomColor = new Color(r, g, b);

            main.startColor = randomColor;

            Vector3 colorToVector = new Vector3(randomColor.r, randomColor.g, randomColor.b);
            photonView.RPC("ColorRPC", RpcTarget.Others, colorToVector);
        }
    }

    private void ChangeMatrial()
    {
        if ((IngameManager.instance.questManager.questId == 80) && (IngameManager.instance.questManager.questActionIndex == 5))
        {
            if (photonView.IsMine)
            {
                Character = GetComponentInChildren<SkinnedMeshRenderer>();
                Character.material = CharacterMaterial;
            }
        }
    }

    [PunRPC]
    private void ColorRPC(Vector3 vector)
    {
        Color color = new Color(vector.x, vector.y, vector.z);
        ParticleSystem.MainModule main = particle.main;
        main.startColor = color;
    }
}

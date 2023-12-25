using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_1 : MonoBehaviour
{
    public float damage;
    [SerializeField] private SphereCollider skill_1Collider;
    public float toggleInterval = 0.4f; // Collider를 토글하는 간격 (초)
    private float timer;

    void Awake()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("SetDmgRPC", RpcTarget.All, IngameManager.instance.stat.damage);
    }

    void Start()
    {
        Destroy(gameObject, 5f);    
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= toggleInterval)
        {
            ToggleCollider(!skill_1Collider.enabled);
            timer = 0f; // 타이머 초기화
        }
    }

    void ToggleCollider(bool isActive)
    {
        if (skill_1Collider != null)
        {
            skill_1Collider.enabled = isActive;
        }
    }

    [PunRPC]
    private void SetDmgRPC(float realDmg)
    {
        damage = realDmg;
    }
}
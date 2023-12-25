using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerWeapon : MonoBehaviourPun
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private int[] itemCode;

    public void Start()
    {
        ChangeWeapon();
    }

    public void ChangeWeapon()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < itemCode.Length; i++)
            {
                if (weapons[i].activeSelf)
                {
                    weapons[i].SetActive(false);
                    photonView.RPC("WeaponRPC", RpcTarget.Others, i, false);

                }

                // IngameManager.instance.equipment.equipment[0] : 장비 중 무기
                if (itemCode[i] == IngameManager.instance.equipment.equipment[0])
                {
                    weapons[i].SetActive(true);
                    photonView.RPC("WeaponRPC", RpcTarget.Others, i, true);
                }
            }
        }
    }

    [PunRPC]
    private void WeaponRPC(int num, bool active)
    {
        weapons[num].SetActive(active);
    }
}

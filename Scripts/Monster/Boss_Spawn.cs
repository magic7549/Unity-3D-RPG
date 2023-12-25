using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss_Spawn : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject instantiatePosition;

    public void Spawn()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Boss[] objects = FindObjectsOfType<Boss>();
        if (objects.Length == 0)
            PhotonNetwork.Instantiate(prefab.name, instantiatePosition.transform.position, instantiatePosition.transform.rotation);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Spawn();
        }
    }
    
}

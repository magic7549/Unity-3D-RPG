using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterSpawner : MonoBehaviourPun
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxSpawnNum;
    [SerializeField] private float sp_minTime;
    [SerializeField] private float sp_maxTime;
    [SerializeField] private int sp_range;

    private Terrain terrain;
    private List<GameObject> spawnList = new List<GameObject>();

    private void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.tag == "Player")
        {
            InitSpawn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.tag == "Player")
        {
            float range = gameObject.GetComponent<SphereCollider>().radius;
            Collider[] playersInRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Player"));

            if (playersInRange.Length == 0)
            {
                OnReset();
            }
        }
    }

    private void InitSpawn()
    {
        while (maxSpawnNum > spawnList.Count)
        {
            // 일정 범위 내 지형 고려한 랜덤 좌표 생성
            Vector3 spawnPosition = new Vector3((int)transform.position.x + Random.Range(-sp_range, sp_range), 0f, (int)transform.position.z + Random.Range(-sp_range, sp_range));
            spawnPosition.y = terrain.SampleHeight(spawnPosition);

            GameObject tempMonster = PhotonNetwork.Instantiate(prefab.name, spawnPosition, Random.rotation);
            tempMonster.gameObject.GetPhotonView().RPC("SetSpawner", RpcTarget.AllBuffered, gameObject.name);
            spawnList.Add(tempMonster);
        }
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (maxSpawnNum > spawnList.Count)
            {
                yield return new WaitForSeconds(Random.Range(sp_minTime, sp_maxTime));
                while (maxSpawnNum > spawnList.Count)
                {
                    Vector3 spawnPosition = new Vector3((int)transform.position.x + Random.Range(-sp_range, sp_range), 0f, (int)transform.position.z + Random.Range(-sp_range, sp_range));
                    spawnPosition.y = terrain.SampleHeight(spawnPosition);

                    GameObject tempMonster = PhotonNetwork.Instantiate(prefab.name, spawnPosition, Random.rotation);
                    tempMonster.gameObject.GetPhotonView().RPC("SetSpawner", RpcTarget.AllBuffered, gameObject.name);
                    spawnList.Add(tempMonster);
                }
            }
        }
    }

    public void Despawn(GameObject monster)
    {
        spawnList.Remove(monster);
    }

    private void OnReset()
    {
        StopAllCoroutines();

        while (spawnList.Count > 0) 
        {
            PhotonNetwork.Destroy(spawnList[0].gameObject);
            spawnList.Remove(spawnList[0]);
        }
    }
}

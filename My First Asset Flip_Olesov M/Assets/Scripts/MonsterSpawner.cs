using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject[] monsters;
    GameObject spawnPosition;

    void Start()
    {
        spawnPosition = gameObject.transform.Find("MonsterSpawn").gameObject;
    }

    public void SpawnEnemy()
    {
        var m = Instantiate(monsters[Random.Range(0,monsters.Length)], spawnPosition.transform.position, spawnPosition.transform.rotation).GetComponent<Monster>();
        m.Init(waypoints);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] spawners;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (GameObject s in spawners)
                s.GetComponent<MonsterSpawner>().SpawnEnemy();
            gameObject.SetActive(false);
        }
    }
}

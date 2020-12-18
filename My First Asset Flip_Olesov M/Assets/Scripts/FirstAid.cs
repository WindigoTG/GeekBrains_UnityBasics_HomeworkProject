using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    [SerializeField] private int healingAmmount = 25;
    [SerializeField] private float turnSpeed = 100;
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, turnSpeed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<Player>().GetHealth(healingAmmount))
                gameObject.SetActive(false);
        }
    }
}

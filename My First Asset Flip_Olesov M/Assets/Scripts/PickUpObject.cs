using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 100;
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, turnSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().AddToInventory(gameObject);
            gameObject.SetActive(false);
        }
    }
}

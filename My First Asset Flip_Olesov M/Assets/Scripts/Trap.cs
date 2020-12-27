using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().SetPressed(true);
            Debug.Log("Press true");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().SetPressed(false);
            Debug.Log("Press false");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log(other.gameObject.GetComponent<Player>().CheckPressed());
            if (other.gameObject.GetComponent<Player>().CheckPressed())
                other.gameObject.GetComponent<Player>().TakeDamage(999);
        }
    }
}

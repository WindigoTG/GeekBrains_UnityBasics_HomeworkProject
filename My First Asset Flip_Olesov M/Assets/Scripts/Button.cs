using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject door;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            var press = Input.GetAxis("Fire3");
            if (press > 0)
                door.GetComponent<Door>().Activate();
        }
    }
}

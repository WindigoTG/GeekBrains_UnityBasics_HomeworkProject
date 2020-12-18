using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject neededKey = null;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetAxis("Fire3") > 0)
            {
                if (neededKey == null)
                    door.GetComponent<Door>().Activate();
                else
                    if (other.GetComponent<Player>().FindInInventory(neededKey.tag.ToString()))
                        door.GetComponent<Door>().Activate();
            }
        }
    }
}

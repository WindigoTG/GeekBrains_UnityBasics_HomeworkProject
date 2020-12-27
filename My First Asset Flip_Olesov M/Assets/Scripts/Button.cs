using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject neededKey = null;

    GameObject ChildGameObject1;

    private void Start()
    {
        ChildGameObject1 = transform.GetChild(1).gameObject;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && Input.GetAxis("Fire2") > 0)
        {
            ChildGameObject1.GetComponent<Animation>().Play();
            if (neededKey == null)
                    door.GetComponent<Door>().Activate();
                else
                    if (other.GetComponent<Player>().FindInInventory(neededKey.tag.ToString()))
                        door.GetComponent<Door>().Activate();
        }
    }
}

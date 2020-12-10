using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    GameObject[] cameraList;
    [SerializeField] GameObject localCamera;
    [SerializeField] GameObject player;

    void Start()
    {
        cameraList = GameObject.FindGameObjectsWithTag("VirtCam");
        foreach (GameObject cam in cameraList)
            Debug.Log(cam.name);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            foreach (GameObject cam in cameraList)
                    cam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 10;
            localCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 99;
        }
    }
}

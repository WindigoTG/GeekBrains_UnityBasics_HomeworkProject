using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float turnSpeed = 20;
    Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }


    void Update()
    {
        var ver = Input.GetAxis("Vertical");
        var hor = Input.GetAxis("Horizontal");
        if (ver > 0)
        {
            transform.Translate(transform.forward * ver * speed * Time.deltaTime, Space.World);
        }
        if (ver < 0)
        {
            transform.Translate(transform.forward * ver * speed / 2 * Time.deltaTime, Space.World);
        }
        if (hor != 0)
        {
            transform.RotateAround(transform.position, Vector3.up, hor * turnSpeed * Time.deltaTime);
        }
        bool isWalkingForward = (ver > 0);
        bool isWalkingBack = (ver < 0);
        m_Animator.SetBool("IsWalkingForward", isWalkingForward);
        m_Animator.SetBool("IsWalkingBack", isWalkingBack);
    }
}

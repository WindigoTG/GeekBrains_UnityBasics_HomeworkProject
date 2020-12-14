using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float turnSpeed = 20;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float bulletSpeed = 3;
    [SerializeField] private float fireRate = 3;

    [SerializeField] private int hp = 100;
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int damage = 25;

    private float fireCoolDown = 0;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        var ver = Input.GetAxis("Vertical");
        var hor = Input.GetAxis("Horizontal");
        m_Movement = transform.forward;

        bool isWalkingForward = false;
        bool isRunningForward = false;
        bool isWalkingBack = false;
        bool isShooting = false;

        if (Input.GetAxis("Fire1") > 0)
        {
            isShooting = true;
            if (fireCoolDown <= 0)
            {
                m_Animator.SetTrigger("ShotMade");
                var b = Instantiate(bullet, bulletSpawnPosition.position, bulletSpawnPosition.rotation).GetComponent<Bullet>();
                b.Init(damage, bulletSpeed);
                fireCoolDown = fireRate;
            }
            if (fireCoolDown > 0)
                fireCoolDown -= Time.deltaTime;
        }

        if (ver !=0)
        {
            //m_Movement.Set(ver, 0f, 0f);
            //m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z * ver * speed * Time.deltaTime);
            //transform.Translate(transform.forward * ver * speed * Time.deltaTime, Space.World);
            //transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            //transform.Translate(m_Movement * speed * Time.deltaTime, Space.Self);
            //Vector3 desiredForward = transform.forward;
            if (Input.GetAxis("Fire2") > 0 && ver > 0 && !isShooting)
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * ver * speed * 2.5f * Time.deltaTime);
                isRunningForward = true;
            }
            else
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * ver * speed * Time.deltaTime);
                isWalkingForward = (ver > 0);
                isWalkingBack = (ver < 0);
            }
        }
        /*if (ver < 0)
        {
            //transform.Translate(transform.forward * ver * speed / 2 * Time.deltaTime, Space.World);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * ver * speed / 2 * Time.deltaTime);
        }*/
        if (hor != 0)
        {
            //transform.RotateAround(transform.position, Vector3.up, hor * turnSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + hor * turnSpeed * Time.deltaTime, 0);
        }

        m_Animator.SetBool("IsWalkingForward", isWalkingForward);
        m_Animator.SetBool("IsRunningForward", isRunningForward);
        m_Animator.SetBool("IsWalkingBack", isWalkingBack);
        m_Animator.SetBool("IsShooting", isShooting);

        

    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
            Death();
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

    public bool GetHealth(int heal)
    {
        if (hp == maxHp)
            return false;
        else
        {
            hp += heal;
            if (hp >= maxHp)
                hp = maxHp;
            return true;
        }
    }
}

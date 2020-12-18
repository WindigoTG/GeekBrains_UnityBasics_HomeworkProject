using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private List<object> inventory = new List<object>();

    private Transform respawnPosition = null;
    private bool checkpointSet = false;

    private GameObject[] pickups;
    private GameObject[] spawns;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Vector3 m_SideMovement;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        pickups = GameObject.FindGameObjectsWithTag("Pickup");
        spawns = GameObject.FindGameObjectsWithTag("MonsterSpawner");
    }


    void Update()
    {
        var ver = Input.GetAxis("Vertical");
        var hor = Input.GetAxis("Horizontal");
        var stre = Input.GetAxis("Streif");
        m_Movement = transform.forward;
        m_SideMovement = transform.right;

        bool isWalkingForward = false;
        bool isRunningForward = false;
        bool isWalkingBack = false;
        bool isShooting = false;
        bool isWalkingRight = false;
        bool isWalkingLeft = false;

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
        if (stre != 0)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_SideMovement * stre * speed / 2 * Time.deltaTime);
            if (ver == 0)
            {
                isWalkingRight = (stre > 0);
                isWalkingLeft = (stre < 0);
            }
        }
        if (hor != 0)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + hor * turnSpeed * Time.deltaTime, 0);
        }

        m_Animator.SetBool("IsWalkingForward", isWalkingForward);
        m_Animator.SetBool("IsRunningForward", isRunningForward);
        m_Animator.SetBool("IsWalkingBack", isWalkingBack);
        m_Animator.SetBool("IsShooting", isShooting);
        m_Animator.SetBool("IsWalkingRight", isWalkingRight);
        m_Animator.SetBool("IsWalkingLeft", isWalkingLeft);


    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
            Death();
    }

    private void Death()
    {
        //gameObject.SetActive(false);
        if (checkpointSet)
        {
            m_Rigidbody.position = respawnPosition.position;
            m_Rigidbody.rotation = respawnPosition.rotation;
            hp = maxHp;
            foreach (GameObject s in spawns)
                s.SetActive(true);
            foreach (GameObject p in pickups)
                p.SetActive(true);
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject m in monsters)
                Destroy(m.gameObject);
        }
        else
            SceneManager.LoadScene(0);
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

    public void AddToInventory(GameObject obj)
    {
        inventory.Add(obj);
    }

    public bool FindInInventory(string tag)
    {
        foreach (GameObject obj in inventory)
            if (obj.CompareTag(tag))
                return true;
        return false;
    }

    public void SetSpawn(Transform chkPoint)
    {
        respawnPosition = chkPoint;
        checkpointSet = true;
    }
}

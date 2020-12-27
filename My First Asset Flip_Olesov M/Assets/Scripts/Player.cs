using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, ITakeDamage
{
    [Header("Movement")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float turnSpeed = 20;

    [Header("Shooting")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float bulletSpeed = 3;
    [SerializeField] private float fireRate = 3;

    [Header("Stats")]
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

    private bool isDead;

    bool isJumping = false;
    bool isTurningAround = false;

    bool isGettingPressed = false;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        pickups = GameObject.FindGameObjectsWithTag("Pickup");
        spawns = GameObject.FindGameObjectsWithTag("MonsterSpawner");
    }


    void Update()
    {
        if (!isDead)
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

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                m_Rigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
                isJumping = true;
                Invoke("Landing", 1);
            }

            if (Input.GetKeyDown(KeyCode.R) && !isTurningAround)
            {
                StartCoroutine(TurnAround(transform.rotation, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + Mathf.Deg2Rad * 180, 0))));
            }

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

            if (ver != 0)
            {
                if (Input.GetAxis("Fire3") > 0 && ver > 0 && !isShooting)
                {
                    m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * ver * speed * 2.5f * Time.deltaTime);
                    isRunningForward = true;
                }
                else if (!isGettingPressed)
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
            if (hor != 0 && !isTurningAround)
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
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            m_Animator.SetTrigger("Death");
            isDead = true;
            Invoke("Death", 3);
        }
    }

    private void Death()
    {
        if (checkpointSet)
        {
            m_Rigidbody.position = respawnPosition.position;
            m_Rigidbody.rotation = respawnPosition.rotation;
            hp = maxHp;
            m_Animator.SetTrigger("Reset");
            isDead = false;
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

    public int HPLeft()
    {
        return hp;
    }

    void Landing()
    {
        isJumping = false;
    }

    IEnumerator TurnAround(Quaternion initRotation, Quaternion endRotation)
    {
        isTurningAround = true;

        var tr = transform;
        var e = tr.eulerAngles;
        if (e.y >=0 && e.y < 180)
            e.y += 180.0f;
        else if (e.y < 360 && e.y >= 180)
            e.y -= 180.0f;

        while (true)
        {
            if (tr.eulerAngles.y - e.y > -1 && tr.eulerAngles.y - e.y < 1)
            {
                tr.eulerAngles = e;
                isTurningAround = false;
                break;
            }
            else
            {
                tr.eulerAngles = Vector3.Lerp(tr.eulerAngles, e, turnSpeed / 10 * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void SetPressed(bool pr)
    {
        isGettingPressed = pr;
    }

    public bool CheckPressed()
    {
        return isGettingPressed;
    }
}

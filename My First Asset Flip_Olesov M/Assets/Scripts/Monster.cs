using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1.75f;
    [SerializeField] private float chaseSpeed = 5.5f;
    //[SerializeField] private float turnSpeed = 20;

    [Header("Stats")]
    [SerializeField] private int hp = 100;
    [SerializeField] private int damage = 15;

    [Header("Navigation")]
    [SerializeField] private float backToPatrolTime = 5;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private float attackCoolDown = 1;
    private float currentCoolDown = 0;


    //Переменные для грубого расчета попадания по игроку. Переменные отвечают за учет времени с момента начала атаки до момента когда атака считается нанесенной
    private float attackHitIn = 0.5f;  
    private float timeToHit = 0;
    private bool isAttacking = false;

    private GameObject player;
    int m_CurrentWaypointIndex;
    bool isAlert = false;
    bool isAlive = true;

    bool playerInRange;
    bool playerIsSeen;

    Animator m_Animator;

    public void Init(Transform[] waypoints)
    {
        this.waypoints = waypoints;
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        navMeshAgent.SetDestination(waypoints[0].position);
        m_Animator.SetBool("IsWalking", true);
        navMeshAgent.speed = speed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (isAlive)
        {
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !isAlert)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            if (!isAlert && playerInRange)
            {
                Vector3 direction = (player.transform.position + new Vector3(0, 2, 0)) - (transform.position + new Vector3(0, 2, 0));

                Ray ray = new Ray(transform.position + new Vector3(0, 2, 0), direction);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
                {
                    if (raycastHit.collider.gameObject == player)
                    {
                        isAlert = true;
                    }
                }
            }
            if (isAlert)
            {
                Vector3 direction = (player.transform.position + new Vector3(0, 2, 0)) - (transform.position + new Vector3(0, 2, 0));

                Ray ray = new Ray(transform.position + new Vector3(0, 2, 0), direction);
                Debug.DrawRay(transform.position + new Vector3(0, 2, 0), direction, Color.green);
                RaycastHit raycastHit;
                
                if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
                {
                    if (raycastHit.collider.gameObject == player)
                    {
                        playerIsSeen = true;
                    }
                    else
                    {
                        playerIsSeen = false;
                    }
                    /*Debug.LogWarning(player.transform.position);
                    Debug.LogWarning(transform.position);
                    Debug.LogWarning(raycastHit.collider.name);
                    Debug.LogWarning(ray.origin);
                    Debug.LogWarning(raycastHit.point);*/
                    
                }
                Debug.Log(playerIsSeen);

                if (playerIsSeen)
                {
                    navMeshAgent.SetDestination(player.transform.position);
                    CancelInvoke("BackToPatrol");
                }

                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && currentCoolDown <= 0)
                {
                    m_Animator.SetBool("IsWalking", false);
                    m_Animator.SetBool("IsRunning", true);
                    navMeshAgent.speed = chaseSpeed;
                }

                //Debug.LogWarning(playerInRange);
                //Debug.LogWarning(currentCoolDown);
                //Debug.LogWarning(navMeshAgent.remainingDistance);

                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && playerInRange && playerIsSeen && raycastHit.collider.gameObject == player && raycastHit.distance <= navMeshAgent.stoppingDistance)
                {
                    m_Animator.SetBool("IsWalking", false);
                    m_Animator.SetBool("IsRunning", false);
                    //navMeshAgent.speed = 0;
                    if (currentCoolDown <= 0)
                    {
                        m_Animator.SetTrigger("Attack");
                        isAttacking = true;
                        currentCoolDown = attackCoolDown;
                    }
                }
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !playerIsSeen)
                {
                    m_Animator.SetBool("IsWalking", false);
                    m_Animator.SetBool("IsRunning", false);
                    Invoke("BackToPatrol", backToPatrolTime);
                }

            }
            if (currentCoolDown > 0)
                currentCoolDown -= Time.deltaTime;
            if (isAttacking)
            {
                timeToHit += Time.deltaTime;
                if (timeToHit >= attackHitIn)
                {
                    {
                        //Если игрок находится на определенном расстоянии от монстра, когда атака считается нанесённой, игрок получает урон
                        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                        {
                            player.GetComponent<Player>().TakeDamage(damage);
                        }
                    }

                    timeToHit = 0;
                    isAttacking = false;
                }
            }
        }
        if (!isAlive)
            navMeshAgent.speed = 0;
        if (player.GetComponent<Player>().HPLeft() <= 0)
        {
            BackToPatrol();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlert)
            isAlert = true;
        hp -= damage;
        if (hp <= 0 && isAlive)
            Death();
    }

    private void Death()
    {
        isAlive = false;
        m_Animator.SetTrigger("IsDead");
        Destroy(gameObject, 1.5f);
    }

    private void BackToPatrol()
    {
        isAlert = false;
        navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        m_Animator.SetBool("IsWalking", true);
        navMeshAgent.speed = speed;
    }
}

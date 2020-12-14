using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] private float speed = 1.75f;
    [SerializeField] private float chaseSpeed = 5.5f;
    [SerializeField] private float turnSpeed = 20;

    [SerializeField] private int hp = 100;
    [SerializeField] private int damage = 15;

    [SerializeField] private Transform[] waypoints;
    [SerializeField]private NavMeshAgent navMeshAgent;

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
        //m_Animator.SetBool("IsWalking", true);
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !isAlert)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
        if (isAlert)
        {
            navMeshAgent.SetDestination(player.transform.position);
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && currentCoolDown <= 0)
            {
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetBool("IsRunning", true);
                navMeshAgent.speed = chaseSpeed;
            }
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetBool("IsRunning", false);
                navMeshAgent.speed = 0;
                if (currentCoolDown <= 0)
                {
                    m_Animator.SetTrigger("Attack");
                    isAttacking = true;
                    currentCoolDown = attackCoolDown;
                }
            }
        }
        if (currentCoolDown > 0)
            currentCoolDown -= Time.deltaTime;
        if (isAttacking)
        {
            timeToHit += Time.deltaTime;
            if (timeToHit >= attackHitIn)
            {
                //Если игрок находится на определенном расстоянии от монстра, когда атака считается нанесённой, игрок получает урон
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                    player.GetComponent<Player>().TakeDamage(damage);
                timeToHit = 0;
                isAttacking = false;
            }
        }
        if (!isAlive)
            navMeshAgent.speed = 0;
        if (!player.activeInHierarchy)
        {
            isAlert = false;
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            m_Animator.SetBool("IsWalking", true);
            navMeshAgent.speed = speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
            isAlert = true;
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
}

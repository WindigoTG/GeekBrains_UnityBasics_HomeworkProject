using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 0;
    [SerializeField] private float speed = 0;
    public void Init(int damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
//        if (other.CompareTag("Environment") || other.CompareTag("Enemy"))
        if (!other.isTrigger)
        {
            if (other.CompareTag("Enemy"))
                other.GetComponent<Monster>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
}

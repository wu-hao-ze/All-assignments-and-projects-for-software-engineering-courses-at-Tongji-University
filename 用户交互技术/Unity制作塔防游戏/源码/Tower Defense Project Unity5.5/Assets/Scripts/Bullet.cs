using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage = 50;

    public float speed = 20;

    public GameObject explosionEffectPrefab;

    private float distanceArriveTarget = 1.2f;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        this.target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Die();
            return;
        }

        transform.LookAt(target.position);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        Vector3 dir = target.position - transform.position;
        if (dir.magnitude < distanceArriveTarget)
        {
            target.GetComponent<Enemy>().TakeDamage(damage);
            Die();   
        }
    }

    void Die()
    {
        GameObject effect = GameObject.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        Destroy(effect, 1);
        Destroy(this.gameObject);
    }

}

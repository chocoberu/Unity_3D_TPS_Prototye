using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : LivingEntity
{
    Poolable pool;
    Rigidbody rb;
    // Start is called before the first frame update

    void Awake()
    {
        pool = GetComponent<Poolable>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        SpawningPool sp = FindObjectOfType<SpawningPool>();
        onDeath += sp.PopEnemyCount;
    }
    void OnEnable()
    {
        maxHealth = 20.0f;
        Health = maxHealth;
        Dead = false;
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("Enemy Die!");
        //GameObject.Destroy(this.gameObject, 3.0f);

        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;

        GameManager.Pool.Push(pool);
    }
}

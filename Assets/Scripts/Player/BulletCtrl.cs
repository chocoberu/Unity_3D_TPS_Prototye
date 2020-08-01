using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f; // 총알의 파괴력
    public float speed = 1000.0f; // 총알 발사 속도
    public LivingEntity Owner { get; set; }

    Poolable pool;

    void Awake()
    {
        pool = GetComponent<Poolable>();
    }
    
    public void ShotBullet()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        StartCoroutine(DestroyBullet());
    }
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(3.0f);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Pool.Push(pool);
    }
    private void ShootLivingEntity()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameManager.Pool.Push(pool);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //GetComponent<Rigidbody>().velocity = Vector3.zero; // 테스트용 임시 코드

        GameObject go = collision.gameObject;
        Debug.Log(go.name);
        LivingEntity le = go.GetComponent<LivingEntity>();
        if(le != null && le != Owner) // 맞은 대상이 플레이어가 아닌 생명체일 때
        {
            // 데미지
            le.OnDamage(damage, collision.transform.position, collision.transform.up);
            // 생명체에게 맞았을 때 총알 바로 제거
            ShootLivingEntity(); 
        }
        
    }
}

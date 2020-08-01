// 유니티 프로그래밍 에센스의 코드를 수정해서 사용
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float Health { get; protected set; } // 현재 체력
    protected float maxHealth; // 최대 체력(초기 체력)
    public bool Dead { get; protected set; } // 생명체가 죽었는지
    public event Action onDeath; // 생명체가 죽었을 때 발동할 이벤트

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Health -= damage;

        Debug.Log($"{this.gameObject.name} Damaged, current hp : {Health}");
        if(Health <= 0.0f && !Dead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (onDeath != null)
            onDeath();

        Dead = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f; // 총알의 파괴력
    public float speed = 1000.0f; // 총알 발사 속도

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(this.gameObject, 3.0f);
    }
}

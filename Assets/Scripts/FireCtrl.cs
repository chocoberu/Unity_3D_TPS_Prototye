using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class FireCtrl : MonoBehaviour
{
    public delegate void Test();

    public GameObject bullet; // 총알 프리팹
    public Transform firePos; // 총알 발사 좌표
    public OnScreenButton fireButton; // 총알 발사 버튼
    PlayerCtrl playerCtrl;
    bool beforePressButton;
    bool isReloading;
    bool isFire;
    bool rotateComplete;

    float timeBetFire = 0.2f; // 총알 발사 간격
    float lastFireTime;

    
    private void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        isReloading = false;
        isFire = false;
        beforePressButton = false;
        rotateComplete = false;

        lastFireTime = 0.0f;
    }
    //Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            isFire = true;
            playerCtrl.SetReadyToFire(); // 이동 중에 쏠때는 어떻게 할지 수정 필요
        }
        if(Input.GetMouseButtonUp(0)) // 버튼 땠을 때
        {
            playerCtrl.SetFinishedFire();
            isFire = false;
        }

#elif UNITY_ANDROID
        // 현재 연사 기능 X
        float data = (float)fireButton.control.ReadValueAsObject();

        if(data > 0.5f && !beforePressButton)
        {
            beforePressButton = true;
            isFire = true;
            playerCtrl.SetReadyToFire();
        }
        if (data < 0.1f)
        {
            beforePressButton = false;
            isFire = false;
        }
#endif
        if (isFire && rotateComplete && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Fire();
            //isFire = false;
            //rotateComplete = false;
        }
    }
    public void Fire()
    {
        if(isFire)
            Instantiate(bullet, firePos.position, firePos.rotation); // Bullet 프리팹을 동적 생성
    }
    public void SetRotateComplete()
    {
        rotateComplete = true;
    }
}

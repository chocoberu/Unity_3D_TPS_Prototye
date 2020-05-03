using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet; // 총알 프리팹
    public Transform firePos; // 총알 발사 좌표
    public OnScreenButton fireButton; // 총알 발사 버튼
    bool beforePressButton;
    bool isReloading;
    bool isFire;
    
    private void Start()
    {
        isReloading = false;
        isFire = false;
        beforePressButton = false;
    }
    // Update is called once per frame
    void Update()
    {
        float data = (float)fireButton.control.ReadValueAsObject();        
        if(data > 0.5f && !beforePressButton)
        {
            beforePressButton = true;
            isFire = true;
        }
        if (data < 0.1f)
        {
            beforePressButton = false;
            isFire = false;
        }
        // 마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
#if UNITY_EDITOR
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            Fire();
        }
#elif UNITY_ANDROID
        if (isFire)
        {
            Fire();
            isFire = false;
        }
#endif
    }
    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation); // Bullet 프리팹을 동적 생성
    }
}

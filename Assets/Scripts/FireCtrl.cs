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
    PlayerCtrl playerCtrl; // 플레이어 컨트롤러 컴포넌트
    bool beforePressButton; // 이전에 버튼을 클릭했는지 체크
    bool isReloading; // 현재 사용하지 않음
    bool isFire; // 현재 총알 발사 중인지 체크
    bool rotateComplete; // 플레이어의 회전이 완료 되었는지 체크

    float timeBetFire = 0.2f; // 총알 발사 간격
    float lastFireTime; // 마지막으로 총알을 발사한 시간을 체크

    public bool IsFire { get { return isFire; } }
    
    private void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        isReloading = false;
        isFire = false;
        rotateComplete = false;
        beforePressButton = false;

        lastFireTime = 0.0f;
    }
    //Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            isFire = true;
            // TODO : 이동 중에 쏠때는 어떻게 할지 수정 필요
            // TODO : 코드 구조를 간결하게 수정 필요
            //playerCtrl.SetReadyToFire(); 
        }
        if(Input.GetMouseButtonUp(0)) // 버튼 땠을 때, 사격 중지 상태
        {
            //playerCtrl.SetFinishedFire();
            isFire = false;
            rotateComplete = false;
        }

#elif UNITY_ANDROID
        // 현재 연사 기능 X
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
            rotateComplete = false;
        }
#endif
        if (isFire && rotateComplete && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Fire();
        }
    }
    public void Fire()
    {
        // TODO : 오브젝트 풀링 적용 필요
        if (isFire)
        {
            //Instantiate(bullet, firePos.position, firePos.rotation); // Bullet 프리팹을 동적 생성
            if(GameManager.Pool.GetOriginal(bullet.name) == null)
            {
                GameManager.Pool.CreatePool(bullet, 20);
            }
            Poolable poolable = GameManager.Pool.Pop(bullet);
            GameObject go = poolable.gameObject;
            go.transform.position = firePos.position;
            go.transform.rotation = firePos.rotation;
            go.GetComponent<BulletCtrl>().ShotBullet();
        }
    }
    public void SetRotateComplete()
    {
        rotateComplete = true;
    }
}

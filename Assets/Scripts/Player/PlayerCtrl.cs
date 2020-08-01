using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class PlayerCtrl : LivingEntity
{
    // 컴포넌트 관련
    private Rigidbody rb;
    private FireCtrl fireCtrl;
    public PlayerInput pis;
    public FollowCam cam;

    public float moveSpeed = 10.0f; // 이동속도
    public float rotSpeed = 80.0f; // 회전속도
    public float jumpPower = 5.0f; // 점프 파워

    // 마우스/오른쪽 아날로그 값 관련 변수
    float r = 0.0f;
    float q = 0.0f;

    public float R { get { return r; } }
    public float Q { get { return q; } }
        
    Vector2 moveValue; // InputSystem에서 받아오는 값
    Vector3 moveDir; // 움직이는 방향
    Vector3 pDir; // 플레이어가 바라보는 방향 벡터
    public Transform cameraTr; // 카메라 위치
    private Vector3 cameraZ; // 카메라 forward의 사영 벡터
    float minInputValue;

    // 점프 관련 변수
    bool isJump;
    bool _isJump;

    // 가상 조이스틱 관련
    public OnScreenStick leftStick;
    public OnScreenStick rightStick;

    // Start is called before the first frame update
    private void Awake()
    {
        pis = new PlayerInput();
        pis.Ingame.Move.performed += SetMoveValue;
        pis.Ingame.Move.canceled += SetMoveValueZero;

        isJump = _isJump = false;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fireCtrl = GetComponent<FireCtrl>();

        pDir = transform.forward;
        minInputValue = 0.2f;
        maxHealth = 100.0f;
        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("h : " + moveValue.x + " v : " + moveValue.y);
        //Debug.Log("pDir : " + pDir.normalized);
        Vector2 ldata, rdata;
        cameraZ = cameraTr.forward;
        cameraZ.Set(cameraZ.x, 0.0f, cameraZ.z);
        cameraZ = cameraZ.normalized;

        // TODO : 모바일에서 점프 추가 (현재 모바일에선 불가)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isJump == false)
                isJump = _isJump = true;
        }
#if UNITY_EDITOR
        ldata = rdata = Vector2.zero;
        r = Input.GetAxisRaw("Mouse X");
        q = Input.GetAxisRaw("Mouse Y");
#elif UNITY_ANDROID
        ldata = (Vector2)leftStick.control.ReadValueAsObject();
        moveValue = ldata;
        rdata = (Vector2)rightStick.control.ReadValueAsObject();
        r = rdata.x;
        q = rdata.y;
#endif

    }
    private void FixedUpdate()
    {
        Turn();
        Move();
        Jump();
    }
    private void OnEnable()
    {
        pis.Enable();
    }
    private void OnDisable()
    {
        pis.Disable();
    }
    private void SetMoveValue(InputAction.CallbackContext ctx)
    {
        moveValue = ctx.ReadValue<Vector2>();
    }
    void Move()
    {
        if (moveValue.sqrMagnitude > minInputValue)
        {
            if (moveValue.y > minInputValue) // 상
            {
                moveDir = cameraZ;
            }
            if (moveValue.y < -minInputValue) // 하
            {
                moveDir = -cameraZ;
            }

            if (moveValue.x > minInputValue || moveValue.x < -minInputValue) // 좌우 이동
            {
                // TODO : 임시로 좌우로 움직이게 설정, 원운동으로 수정 필요
                if (!fireCtrl.IsFire)
                    moveDir += pDir;
                else
                {
                    if(moveValue.x > minInputValue)
                        moveDir += cameraTr.right.normalized;
                    else if(moveValue.x < -minInputValue)
                        moveDir -= cameraTr.right.normalized;
                }
            }
            moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + moveDir);
        }
        else
        {
            moveValue = Vector2.zero;
        }
    }
    void Turn()
    {
        Vector3 prevDir = pDir; // 이전 방향 값을 저장
        bool modified = false; // 방향 값이 바꼈는지 확인
        pDir = Vector3.zero;
        
        if (!fireCtrl.IsFire) // 총알을 쏘지 않을 때 플레이어 방향
        {
            if (moveValue.x > minInputValue) // 오른쪽
            {
                pDir = cameraTr.right.normalized;
                modified = true;
            }
            if (moveValue.x < -minInputValue) // 왼쪽
            {
                pDir = -cameraTr.right.normalized;
                modified = true;
            }
            if (moveValue.y > minInputValue)
            {
                pDir += cameraZ;
                modified = true;
            }
            if (moveValue.y < -minInputValue)
            {
                pDir -= cameraZ;
                modified = true;
            }
        }
        else // 총알을 쏠 때, pDir == 카메라 방향
        {
            pDir = cameraZ;
            modified = true;
        }

        // pDir이 그대로라면
        if (modified == false)
        {
            pDir = prevDir;
            return;
        }
        
        //Debug.Log("pDir : " + pDir.normalized);
        Quaternion rot = Quaternion.LookRotation(pDir.normalized);
        rb.rotation = Quaternion.Slerp(rb.rotation, rot, rotSpeed * Time.deltaTime);
    }
    void Jump()
    {
        if (_isJump)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            _isJump = false;
        }
    }
    private void OnCollisionEnter(Collision coll)
    {
        //if (isJump == true)
        //  isJump = false;
        if (coll.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
    void SetMoveValueZero(InputAction.CallbackContext ctx)
    {
        // Input 입력 값을 0으로 초기화
        moveValue = Vector3.zero;
    }
    public void SetPlayerRotationCam(Vector3 newPos)
    {
        newPos.Set(newPos.x, 0.0f, newPos.z);
        transform.forward = newPos;
        fireCtrl.SetRotateComplete();
    }
    //public void SetReadyToFire()
    //{
    //    cam.SetFireButtonClicked();
    //}
    public override void Die()
    {
        base.Die();

        GameObject.Destroy(this.gameObject, 5.0f);
    }
}

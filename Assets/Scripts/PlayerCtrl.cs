using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody rb;
    private FireCtrl fireCtrl;
    public PlayerInput pis;
    public FollowCam cam;
    public float moveSpeed = 10.0f; // 이동속도
    public float rotSpeed = 80.0f; // 회전속도
    public float jumpPower = 5.0f; // 점프 파워
    Vector2 moveValue; // InputSystem에서 받아오는 값
    Vector3 moveDir; // 움직이는 방향
    Vector3 pDir; // 플레이어가 바라보는 방향
    public Transform cameraTr;
    private Vector3 cameraZ;
    float minInputValue;
    bool isFire;

    bool isJump;
    bool _isJump;

    public OnScreenStick leftStick;

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
        isFire = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("h : " + moveValue.x + " v : " + moveValue.y);
        //Debug.Log("pDir : " + pDir.normalized);
        Vector2 data;
        cameraZ = cameraTr.forward;
        cameraZ.Set(cameraZ.x, 0.0f, cameraZ.z);
        cameraZ = cameraZ.normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isJump == false)
                isJump = _isJump = true;
        }
#if UNITY_EDITOR
        data = Vector2.zero;
#elif UNITY_ANDROID
        data = (Vector2)leftStick.control.ReadValueAsObject();
        moveValue = data;
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
                // 임시로 좌우로 움직이게 설정, 원운동으로 수정 필요
                if (!isFire)
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
        if (moveValue.sqrMagnitude < minInputValue)
        {
            return;
        }
        pDir = Vector3.zero;
        // 좌우 회전
        if (!isFire)
        {
            if (moveValue.x > minInputValue) // 오른쪽
                pDir = cameraTr.right.normalized;
            if (moveValue.x < -minInputValue) // 왼쪽
                pDir = -cameraTr.right.normalized;
        }
        else
            pDir = transform.forward;
        // 상하 회전
        if (moveValue.y > minInputValue)
            pDir += cameraZ;
        if (moveValue.y < -minInputValue)
            pDir -= cameraZ;
        Debug.Log("pDir : " + pDir.normalized);
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
        moveValue = Vector3.zero;
    }
    public void SetPlayerRotationCam(Vector3 newPos)
    {
        newPos.Set(newPos.x, 0.0f, newPos.z);
        transform.forward = newPos;
        fireCtrl.SetRotateComplete();
    }
    public void SetReadyToFire()
    {
        cam.SetFirebuttonClicked();
        isFire = true;
    }
    public void SetFinishedFire()
    {
        isFire = false;
    }
}

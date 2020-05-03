using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerInput pis;
    public float moveSpeed = 10.0f; // 이동속도
    public float rotSpeed = 80.0f; // 회전속도
    Vector2 moveValue; // InputSystem에서 받아오는 값
    Vector3 moveDir;
    Vector3 pDir;
    public Transform cameraTr;
    private Vector3 cameraZ;
    float minInputValue;

    public OnScreenStick leftStick;

    // Start is called before the first frame update
    private void Awake()
    {
        pis = new PlayerInput();
        pis.Ingame.Move.performed += SetMoveValue;
        pis.Ingame.Move.canceled += SetMoveValueZero;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pDir = transform.forward;
        minInputValue = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("h : " + moveValue.x + " v : " + moveValue.y);
        //Debug.Log("pDir : " + pDir.normalized);
        Vector2 data = (Vector2)leftStick.control.ReadValueAsObject();
#if UNITY_EDITOR
        data = Vector2.zero;
#elif UNITY_ANDROID
        moveValue = data;
#endif
        cameraZ = cameraTr.forward;
        cameraZ.Set(cameraZ.x, 0.0f, cameraZ.z);
        cameraZ = cameraZ.normalized;
    }
    private void FixedUpdate()
    {
        Turn();
        Move();
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
                //moveDir.Set(0.0f, 0.0f, moveValue.y);  // 임시 이동 방식
                moveDir = cameraZ;
            }
            if (moveValue.y < -minInputValue) // 하
            {
                moveDir = -cameraZ;
            }

            if (moveValue.x > minInputValue || moveValue.x < -minInputValue)
            {
                //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
                // 임시로 좌우로 움직이게 설정, 원운동으로 수정 필요
                moveDir += pDir;
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
        if (moveValue.x > minInputValue)
            pDir = cameraTr.right.normalized;
        if (moveValue.x < -minInputValue)
            pDir = -cameraTr.right.normalized;
        if (moveValue.y > minInputValue)
            pDir += cameraZ;
        if (moveValue.y < -minInputValue)
            pDir -= cameraZ;
        //Debug.Log("pDir : " + pDir.normalized);
        Quaternion rot = Quaternion.LookRotation(pDir.normalized);
        rb.rotation = Quaternion.Slerp(rb.rotation, rot, rotSpeed * Time.deltaTime);
    }
    void SetMoveValueZero(InputAction.CallbackContext ctx)
    {
        moveValue = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerInput pis;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 50.0f; // 회전속도
    Vector2 moveValue;
    Vector2 mouseMove;
    Vector3 moveDir;
    Vector3 pDir;
    public float angle = 0.0f;
    public Transform cameraTr;
    private Vector3 cameraZ;

    public OnScreenStick leftStick;
    


    // Start is called before the first frame update
    private void Awake()
    {
        pis = new PlayerInput();
        pis.Ingame.Move.performed += SetMoveValue;
        pis.Ingame.Move.canceled += SetMoveValueZero;
        //pis.Ingame.MouseMove.performed += SetMouseMove;

    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pDir = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("h : " + moveValue.x + " v : " + moveValue.y);
        Debug.Log("pDir : " + pDir.normalized);

        Vector2 data = (Vector2)leftStick.control.ReadValueAsObject();
        moveValue = data;
        //cameraZ.Set(transform.position.x - cameraTr.position.x, 0.0f, transform.position.z - cameraTr.position.z);
        cameraZ = cameraTr.forward;
        cameraZ.Set(cameraZ.x, 0.0f, cameraZ.z);
        cameraZ = cameraZ.normalized;
        //Debug.Log("camZ : " + cameraZ);
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
        if (moveValue.sqrMagnitude > 0.01f)
        {
            if (moveValue.y > 0.1f) // 상
            {
                //transform.forward = cameraZ;
                //angle += moveValue.x * 0.05f;
                //angle = angle % (Mathf.PI * 2.0f); // 회전용
                //moveDir.Set(0.0f, 0.0f, moveValue.y);  // 임시 이동 방식
                moveDir = cameraZ;
               
            }
            if(moveValue.y < -0.1f) // 하
            {
               // transform.forward = -cameraZ;
                moveDir = -cameraZ;
                //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
                //rb.MovePosition(transform.position + moveDir);
            }

            if (moveValue.x > 0.01f || moveValue.x < -0.01f)
            {
                //moveDir.Set(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
                //moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
                // 임시로 좌우로 움직이게 설정, 원운동으로 수정 필요
                moveDir += pDir;
                //moveDir = pDir * moveSpeed * Time.deltaTime;
                //rb.MovePosition(transform.position + moveDir);
            }
            moveDir = moveDir.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + moveDir);
        }
        else
        {
            moveValue.Set(0.0f, 0.0f);
            angle = 0.0f;
        }
    }
    void Turn()
    {
        if (moveValue.sqrMagnitude < 0.1f)
        {
            return;
        }
        pDir = Vector3.zero;
        if (moveValue.x > 0.1f)
            pDir = cameraTr.right.normalized;
        if(moveValue.x < -0.1f)
            pDir = -cameraTr.right.normalized;
        if (moveValue.y > 0.1f)
            pDir += cameraZ;
        if (moveValue.y < -0.1f)
            pDir -= cameraZ;
        //Debug.Log("pDir : " + pDir.normalized);
        Quaternion rot = Quaternion.LookRotation(pDir.normalized);

        //rb.rotation = rot;
        rb.rotation = Quaternion.Slerp(rb.rotation, rot, rotSpeed * Time.deltaTime);
        
        
        //transform.rotation = rot;
        
    }
    //void SetMouseMove(InputAction.CallbackContext ctx)
    //{
    //    mouseMove = ctx.ReadValue<Vector2>();
    //}
    void SetMoveValueZero(InputAction.CallbackContext ctx)
    {
        moveValue.Set(0.0f, 0.0f);
        //angle = 0.0f;
        //Debug.Log("stop");
    }
}

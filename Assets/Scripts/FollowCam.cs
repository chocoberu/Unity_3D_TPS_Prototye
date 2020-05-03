using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;


public class FollowCam : MonoBehaviour
{
    public Transform target; // 추적할 대상
    public float moveDamping = 20.0f; // 이동 속도 계수
    public float rotateDamping = 10.0f; // 회전 속도 계수
    public float distance = 5.0f; // 추적 대상과의 거리
    public float height = 4.0f; // 추적 대상과의 높이
    public float targetOffset = 2.0f; // 추적 좌표의 오프셋
    public Vector3 initForward;
    private float r = 0.0f;
    private float q = 0.0f;
    private float yAngle = 0.0f;
    private float xAngle = 0.0f;
    public float maxXAngle = 25.0f;
    public float minXAngle = -5.0f;
    Vector3 xRotAxis;
    public OnScreenStick rightStick;

    void Start()
    {
        //tr = GetComponent<Transform>(); // CameraRig의 Transform 컴포넌트를 추출
        initForward = target.forward;
    }
    void Update()
    {
#if UNITY_EDITOR
        r = Input.GetAxisRaw("Mouse X");
        q = Input.GetAxisRaw("Mouse Y");
#elif UNITY_ANDROID
        Vector2 data = (Vector2)rightStick.control.ReadValueAsObject();
        r = data.x;
        q = data.y;
#endif

        //Debug.Log("r = " + r + " q = " + q);
        yAngle += r * 0.1f;
        yAngle = yAngle % (2 * Mathf.PI);
        xAngle += q * 1.2f;
        if (xAngle > maxXAngle)
            xAngle = maxXAngle;
        if (xAngle < minXAngle)
            xAngle = minXAngle;
        //Debug.Log("yAngle : " + yAngle + " xAngle : " + xAngle);
    }

    void LateUpdate()
    {
        //var camPos = target.position - (target.forward * distance) + (target.up * height); // 카메라의 높이와 거리를 계산
        var camPos = target.position - (initForward * distance) + (target.up * height); // 카메라의 높이와 거리를 계산

        //transform.position = Vector3.Slerp(transform.position, camPos, Time.deltaTime * moveDamping); // 이동할 때의 속도 계수를 적용
        transform.position = camPos;
        transform.RotateAround(target.position, target.up, yAngle); // 플레이어의 y축을 기준으로 카메라를 공전

        initForward.x = Mathf.Sin(yAngle);
        initForward.z = Mathf.Cos(yAngle);

        transform.LookAt(target.position + (target.up * targetOffset)); // 카메라의 Z축을 타겟의 위치로 설정
        xRotAxis.Set(transform.right.x, 0.0f, transform.right.z);
        transform.RotateAround(target.position + (target.up * targetOffset), xRotAxis, -xAngle); // 타켓 위치를 기준점으로 카메라의 x축 공전
                                                                                                 //Debug.Log("xRotAxis : " + xRotAxis);

    }

    void OnDrawGizmos() // 추적할 좌표를 시각적으로 표현
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f); // 추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position); // 메인 카메라와 추적 지점 간의 선을 표시
    }
}

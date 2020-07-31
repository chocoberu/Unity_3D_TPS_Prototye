// 절대강좌 유니티 교재의 FollowCam을 수정해서 사용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class FollowCam : MonoBehaviour
{
    public Transform target; // 추적할 대상 (플레이어)
    // 컴포넌트 관련
    public PlayerCtrl playerCtrl;
    public FireCtrl fireCtrl;
    public float moveDamping = 20.0f; // 이동 속도 계수
    public float rotateDamping = 10.0f; // 회전 속도 계수
    public float distance = 5.0f; // 추적 대상과의 거리
    public float height = 4.0f; // 추적 대상과의 높이
    public float targetOffset = 2.0f; // 추적 좌표의 오프셋

    public Vector3 initForward; // 초기 forward 값

    // 각도 관련
    private float yAngle = 0.0f;
    private float xAngle = 0.0f;
    float beforeXAngle = 0.0f;
    public float maxXAngle = 25.0f;
    public float minXAngle = -5.0f;
    Vector3 xRotAxis;
    public GameObject firePosObj; // 총구 게임 오브젝트 (임시용)
    //bool firebuttonClicked;

    public Image uiTest;

    Vector3 uiPosition; // 조준선 스크린 좌표을 구하기 위해 필요한 변수
    Vector3 camDir; // 카메라가 가리키는 방향을 projection 한 후 나타내는 변수 
    Vector3 fireDir; // 

    void Start()
    {
        //tr = GetComponent<Transform>(); // CameraRig의 Transform 컴포넌트를 추출
        initForward = target.forward;
        //firebuttonClicked = false;

        fireDir = firePosObj.transform.forward;
    }
    void Update()
    {
        
//#if UNITY_EDITOR
//        r = Input.GetAxisRaw("Mouse X");
//        q = Input.GetAxisRaw("Mouse Y");
//#elif UNITY_ANDROID
//        Vector2 data = (Vector2)rightStick.control.ReadValueAsObject();
//        r = data.x;
//        q = data.y;
//#endif

        //Debug.Log("r = " + r + " q = " + q);
        beforeXAngle = xAngle;
        yAngle += playerCtrl.R * 0.1f;
        yAngle = yAngle % (2 * Mathf.PI);
        xAngle += playerCtrl.Q * 1.2f;
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

        if (firePosObj != null)
        {
            firePosObj.transform.Rotate((beforeXAngle- xAngle) * 0.5f, 0.0f, 0.0f); // 총구 부분 회전

            // 조준선 UI 업데이트
            // TODO : 카메라 방향과 플레이어의 정면 방향이 다를 때 처리 필요 (아직 완성 X)
            
            Vector3 normal = Vector3.Cross(transform.forward, transform.up);
            camDir = -Vector3.Cross(normal, target.transform.up);
            float angle = Vector3.Angle(camDir, firePosObj.transform.forward);
            //Quaternion rot = Quaternion.AngleAxis(Vector3.Angle(normal, firePosObj.transform.forward), target.transform.up);
            Debug.Log(angle);

            Quaternion rot1;
            Quaternion rot2;

            rot1 = Quaternion.AngleAxis(angle, target.transform.up);
            rot2 = Quaternion.AngleAxis(-angle, target.transform.up);

            Vector3 temp1 = (rot1 * firePosObj.transform.forward).normalized;
            Vector3 temp2 = (rot2 * firePosObj.transform.forward).normalized;

            if (Vector3.Angle(camDir, temp1) < Vector3.Angle(camDir, temp2))
            {
                fireDir = temp1;
            }
            else
            {
                fireDir = temp2;
            }
            
            //fireDir = Vector3.ProjectOnPlane(firePosObj.transform.forward, normal).normalized;
            
            uiPosition = Camera.main.WorldToScreenPoint(firePosObj.transform.position + fireDir);
            
            //Debug.Log(target.transform.position + " " + firePosObj.transform.position);
            //Debug.Log(Vector3.Angle(target.transform.forward, firePosObj.transform.forward));
            uiTest.transform.localPosition = new Vector3(0.0f, Screen.height * 0.5f - uiPosition.y, 0.0f);
            
        }
        
        if(fireCtrl.IsFire) // 총알 발사 버튼이 눌렸을 때
        {
            playerCtrl.SetPlayerRotationCam(transform.forward);
            //firebuttonClicked = false;
        }
        
    }

    void OnDrawGizmos() // 추적할 좌표를 시각적으로 표현
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f); // 추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position); // 메인 카메라와 추적 지점 간의 선을 표시

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(firePosObj.transform.position, firePosObj.transform.position + fireDir * 10.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.transform.position, target.transform.position + Vector3.Cross(transform.forward, transform.up) * 10.0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePosObj.transform.position, firePosObj.transform.position + firePosObj.transform.forward * 10.0f);
    }
    //public void SetFireButtonClicked()
    //{
    //    firebuttonClicked = true;
    //}

    //private void OnGUI()
    //{
    //    GUI.Box(new Rect(Screen.width * 0.5f, uiPosition.y - 10.0f, 20.0f, 20.0f), "Test");
    //}
}

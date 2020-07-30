// 인프런 유니티 강좌 코드를 수정해서 사용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager s_instance = null;
    public static GameManager Instance { get { Init(); return s_instance; } }
    public GameObject mobileJoystick;

    PoolManager _pool = new PoolManager();

    public static PoolManager Pool { get { return Instance._pool; } }

    void Start()
    {
        Init();
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        mobileJoystick.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS
        mobileJoystick.SetActive(true);
#endif
    }
    static void Init()
    {
        if (s_instance == null)
        {
            // 초기화
            GameObject gObj = GameObject.Find("GameManager");

            if (gObj == null) // 하이어라키에 게임 매니저가 없다면
            {
                gObj = new GameObject { name = "GameManager" };
                gObj.AddComponent<GameManager>();
            }
            DontDestroyOnLoad(gObj);
            s_instance = gObj.GetComponent<GameManager>();
        }
        s_instance._pool.Init();
    }
    public static void Clear()
    {
        Pool.Clear();
    }
}

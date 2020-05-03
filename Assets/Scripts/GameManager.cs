using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager s_instance = null;
    public static GameManager Instance { get { Init(); return s_instance; } }
    // Start is called before the first frame update
    void Start()
    {
        Init();
        Application.targetFrameRate = 60;
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
    }
}

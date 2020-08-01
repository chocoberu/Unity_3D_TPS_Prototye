using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawningPool : MonoBehaviour
{
    public int maxCount; // 최대 생성할 적의 개수
    int curCount = 0; // 현재 생존한 적의 개수
    Vector3 spawnPos; // 스폰 위치
    float spawnTime;
    public GameObject enemy; // 적 프리팹
    GameObject root; // 적을 모아둘 empty 게임오브젝트
    bool isSpawning = false;

    public Text enemyUI;

    // Start is called before the first frame update
    void Start()
    {
        if(root == null)
        {
            root = new GameObject { name = "Enemy_Root" };
        }
        if(GameManager.Pool.GetOriginal(enemy.name) == null)
        {
            GameManager.Pool.CreatePool(enemy, maxCount);
        }
        spawnTime = 0.15f;
        StartCoroutine(SpawnEnemy());
        isSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(curCount <= 0 && !isSpawning)
        {
            spawnTime = Random.Range(0.5f, 2.0f);
            StartCoroutine(SpawnEnemy());
            isSpawning = true;
        }
    }
    IEnumerator SpawnEnemy()
    {
        for (int i = curCount; i < maxCount; i++)
        {
            yield return new WaitForSeconds(spawnTime);
            Poolable poolable = GameManager.Pool.Pop(enemy, root.transform);
            GameObject go = poolable.gameObject;
            spawnPos.x = Random.Range(-25.0f, 25.0f);
            spawnPos.y = 1.5f;
            spawnPos.z = Random.Range(-5.0f, 65.0f);

            go.transform.position = spawnPos;
            curCount++;
            SetTextUI();
        }
        isSpawning = false;
    }
    public void PopEnemyCount()
    {
        curCount--;
        SetTextUI();
        Debug.Log($"current enemy : {curCount}");
    }
    void SetTextUI()
    {
        enemyUI.text = $"Enemy : {curCount}";
    }
}

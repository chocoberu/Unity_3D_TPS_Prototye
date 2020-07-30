// 인프런 유니티 강좌 코드를 사용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    class Pool
    {
        public GameObject Original { get; private set; }
        public GameObject Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 10)
        {
            Original = original;
            Root = new GameObject();
            Root.name = $"{original.name}_Root"; // 분류용 루트 게임 오브젝트

            for(int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            if (go.GetComponent<Poolable>() == null)
                go.AddComponent<Poolable>();

            return go.GetComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;
            poolable.transform.parent = Root.transform; // Poolable 객체를 루트의 하위로 옮김
            poolable.gameObject.SetActive(false); // 게임 오브젝트 비활성화
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }
        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0) // 풀링에 객체가 존재한다면
                poolable = _poolStack.Pop();
            else // 없다면
                poolable = Create();

            poolable.gameObject.SetActive(true); // 게임 오브젝트 활성화

            // DontDestroyOnLoad 해제용
            //if (parent == null)
              //  poolable.transform.parent = Manager.Scene.CurrentScene.transform;
            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    GameObject _root; // 모든 poolable 객체를 보관할 게임 오브젝트

    public void Init()
    {
        if (_root == null) // _root가 없다면 생성
        {
            _root = new GameObject { name = "@Pool_Root" };
            Object.DontDestroyOnLoad(_root);
        }
    }
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.transform.parent = _root.transform;

        _pool.Add(original.name, pool); // 딕셔너리에 pool 추가
    }
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false) // 딕셔너리에 해당 객체가 없다면
        {
            GameObject.Destroy(poolable.gameObject); // 해당 객체를 Destroy
            return;
        }
        _pool[name].Push(poolable); // 딕셔너리에 사용이 끝난 Poolable 추가
    }
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false) // 딕셔너리에 오리지날이 없다면
        {
            CreatePool(original);
        }
        return _pool[original.name].Pop(parent);
    }
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }
    public void Clear()
    {
        foreach (Transform child in _root.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        _pool.Clear();
    }
}

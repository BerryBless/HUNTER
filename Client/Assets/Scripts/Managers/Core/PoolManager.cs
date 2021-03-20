using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region POOL
    class Pool
    {
        // 원본
        public GameObject Original { get; private set; }
        // 모아둘곳
        public Transform Root { get; set; }
        // POOL
        Stack<Poolable> _poolStack = new Stack<Poolable>();

        // 초기화
        public void Init(GameObject original, int count = 10)
        {
            Original = original;                                // 원본을 저장하고
            Root = new GameObject().transform;                  // Pool_Root를 만들고
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)                           // count 만큼 풀링
            {
                Push(Create());
            }
        }
        // 복제
        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);   // 원본 복사해서
            go.name = Original.name;                                    // 이름바꾸고
            return Util.GetOrAddComponent<Poolable>(go);                // Poolable 반환 없음 달아주기
        }

        // POOL에 넣기
        public void Push(Poolable poolable)
        {
            if (poolable == null) return;                               // 풀링 대상이 아님

            poolable.transform.parent = Root;                           // Root 상속받고
            poolable.gameObject.SetActive(false);                       // 숨기기
            poolable.IsUsing = false;                                   // 사용안하는중! 표시


            _poolStack.Push(poolable);                                  // 풀에 넣기
        }

        // POOL 에서 빼오기
        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)                                   // 풀스택에 있으면
                poolable = _poolStack.Pop();                            // 빼오기
            else
                poolable = Create();                                    // 없으면 만들기

            poolable.gameObject.SetActive(true);                        // 사용! 게임상에 나타남

            poolable.transform.parent = parent;                         // 사용할 곳 에 붙기
            poolable.IsUsing = true;                                    // 사용중! 표시

            return poolable;
        }
    }
    #endregion

    // POOL을 관리할 POOLMANAGER
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>(); // <Pool.Original.Name , Pool>

    // Pool을! 모아둘곳
    Transform _root;
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;   // Pool을 모아둘 Root 생성
            Object.DontDestroyOnLoad(_root);
        }
    }
    // 풀생성
    public void CreatePool(GameObject original, int count = 10) {
        Pool pool = new Pool();
        pool.Init(original, count); // Pool 만들고
        pool.Root.parent = _root;   // _root에 상속시키기

        _pool.Add(original.name, pool); // 딕셔너리 등록
    }

    // 해당 풀에 넣기
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;                                 // 키 가져오기
        if (_pool.ContainsKey(name) == false) { GameObject.Destroy(poolable.gameObject); return; }  // 풀이 없으면 삭제

        _pool[name].Push(poolable);     // 풀이 있으면 풀에 push
    }

    // 해당 풀에서 빼오기
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);   // 풀없으면 만들기

        return _pool[original.name].Pop(parent); // 해당 풀에 접근해 pop호출하기
    }

    // 풀의 원본 찾기
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }

}

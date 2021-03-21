using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region sington
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    static void Init() { 
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            // 여기서 사용할 매니져 초기화
            s_instance._pool.Init();
        }
    }

    // 게임 콘텐츠에 관한 메니져
    #region Contents
    MapManager _map = new MapManager();

    public static MapManager Map { get { return Instance._map; } }
    #endregion

    // 게임 실행에 관한 메니져
    #region Core
    SceneManagerEx _scene = new SceneManagerEx();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    PoolManager _pool = new PoolManager();

    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static PoolManager Pool { get { return Instance._pool; } }

    #endregion


    public static void Clear()
    {
        // 여기서 사용할 매니져 클리어
        Scene.Clear();
        Input.Clear();
        Pool.Clear();
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Instance._input.UpdateInput();
    }
}

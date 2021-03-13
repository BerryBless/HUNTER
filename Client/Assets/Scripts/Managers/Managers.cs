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

        }
    }

    #region Core
    ResourceManager _resource = new ResourceManager();

    public static ResourceManager Resource { get { return Instance._resource; } }

    #endregion


    public static void Clear()
    {
        // 여기서 사용할 매니져 클리어
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
        
    }
}

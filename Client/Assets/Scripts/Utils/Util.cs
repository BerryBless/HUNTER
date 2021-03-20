using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // 복잡한데 자주 쓰는거 모아놓기

    // 컴포넌트를 찾고 널이면 추가
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    // 자식 찾기
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }
    // 자식중에 컴포넌트 찾기
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null) return null;
        if(recursive == true)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }

    // 딕셔너리 값있으면 true 없음 add
    public static bool TryAdd<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue addValue)
    {
        bool canAdd = !dict.ContainsKey(key);

        if (canAdd)
            dict.Add(key, addValue);

        return canAdd;
    }
}

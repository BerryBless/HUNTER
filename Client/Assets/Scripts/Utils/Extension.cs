using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    // 사용하기 편하게! 
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return go.GetOrAddComponent<T>();
    }
    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Util.FindChild<T>(go, name, recursive);
    }
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue)
    {
        return Util.TryAdd<TKey, TValue>(dict, key, addValue);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        BindEvent(go, action, type);
    }

}

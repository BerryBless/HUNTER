using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private Action KeyAction = null;

    public void UpdateInput()
    {
        if (Input.anyKey == false) return;

        if (KeyAction != null)
            KeyAction.Invoke();
    }

    public void Clear()
    {
        KeyAction = null;
    }

    // 키보드 액션 등록
    public void AddKeyAction(Action keyAction)
    {
        RemoveKeyAction(keyAction);         // 같은거 삭제하고
        KeyAction += keyAction;             // 새로 등록
    }
    // 키보드 액션 삭제
    public void RemoveKeyAction(Action keyAction)
    {
        KeyAction -= keyAction;
    }
}

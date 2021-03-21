using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    // Enum 모아두는 클래스
    // 상/하/좌/우 | 북/남/서/동 2차원 방향
    public enum MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }

    // 생물체의 State
    public enum CreatureState
    {
        Idle,
        Moving,
        Attack,
        Dead,
    }

    // 사용할 Scene의 name!
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game
    }
    // 오디오 소스 Type
    public enum AudioRole
    {
        Bgm,
        Effect,
        MaxCount,
    }
    // UI이벤트 구분
    public enum UIEvent
    {
        Click,
        Drag,
    }

}

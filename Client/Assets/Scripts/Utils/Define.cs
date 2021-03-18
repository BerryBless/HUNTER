using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    // 헌터에서 Enum 모아두는 클래스
    public enum MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum CreatureState
    {
        Idle,
        Moving,
        Attack,
        Dead,
    }
}

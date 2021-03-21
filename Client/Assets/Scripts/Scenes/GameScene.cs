using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override void Clear()
    {
        // TODO : 종료시 날려주는 부분
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);
        Managers.Resource.Instantiate("Creatures/Player/Player");
    }
}

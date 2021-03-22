using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override void Clear()
    {
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Lobby;
        //Managers.Audio.Play("Sounds/8-Bit Sfx/Climb_Rope_Loop_00",Define.AudioRole.Bgm,0.5f);
        Managers.Input.AddKeyAction(KeyInput);
    }
    private void Update()
    {
        
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //Managers.Audio.Play("Sounds/8-Bit Sfx/Craft_00");
        }
    }
}

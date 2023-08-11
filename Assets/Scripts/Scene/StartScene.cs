using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : BaseScene
{
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Scene = Define.Scenes.StartScene;
        Manager.Sound.Play("BGM/StartGame", Define.Sound.Bgm);
    }

    public override void Clear(Define.Scenes type)
    {
        base.Clear(type);
    }
}

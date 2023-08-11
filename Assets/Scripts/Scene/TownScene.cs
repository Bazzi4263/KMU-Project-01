using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Manager.Sound.Play("BGM/TownAfterBattle", Define.Sound.Bgm);
        Manager.UI.ShowSceneUI<UI_Artefact>();
    }
    public override void Clear(Define.Scenes type)
    {
        base.Clear(type);
    }
}

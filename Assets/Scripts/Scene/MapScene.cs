using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScene : BaseScene
{
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Manager.UI.ShowSceneUI<UI_Scene>("UI_MapScene");
        Manager.UI.ShowSceneUI<UI_Artefact>();
        Manager.UI.ShowPopUpUI<Info>();

        switch (MapManager.Instance.currentStage)
        {
            case Define.STAGE.GRASSLAND:
                Manager.Sound.Play("BGM/Stage1", Define.Sound.Bgm);
                break;
            case Define.STAGE.DESERT:
                //Manager.Sound.Play(Define.Sound.Bgm, "");
                break;
            case Define.STAGE.SNOWLAND:
                //Manager.Sound.Play(Define.Sound.Bgm, "");
                break;
        }
        //Manager.Data.MapSceneSave();

    }
    public override void Clear(Define.Scenes type)
    {
        base.Clear(type);
    }
}

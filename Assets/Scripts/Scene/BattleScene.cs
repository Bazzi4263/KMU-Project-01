using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : BaseScene
{
    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        switch (MapManager.Instance.currentStage)
        {
            case Define.STAGE.GRASSLAND:
                Manager.Sound.Play("BGM/NormalBattle", Define.Sound.Bgm);
                break;

            case Define.STAGE.DESERT:
                break;

            case Define.STAGE.SNOWLAND:
                break;

            case Define.STAGE.TOWN:
                break;
        }

        Manager.UI.ShowSceneUI<UI_Scene>("UI_BattleScene");
        Manager.UI.ShowSceneUI<UI_Artefact>();
        Manager.UI.ShowPopUpUI<Info>();
        //Manager.Data.AddPlayer("Warrior"); //테스트용 코드
        //Manager.Data.BattleSceneSave();
        //Manager.Data.SaveGameData();
        StartCoroutine(Manager.Battle.SetupBattle());
    }

    public override void Clear(Define.Scenes type)
    {
        Manager.Battle.EndBattle();
        base.Clear(type);
    }

    private void Update()
    {
        Manager.Battle.UpdateBattle();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleScene : UI_Scene
{
    enum GameObjects
    {
        UI_Panel,
        UI_SettingBar,
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject uiPanel = Get<GameObject>((int)GameObjects.UI_Panel);
        GameObject uiSettingBar = Get<GameObject>((int)GameObjects.UI_SettingBar);
        Util.GetOrAddComponent<UI_Panel>(uiPanel);
        Util.GetOrAddComponent<UI_SettingBar>(uiSettingBar);
    }
}

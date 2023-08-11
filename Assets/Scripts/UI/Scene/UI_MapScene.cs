using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapScene : UI_Scene
{
    enum GameObjects
    {
        UI_SettingBar
    }

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject uiSettingBar = Get<GameObject>((int)GameObjects.UI_SettingBar);
        Util.GetOrAddComponent<UI_SettingBar>(uiSettingBar);
    }
}

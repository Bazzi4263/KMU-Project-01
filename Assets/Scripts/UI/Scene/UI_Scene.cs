using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Scene : UI_Base
{
    protected override void Init()
    {
        Manager.UI.SetCanvas(gameObject, false);
        Bind<Image>(typeof(Define.Images));
        Bind<Text>(typeof(Define.Texts));
        Bind<Button>(typeof(Define.Buttons));
    }
}

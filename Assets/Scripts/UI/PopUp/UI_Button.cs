using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Scene
{
    private void Start()
    {
       Init();
    }

    protected override void Init()
    {
        base.Init();
        GameObject go = GetImage((int)Define.Images.StartIcon).gameObject;

        AddUIEvent(go, ((PointerEventData data) => { Manager.Scene.LoadScene(Define.Scenes.MapScene); }), Define.UIEvent.Click);
    }

    private void Update()
    {
        
    }

}

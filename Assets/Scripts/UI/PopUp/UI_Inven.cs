using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 맵씬과 전투씬에서 가방을 열 시 가지고 있는 아이템 목록을 보여준다
 */
public class UI_Inven : UI_PopUp
{
    enum GameObjects
    {
        Scroll
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        GameObject scroll = Util.FindChild(Get<GameObject>((int)GameObjects.Scroll), "Viewport");
        GameObject Contents = Util.FindChild(scroll, "Content");

        //가지고 있는 아이템 목록을 가져옴
        List<string> items = Manager.Item.CountExistItem();

        //자동화 코드
        foreach (string item in items)
        {
            GameObject itemicon = Manager.Resource.Instantiate("UI/PopUp/ItemIcon");
            itemicon.transform.SetParent(Contents.transform, false);
            Type type = Type.GetType(item);
            Util.GetOrAddComponent(itemicon, type);
        }

        if (Manager.UI.Issetting)
        {
            _out.transform.SetAsLastSibling();
            AddUIEvent(_cancel.gameObject, (PointerEventData) => { Manager.UI.Issetting = false; }, Define.UIEvent.Click);
        }
    }
}

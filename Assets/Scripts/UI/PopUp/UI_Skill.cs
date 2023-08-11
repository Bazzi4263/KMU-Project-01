using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Skill : UI_PopUp
{
    GameObject _Contents;
    Stat player = new Stat();

    enum GameObjects
    {
        GridPanel,
        Scroll
    }

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        GameObject scroll = Util.FindChild(Get<GameObject>((int)GameObjects.Scroll), "Viewport");
        _Contents = Util.FindChild(scroll, "Content");

        if (Manager.UI.Issetting)
        {
            _out.transform.SetAsLastSibling();
            player = Manager.Data.nowplayer;
            AddUIEvent(_cancel.gameObject, (PointerEventData) => { Manager.UI.Issetting = false; }, Define.UIEvent.Click);
        }

        else
        {
            player = Manager.Battle.nowTurnPlayer._stats;
        }

        foreach (Skill skill in player.skills.Values)
        {
            if (player.level >= skill.required_level)
            {
                GameObject skillicon = Manager.Resource.Instantiate("UI/PopUp/ItemIcon");
                RectTransform itemrect = skillicon.GetComponent<RectTransform>();
                skillicon.transform.SetParent(_Contents.transform, false);
                Type type = Type.GetType(skill.name);
                Util.GetOrAddComponent(skillicon, type);
            }
        }
    }
}

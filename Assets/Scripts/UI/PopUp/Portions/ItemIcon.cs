using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : Icon
{
    protected override void Init()
    {
        if (!Manager.UI.Issetting)
            AddUIEvent(gameObject, (PointerEventData) =>
            {
                Manager.Battle._itemdata = Manager.Item.items[this.GetType().Name];
                Manager.Battle._data = Manager.Battle._itemdata;
                Manager.Battle.OnTarget();
                if (Manager.Battle.state != Define.BattleState.SELECTINGPPTARGET) Manager.UI.ClosePopUpUI();
            }, Define.UIEvent.Click);
        
        base.Init();
        destext.text = Manager.Item.items[this.GetType().Name].description;
        image.sprite = Manager.Item.GetSprite(Manager.Item.items[this.GetType().Name].spritenum);
        numtext.text = Manager.Item.items[this.GetType().Name].pp.ToString();
    }
}

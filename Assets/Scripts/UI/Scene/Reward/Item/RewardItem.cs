using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : RewardIcon
{
    protected override void Init()
    {
        base.Init();
        string name = this.GetType().Name.Substring(6);
        _destext.text = Manager.Item.items[name].description;
        _image.sprite = Manager.Item.GetSprite(Manager.Item.items[name].spritenum);
        AddUIEvent(gameObject, (PointerEventData) => { Manager.Item.items[name].pp++; }, Define.UIEvent.Click);
    }
}

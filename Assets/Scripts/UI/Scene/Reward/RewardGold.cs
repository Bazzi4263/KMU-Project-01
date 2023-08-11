using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGold : RewardIcon
{
    protected override void Init()
    {
        base.Init();
        _destext.text = $"{Manager.Battle.gold} Gold";
        _image.sprite = Manager.Item.GetSprite(10);
        AddUIEvent(gameObject, (PointerEventData) => 
        { 
            Manager.Item.gold += Manager.Battle.gold;
            Manager.Battle.gold = 0;
            Manager.Sound.Play("Effect/Coin");
        }, Define.UIEvent.Click);
    }

    private void Start()
    {
        Init();
    }
}

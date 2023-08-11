using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPPPortion : RewardItem
{
    protected override void Init()
    {
        base.Init();
        //AddUIEvent(gameObject, (PointerEventData => { StartScene.go = true; }), Define.UIEvent.Click);
    }

    void Start()
    {
        Init();
    }
}

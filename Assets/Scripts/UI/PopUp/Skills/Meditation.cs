using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meditation : SkillIcon
{
    protected override void Init()
    {
        //Player player = Manager.Battle.nowTurnPlayer;
        //bool clicked = false;
        //AddUIEvent(gameObject, (PointerEventData) =>
        //{
        //    if (!clicked)
        //    {
        //        player.resourcenum += 1;
        //        clicked = true;
        //        UI_Panel.Dialog("기운을 1 회복했다!");
        //    }
        //}, Define.UIEvent.Click);
        base.Init();
    }

    void Start()
    {
        Init();
    }
}

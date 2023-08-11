using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownEvent_DamageHP : UnknownEvent
{
    public UnknownEvent_DamageHP()
    {
        description = "모든 아군의 체력 1감소...";
    }

    public override void Effect()
    {
        Manager.UI.SetDialog(description);
        foreach (var player in Manager.Data.playerstats)
        {
            player.currenthp = Mathf.Max(0, player.currenthp - 1);
            if (player.currenthp == 0)
                player.isdead = true;
        }
    }
}

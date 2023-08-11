using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownEvent_RecoverHP : UnknownEvent
{
    public UnknownEvent_RecoverHP()
    {
        description = "모든 아군의 체력 회복!";
    }

    public override void Effect()
    {
        Manager.UI.SetDialog(description);
        foreach (var player in Manager.Data.playerstats)
            player.currenthp = player.maxhp;
    }
}

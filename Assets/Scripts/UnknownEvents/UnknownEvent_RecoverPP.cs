using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownEvent_RecoverPP : UnknownEvent
{
    public UnknownEvent_RecoverPP()
    {
        description = "모든 아군의 PP 회복!";
    }

    public override void Effect()
    {
        Manager.UI.SetDialog(description);
        foreach (var player in Manager.Data.playerstats)
            foreach (var skill in player.skills.Values)
                skill.pp = skill.maxpp;
    }
}

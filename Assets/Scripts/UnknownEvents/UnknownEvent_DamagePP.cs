using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnknownEvent_DamagePP : UnknownEvent
{
    public UnknownEvent_DamagePP()
    {
        description = "모든 아군의 PP 3 감소...";
    }

    public override void Effect()
    {
        Manager.UI.SetDialog(description);
        foreach (var player in Manager.Data.playerstats)
            foreach (var skill in player.skills.Values)
                if (skill.required_level <= player.level) skill.pp = (int)MathF.Max(0, skill.pp - 3);
    }
}

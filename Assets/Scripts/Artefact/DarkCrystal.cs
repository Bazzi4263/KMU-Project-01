using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DarkCrystal
{
         
    public static void Effect(List<Stat> players, float[] rates, bool isReset)
    {
        foreach (Stat player in players)
        {
            foreach (var skill in player.skills.Values)
            {
                if (skill.type == Define.actionType.Attack || skill.type == Define.actionType.Combined)
                {
                    skill.count += isReset ? -1 : +1;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicainBook
{  
    public static void Effect(List<Stat> players, float[] rates, bool isReset)
    {
        foreach (Stat player in players)
        {
            foreach (var skill in player.skills.Values)
            {
                if (skill.type == Define.actionType.Buff)
                {
                    for (int i = 0; i < skill.duration.Length; i++)
                    {
                        skill.duration[i] = isReset ? (int)(skill.duration[i] / (1 + rates[0]))
                            : (int)(skill.duration[i] * (1 + rates[0]));
                    }
                }
            }            
        }
    }
}

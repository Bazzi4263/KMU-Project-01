using System.Collections.Generic;
using UnityEngine;

public class InfinityBuff : Basebuff
{
    Queue<bool> tempBool = new Queue<bool>();

    protected override void init()
    {
        base.init();

        foreach (Skill skill in affectedchara._stats.skills.Values)
        {
            tempBool.Enqueue(skill.requireResource);
            skill.requireResource = false;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (Skill skill in affectedchara._stats.skills.Values)
        {
            skill.requireResource = tempBool.Dequeue();
        }
    }
}

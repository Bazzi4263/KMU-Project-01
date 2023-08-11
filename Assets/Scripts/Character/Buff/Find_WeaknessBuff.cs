using UnityEngine;

public class Find_WeaknessBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        affectedchara._buffedstats.armor = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

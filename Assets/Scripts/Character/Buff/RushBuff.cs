using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RushBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        affectedchara._stats.currenthp -= Manager.Battle._data.rates[2];
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

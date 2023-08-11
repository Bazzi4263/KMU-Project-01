using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = (int)Manager.Battle._data.rates[1];
        affectedchara.turngauge = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

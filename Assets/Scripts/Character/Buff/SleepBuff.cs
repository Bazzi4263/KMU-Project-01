using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SleepBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = $"수면. 행동 불가";
        affectedStatus = characterStatus.Inactive;
        affectedchara._status[characterStatus.Inactive] = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._status[characterStatus.Inactive] = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StunBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = $"기절. 행동 불가";
        affectedStatus = characterStatus.Inactive;
        affectedchara._status[characterStatus.Inactive] = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._status[characterStatus.Inactive] = false;
    }
}

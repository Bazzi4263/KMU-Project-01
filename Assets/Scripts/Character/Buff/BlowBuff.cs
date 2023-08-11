using UnityEngine;
using static Define;

public class BlowBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        affectedStatus = Define.characterStatus.Inactive;
        caster._status[characterStatus.Inactive] = true;
        description = "행동 불가";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        caster._status[characterStatus.Inactive] = false;
    }
}

using UnityEngine;
using static Define;

public class ChronoBreakBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = $"{duration}턴간 행동 제한";
        affectedchara._status[characterStatus.Inactive] = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._status[characterStatus.Inactive] = false;
    }
}

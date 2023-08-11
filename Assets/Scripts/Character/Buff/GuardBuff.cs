using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = (int)(affectedchara._stats.armor * Manager.Battle._data.rates[0]);
        affectedchara._buffedstats.armor += amount;
        description = $"한 턴간 방어력 {amount} 상승";
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.armor -= amount;
        base.OnDestroy();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasteBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(caster._stats.speed * Manager.Battle._data.rates[0]);
        description = $"3턴동안 스피드 {amount} 증가";
        affectedchara._buffedstats.speed += amount;
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.speed -= amount;
        base.OnDestroy();
    }

}

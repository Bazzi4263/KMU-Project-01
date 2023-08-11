using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(caster._stats.attack_power * Manager.Battle._data.rates[0]);
        description = $"3턴동안 공격력 {amount} 증가";
        affectedchara._buffedstats.attack_power += amount;
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.attack_power -= amount;
        base.OnDestroy();
    }

}

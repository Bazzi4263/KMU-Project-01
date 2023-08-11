using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEnergyBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(affectedchara._stats.attack_power * Manager.Battle._data.rates[0]);
        description = $"공격력 {amount} 증가";
        affectedchara._buffedstats.attack_power += amount;
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.attack_power -= amount;
        base.OnDestroy();
    }

}

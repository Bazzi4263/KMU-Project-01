using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        bufftype = Define.buffType.Instant;
        amount = Mathf.RoundToInt(affectedchara._stats.currenthp + caster._buffedstats.attack_power * Manager.Battle._data.rates[0] > affectedchara._stats.maxhp ?
            affectedchara._stats.maxhp - affectedchara._stats.currenthp : caster._buffedstats.attack_power * Manager.Battle._data.rates[0]);
        affectedchara.Heal(amount);
        Manager.UI.SetDialog($"대상의 체력을 {amount}만큼 회복!");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

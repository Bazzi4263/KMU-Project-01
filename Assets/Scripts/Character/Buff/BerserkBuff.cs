using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BerserkBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(affectedchara._stats.maxhp - affectedchara._stats.currenthp);
        description = $"공격력 {amount}증가";
        affectedchara._buffedstats.attack_power += amount;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._buffedstats.attack_power -= amount;
    }


    public override void EffectOnEffectedTurnStart()
    {
        affectedchara._buffedstats.attack_power -= amount;
        amount = Mathf.RoundToInt(affectedchara._stats.maxhp - affectedchara._stats.currenthp);
        description = $"공격력 {amount}증가";
        affectedchara._buffedstats.attack_power += amount;
    }
}

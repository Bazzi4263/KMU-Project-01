using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BerserkIIBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt((affectedchara._stats.maxhp - affectedchara._stats.currenthp) / 2);
        description = $"방어력 {amount}증가";
        affectedchara._buffedstats.armor += amount;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._buffedstats.attack_power -= amount;
    }


    public override void EffectOnTurnStart()
    {
        affectedchara._buffedstats.armor -= amount;
        amount = Mathf.RoundToInt((affectedchara._stats.maxhp - affectedchara._stats.currenthp) / 2);
        description = $"방어력 {amount}증가";
        affectedchara._buffedstats.armor += amount;
    }
}

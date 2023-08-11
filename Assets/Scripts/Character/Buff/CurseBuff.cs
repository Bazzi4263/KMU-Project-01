using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseBuff : Basebuff //디버프여도 그냥 이름에 Buff를 붙임
{
    float amount;
    static bool isDownedHP;

    protected override void init()
    {
        base.init();
        amount = affectedchara._buffedstats.speed * Manager.Battle._data.rates[0];

        affectedchara._buffedstats.speed -= amount;

        description = $"속도 {amount} 하락";

        if (!isDownedHP)
        {
            isDownedHP = true;
            caster._stats.currenthp -= Manager.Battle._data.rates[1];
        }
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.speed += amount;
        base.OnDestroy();
    }
}

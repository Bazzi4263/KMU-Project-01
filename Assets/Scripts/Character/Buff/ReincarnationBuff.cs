using System;
using UnityEngine;

public class ReincarnationBuff : Basebuff
{
    Data data;

    protected override void init()
    {
        caster = Manager.Battle.nowTurnChar;
        affectedchara = gameObject.GetComponent<Character>();

        data = affectedchara._stats.skills["Reincarnation"];
        sprite = Manager.Item.GetSprite(data.spritenum);
        duration = leftdur = data.duration?[0] ?? 0;
        affectedchara.gui.SetBuffGUI(this);
        buffname = data.krname;
        bufftype = data.bufftype;
        effect = data.effect;
        particle = data.particle;

        amount = (int)(affectedchara._stats.attack_power * data.rates[0]);

        affectedchara._buffedstats.attack_power += amount;

        description = $"공격력, {amount}증가";

        affectedchara.turngauge = 9999;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._buffedstats.attack_power -= amount;
        if (duration == 1)
        {
            Manager.Battle.Player.Remove((Player)affectedchara);
            affectedchara.Death();
        }
        affectedchara.turngauge = 0;
    }
}

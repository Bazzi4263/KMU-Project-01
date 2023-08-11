using System;
using UnityEngine;

public class FranticBuff : Basebuff
{
    Data data;

    protected override void init()
    {
        stackable = true;

        caster = Manager.Battle.nowTurnChar;
        data = caster._stats.skills["Frantic"];
        affectedchara = gameObject.GetComponent<Character>();
        sprite = Manager.Item.GetSprite(data.spritenum);
        duration = leftdur = data.duration?[0] ?? 0;
        affectedchara.gui.SetBuffGUI(this);
        buffname = data.krname;
        description = data.description;
        bufftype = data.bufftype;
        effect = data.effect;
        particle = data.particle;

        maxstack = data.duration[0];

        leftdur = 1;
        amount = 0;
        UpdateStack(1);

        description = $"스피드 {amount}증가";
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        amount = (int)data.rates[0] * stack;
        affectedchara._buffedstats.speed -= amount;
    }

    public override void UpdateStack(int num)
    {
        amount = (int)data.rates[0] * stack;
        affectedchara._buffedstats.speed -= amount;
        base.UpdateStack(num);
        amount = (int)data.rates[0] * stack;
        affectedchara._buffedstats.speed += amount;

        description = $"스피드 {amount}증가";
        affectedchara.gui.UpdateBuffStack(this);
    }
}

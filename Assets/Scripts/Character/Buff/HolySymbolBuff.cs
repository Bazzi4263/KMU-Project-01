using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HolySymbolBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(caster._stats.attack_power * Manager.Battle._data.rates[0]);
        if (amount < 1) amount = 1;
        description = $"매 턴 {amount}만큼 체력 회복";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }


    public override void EffectOnEffectedTurnStart()
    {
        int am = (int)(affectedchara._stats.currenthp + amount > affectedchara._stats.maxhp ?
            affectedchara._stats.maxhp - affectedchara._stats.currenthp : amount);
        Debug.Log($"{amount} {am}");
        if (am != 0) {
            affectedchara.GenerateParticle(Define.ParticleTypes.Green);
            affectedchara.GenerateEffect("Heal");
            affectedchara.Heal(am);
        }
        
    }
}

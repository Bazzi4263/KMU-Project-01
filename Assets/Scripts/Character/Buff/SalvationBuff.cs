using UnityEngine;

public class SalvationBuff : Basebuff
{
    protected override void init()
    {
        bufftype = Define.buffType.Instant;
        base.init();
        amount = Mathf.RoundToInt(caster._buffedstats.attack_power * Manager.Battle._data.rates[0]);
        affectedchara.Heal(amount);
        caster.Heal(amount);
        Manager.UI.SetDialog($"체력을 {amount}만큼 회복!");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

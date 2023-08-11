using UnityEngine;

public class RestBuff : Basebuff
{
    protected override void init()
    {
        bufftype = Define.buffType.Instant;
        base.init();
        amount = Mathf.RoundToInt(affectedchara._stats.currenthp + affectedchara._buffedstats.armor * Manager.Battle._data.rates[0] > affectedchara._stats.maxhp ? 
            affectedchara._stats.maxhp - affectedchara._stats.currenthp : affectedchara._buffedstats.armor * Manager.Battle._data.rates[0]);
        affectedchara.Heal(amount);
        Manager.UI.SetDialog($"체력을 {amount}만큼 회복!");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

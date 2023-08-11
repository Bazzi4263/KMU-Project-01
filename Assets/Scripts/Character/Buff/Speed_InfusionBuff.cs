using UnityEngine;

public class Speed_InfusionBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(affectedchara._stats.speed * Manager.Battle._data.rates[0]);
        affectedchara._buffedstats.speed += amount;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._buffedstats.speed -= amount;
    }

}

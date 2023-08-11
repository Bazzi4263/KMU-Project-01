using UnityEngine;

public class SpeedEnergyBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = (int)Manager.Battle._data.rates[0];
        affectedchara._buffedstats.speed += amount;
        description = $"속도 {amount} 증가.";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._buffedstats.speed -= amount;
    }

}

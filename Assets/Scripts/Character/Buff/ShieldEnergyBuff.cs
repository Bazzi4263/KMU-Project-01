using TMPro;
using UnityEngine;

public class ShieldEnergyBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = "적의 공격을 막아내는 방어막";
        amount = (int)Manager.Battle._data.rates[0];
        affectedchara.shield = amount;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

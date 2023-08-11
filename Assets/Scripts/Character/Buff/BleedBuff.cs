using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = (int)Manager.Battle._data.rates[0];
        description = $"매 턴마다 {amount}만큼의 피해";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void EffectOnEffectedTurnStart()
    {
        StartCoroutine(affectedchara.getDamaged(amount, true, 1, true, particle: Define.ParticleTypes.Blood));
    }
}

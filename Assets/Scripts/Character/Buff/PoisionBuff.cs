using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionBuff : Basebuff
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

    public override void EffectOnEffectedTurnEnd()
    {
        StartCoroutine(affectedchara.getDamaged(amount, true, 1, true, "Debuff_2", Define.ParticleTypes.Purple));
    }
}

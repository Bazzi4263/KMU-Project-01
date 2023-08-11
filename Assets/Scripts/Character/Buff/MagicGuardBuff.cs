using TMPro;
using UnityEngine;

public class MagicGuardBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = "적의 공격을 막아내는 방어막";
        amount = Mathf.RoundToInt(caster._stats.attack_power * Manager.Battle._data.rates[0]);
        affectedchara.shield = Mathf.Max(amount, affectedchara.shield);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

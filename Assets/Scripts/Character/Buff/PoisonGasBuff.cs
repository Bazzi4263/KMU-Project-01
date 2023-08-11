using UnityEngine;

public class PoisonGasBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount = Mathf.RoundToInt(caster._buffedstats.attack_power * Manager.Battle._data.rates[0]);
        description = $"매 턴마다 {amount}만큼의 피해";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void EffectOnTurnStart()
    {
        CoroutineHelper.StartCoroutine((affectedchara.getDamaged(amount, true, 1, true, "Debuff_2", Define.ParticleTypes.Purple)));
        BuffManager.Instance.UpdateBuffStatus(this);
    }
}

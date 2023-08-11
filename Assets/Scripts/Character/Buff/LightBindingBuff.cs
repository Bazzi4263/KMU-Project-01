using UnityEngine;
using static Define;

public class LightBindingBuff : Basebuff
{
    float probability;

    protected override void init()
    {
        base.init();
        probability = Manager.Battle._data.rates[0];
        description = $"{duration}턴간 행동 제한";
        affectedchara._status[characterStatus.Inactive] = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._status[characterStatus.Inactive] = false;
    }

    public override void EffectOnEffectedTurnEnd()
    {
        if (Random.Range(0f, 1f) < probability)
        {
            affectedchara.GenerateEffect("Buff_7");
            leftdur += 1;
            description = $"{leftdur}턴간 행동 제한";
            Manager.UI.SetDialog($"LightBinding 지속시간 1턴 증가!", 1f);
        }
    }
}

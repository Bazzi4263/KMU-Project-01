using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GenesisBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = $"강력한 스킬을 사용하기 위해 힘을 모으는 중. 행동 불가";
        if (duration == 0)
            leftdur += 1;
        StartCoroutine(InactiveCoroutine());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void EffectOnEffectedTurnStart()
    {
        if (duration == 0)
        {
            leftdur = 0;
        }

        if (leftdur < 1) 
        {
            affectedchara.GenerateEffect("Buff_8");
            affectedchara._status[characterStatus.Inactive] = false;
            description = $"Genesis 사용 준비 완료!";        
        }
    }

    public override void EffectOnEffectedTurnEnd()
    {
        if (duration == 0)
        {
            StopAllCoroutines();
            affectedchara._status[characterStatus.Inactive] = false;
            description = $"Genesis 사용 준비 완료!";
            affectedchara.turngauge = 100;
            affectedchara.GenerateEffect("Buff_8");
        }
    }

    IEnumerator InactiveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        affectedStatus = characterStatus.Inactive;
        caster._status[characterStatus.Inactive] = true;
    }
}

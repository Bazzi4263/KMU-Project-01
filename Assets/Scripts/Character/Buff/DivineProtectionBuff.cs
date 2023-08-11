using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DivineProtectionBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        affectedStatus = Define.characterStatus.DebuffImmune;
        description = "모든 디버프에 면역";
        affectedchara._status[Define.characterStatus.DebuffImmune] = true;
        for(int i = affectedchara.bufflist.Count-1; i>=0; i--)
        {
            if (affectedchara.bufflist[i].bufftype == Define.buffType.Debuff)
            {
                BuffManager.Instance.DestroyBuff(affectedchara, affectedchara.bufflist[i]);
            }
        }
    }

    protected override void OnDestroy()
    {
        affectedchara._status[Define.characterStatus.DebuffImmune] = false;
        base.OnDestroy();
    }

    public override void ApplyBuffEffect() // awake(init)이후에 적용되야 할 효과
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class SmokeBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        affectedStatus = Define.characterStatus.Cloak;
        affectedchara._status[Define.characterStatus.Cloak] = true;
        affectedchara.GetComponent<SpriteRenderer>().DOFade(0.2f, 0.5f);
        ShadowPartnerBuff shadowbuff = (ShadowPartnerBuff)affectedchara.bufflist.Find(x => x.GetType() == typeof(ShadowPartnerBuff));
        if(shadowbuff != null) shadowbuff.thiefShadowObj.GetComponent<SpriteRenderer>().DOFade(0.2f, 0.5f);
        amount = Mathf.RoundToInt(caster._stats.attack_power * Manager.Battle._data.rates[0]);
    }

    protected override void OnDestroy()
    {
        affectedchara._status[Define.characterStatus.Cloak] = false;
        affectedchara.GetComponent<SpriteRenderer>().DOFade(1f, 0.1f);
        ShadowPartnerBuff shadowbuff = (ShadowPartnerBuff)affectedchara.bufflist.Find(x => x.GetType() == typeof(ShadowPartnerBuff));
        if (shadowbuff != null) shadowbuff.thiefShadowObj.GetComponent<SpriteRenderer>().DOFade(1f, 0.1f);
        base.OnDestroy();
    }

}

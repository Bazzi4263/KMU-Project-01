using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPartnerBuff : Basebuff
{
    public GameObject thiefShadowObj;

    protected override void init()
    {
        base.init();
        amount = (int)Manager.Battle._data.rates[0];
        description = $"{leftdur}턴동안 타수 {amount}배 증가";
        foreach (var skill in affectedchara._stats.skills.Values)
        {
            if (skill.type == Define.actionType.Attack)
            {
                skill.count *= amount;
            }
        }
        thiefShadowObj = Manager.Resource.Instantiate("battlescene/ThiefShadow", Vector3.zero, affectedchara.transform);
        thiefShadowObj.transform.localPosition = new Vector3(-0.3f, 0, 0);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var skill in affectedchara._stats.skills.Values)
        {
            if (skill.type == Define.actionType.Attack)
            {
                skill.count /= amount;
            }
        }
        Destroy(thiefShadowObj);
    }

}

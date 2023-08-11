using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Advanced_StrikeBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        description = $"기절. 행동 불가";
        affectedStatus = characterStatus.Inactive;
        affectedchara._status[characterStatus.Inactive] = true;
    }

    private void Start()
    {
        if (Random.Range(0f, 1f) < Manager.Battle._data.rates[1])
        {
            BuffManager.Instance.DestroyBuff(affectedchara, this);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        affectedchara._status[characterStatus.Inactive] = false;
    }
}

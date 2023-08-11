using System;
using UnityEngine;

public class MeditationBuff : Basebuff
{
    float changeddmg;
    protected override void init()
    {
        stackable = true;
        maxstack = 99;
        base.init();
        leftdur = 99;
        description = "강력한 스킬을 사용하기 위한 기운.";
        UpdateStack((int)(Manager.Battle._data.rates[0]));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void UpdateStack(int num)
    {
        base.UpdateStack(num);
        
        affectedchara.resourcenum += num;
        affectedchara.gui.UpdateBuffStack(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genius : Monster
{
    protected override void init()
    {
        base.init();
        Manager.Sound.Play("Effect/BossImpact");
    }
}
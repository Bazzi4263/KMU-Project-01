using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : Monster
{
    protected override void init()
    {
        base.init();
        Manager.Sound.Play("Effect/BossImpact");
    }
}
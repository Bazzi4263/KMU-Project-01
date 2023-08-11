using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleShot : SkillIcon
{
    protected override void Init()
    {
        base.Init();
        image.color = new Color(110 / 255f, 1.0f, 1.0f);
    }

    void Start()
    {
        Init();
    }
}

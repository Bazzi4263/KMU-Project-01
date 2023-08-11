using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardExp : RewardIcon
{
    protected override void Init()
    {
        base.Init();
        _destext.text = $"{Manager.Battle.exp} Exp";
        _image.sprite = Manager.Item.GetSprite(911);
    }

    private void Start()
    {
        Init();
    }
}

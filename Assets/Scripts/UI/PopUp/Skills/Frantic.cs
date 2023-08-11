using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Frantic : SkillIcon
{
    protected override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        image = Get<GameObject>((int)GameObjects.ItemImage).GetComponent<Image>();
        destext = Get<GameObject>((int)GameObjects.ItemDesText).GetComponent<TextMeshProUGUI>();
        numtext = Get<GameObject>((int)GameObjects.ItemNumText).GetComponent<Text>();

        Player player = Manager.Battle.nowTurnPlayer;

        destext.text = player._stats.skills[this.GetType().Name].description;
        image.sprite = Manager.Item.GetSprite(player._stats.skills[this.GetType().Name].spritenum);
        numtext.text = $"{player._stats.skills[this.GetType().Name].pp} / {player._stats.skills[this.GetType().Name].maxpp}";
    }

    void Start()
    {
        Init();
    }
}

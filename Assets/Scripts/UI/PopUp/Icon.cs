using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * Item과 Skill의 개수, 설명, 아이콘을 띄워주기 위해 각각을 변수로 받아놓는다.
 */
public class Icon : UI_Base
{
    public TextMeshProUGUI destext;
    public Text numtext;
    public Image image;
    public Data data = new Data();
    protected enum GameObjects
    {
        ItemImage,
        ItemDesText,
        ItemNumText
    }

    protected override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        image = Get<GameObject>((int)GameObjects.ItemImage).GetComponent<Image>();
        destext = Get<GameObject>((int)GameObjects.ItemDesText).GetComponent<TextMeshProUGUI>();
        numtext = Get<GameObject>((int)GameObjects.ItemNumText).GetComponent<Text>();
    }

    private void Awake()
    {
        Init();
    }
}

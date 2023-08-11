using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardIcon : UI_Base
{
    protected Text _destext;
    protected Image _image;

    enum GameObjects
    {
        ItemIcon,
        ItemDesText
    }

    protected override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _image = Get<GameObject>((int)GameObjects.ItemIcon).GetComponent<Image>();
        _destext = Get<GameObject>((int)GameObjects.ItemDesText).GetComponent<Text>();
        AddUIEvent(gameObject, (PointerEventData) => { gameObject.SetActive(false); }, Define.UIEvent.Click);
    }
}

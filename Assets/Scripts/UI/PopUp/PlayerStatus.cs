using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerStatus : UI_Base
{
    Text _text;
    Image _image;
    Image _gameobjectimage;

    enum GameObjects
    {
        PlayerIcon,
        PlayerText,
    }

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _image = Get<GameObject>((int)GameObjects.PlayerIcon).GetComponent<Image>();
        _text = Get<GameObject>((int)GameObjects.PlayerText).GetComponent<Text>();
        _gameobjectimage = Util.GetOrAddComponent<Image>(gameObject);
        Color imagecolor = _image.color;
        Color gameobjectcolor = _gameobjectimage.color;
        Color textcolor = _text.color;

        AddUIEvent(gameObject, (PointerEventData data) =>
        {
            gameObject.transform.position = data.position;
            SetColor(true);
        }
        , Define.UIEvent.Drag);

        AddUIEvent(gameObject, (PointerEventData) =>
        {
            SetColor(false);
        }
        , Define.UIEvent.EndDrag);
    }

    private void SetColor(bool IsDraging)
    {
        Color imagecolor = _image.color;
        Color gameobjectcolor = _gameobjectimage.color;
        Color textcolor = _text.color;

        if (IsDraging)
        {
            imagecolor.a = 0.5f;
            gameobjectcolor.a = 0.5f;
            textcolor.a = 0.5f;
            _image.color = imagecolor;
            _gameobjectimage.color = gameobjectcolor;
            _text.color = textcolor;
        }

        else
        {
            imagecolor.a = 1f;
            gameobjectcolor.a = 1f;
            textcolor.a = 1f;
            _image.color = imagecolor;
            _gameobjectimage.color = gameobjectcolor;
            _text.color = textcolor;
        }
    }

    public void SetPanel(Stat stat)
    {
        _image.sprite = stat.sprites[1];
        _text.text = $"\t\t\t{stat.name}\n\tATK: {stat.attack_power}\n\tDEF: {stat.armor}" +
            $"\n\tHP: {stat.currenthp}/{stat.maxhp}\n\tSPD: {stat.speed}";

        AddUIEvent(_image.gameObject, (PointerEventData data) =>
        {
            if (data.button ==  PointerEventData.InputButton.Left)
            {
                Manager.UI.Issetting = true;
                Manager.Data.nowplayer = stat;
                UI_Skill skill = Manager.UI.ShowPopUpUI<UI_Skill>();
            }
        }
        , Define.UIEvent.Click);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * BattleScene, MapScene에서 사용할 UI_SettingBar, 시간과 미니맵을 보고 세팅을 할 수 있다
 */
public class UI_SettingBar : UI_Scene
{
    Text _time;
    Text _gold;
    int _h, _m;
    string _fmt;

    void Start()
    {
        Init();
        _fmt = "00";
    }

    private void Update()
    {
        _gold.text = Manager.Item.gold.ToString();
        _m = Manager.Data.time % 60;
        _h = (Manager.Data.time - _m) / 60;
        _time.text = $"{_h.ToString(_fmt)}:{_m.ToString(_fmt)}";
    }

    protected override void Init()
    {
        base.Init();

        _time = GetText((int)Define.Texts.TimeText);
        _gold = GetText((int)Define.Texts.GoldText);
        Button minimap = GetButton((int)Define.Buttons.MinimapButton);
        
        Button setting = GetButton((int)Define.Buttons.SettingButton);
        Button bag = GetButton((int)Define.Buttons.BagButton);
        Button character = GetButton((int)Define.Buttons.CharButton);

        AddUIEvent(minimap.gameObject, (PointerEventData => { Manager.UI.ShowPopUpUI <Minimap>(); }), Define.UIEvent.Click);
        AddUIEvent(setting.gameObject, (PointerEventData => 
        { 
            Manager.UI.ShowPopUpUI<Setting>();
        }), Define.UIEvent.Click);

        AddUIEvent(bag.gameObject, (PointerEventData) => 
        {
            Manager.UI.ShowPopUpUI<UI_Inven>();
            Manager.UI.Issetting = true;
        }, Define.UIEvent.Click);

        AddUIEvent(character.gameObject, (PointerEventData data) => 
        {
            Manager.UI.ShowPopUpUI<CurrentPlayerList>();
        }, Define.UIEvent.Click);

    }
}

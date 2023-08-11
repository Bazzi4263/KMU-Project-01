using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*
 * BattleScene에서 UI_Scene(웬만하면 없어지지 않는) 산하의 오브젝트들을 다룸
 * 스킬버튼, 가방버튼, 도망버튼, 취소버튼, 전투상황 텍스트가 있으며 Dialog 함수에 string으로 전투상황 텍스트를 적으면 텍스트가 나온다
 * 플레이어의 턴이 아닐 경우 스킬, 가방, 도망은 보일 필요가 없고, 타겟팅 중이 아니면 취소버튼이 필요 없으니 State 변경 시 SetButton을 실행시켜주자
*/
public class UI_Panel : UI_Scene
{
    private Button _skill;
    private Button _bag;
    private Button _run;
    private Button _cancel;
    private Text _dialog;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
 
        _skill = GetButton((int)Define.Buttons.SkillButton);
        _bag = GetButton((int)Define.Buttons.BagButton);
        _run = GetButton((int)Define.Buttons.RunButton);
        _cancel = GetButton((int)Define.Buttons.CancelButton);

        _dialog = GetText((int)Define.Texts.DialogText).GetComponent<Text>();
        _dialog.color = new Color(50f/255f, 50f/255f, 50f/255f);

        AddUIEvent(_bag.gameObject, (PointerEventData) => { Manager.UI.Issetting = false; }, Define.UIEvent.Click);
        AddUIEvent(_bag.gameObject, (PointerEventData) => { Manager.UI.ShowPopUpUI<UI_Inven>(); }, Define.UIEvent.Click);

        AddUIEvent(_skill.gameObject, (PointerEventData) => { Manager.UI.Issetting = false; }, Define.UIEvent.Click);
        AddUIEvent(_skill.gameObject, (PointerEventData) => { Manager.UI.ShowPopUpUI<UI_Skill>(); }, Define.UIEvent.Click);

        AddUIEvent(_run.gameObject, (PointerEventData) => { Manager.Battle.Run(); }, Define.UIEvent.Click);
        AddUIEvent(_cancel.gameObject, (PointerEventData) => 
        {
            if (Manager.Battle.nowTurnPlayer != Manager.Battle.nowTurnChar) Manager.Battle.nowTurnPlayer = (Player)Manager.Battle.nowTurnChar;
            if(Manager.Battle.state == Define.BattleState.SELECTINGDEADTARGET) Manager.Battle.SetCharAlpha(null, 1f, true);
            else Manager.Battle.SetCharAlpha(null, 1f);
            Manager.Battle.changeTurn();
        }, Define.UIEvent.Click);

        _skill.gameObject.SetActive(false);
        _bag.gameObject.SetActive(false);
        _run.gameObject.SetActive(false);
        _cancel.gameObject.SetActive(false);

        Manager.UI.Panel = this;
    }

    public void Dialog(string dialog)
    {
        CoroutineHelper.StartCoroutine(CoroutineHelper.Typing(_dialog, dialog, 0.01f/* * Time.deltaTime*/));
    }

    public void SetButton(Define.BattleState state)
    {
        if(state == Define.BattleState.SELECTINGTARGET || state == Define.BattleState.SELECTINGDEADTARGET)
            _cancel.gameObject.SetActive(true);

        else
            _cancel.gameObject.SetActive(false);
        


        if (state == Define.BattleState.PLAYERTURN)
        {
            _skill.gameObject.SetActive(true);
            _bag.gameObject.SetActive(true);
            _run.gameObject.SetActive(true);
        }

        else
        {
            _skill.gameObject.SetActive(false);
            _bag.gameObject.SetActive(false);
            _run.gameObject.SetActive(false);
        }
    }
}

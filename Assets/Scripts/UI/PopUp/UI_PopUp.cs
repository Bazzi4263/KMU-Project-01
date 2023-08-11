using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*
 * 모든 팝업창들에게 붙여짐
 * 팝업창 이외의 곳을 클릭하면 팝업창이 닫히게 해준다
 * 캔슬버튼도 만들어서 누를 시 닫히게 만들어준다
 */
public class UI_PopUp : UI_Base
{
    protected Image _out;
    protected Button _cancel;

    protected override void Init()
    {
        Manager.UI.SetCanvas(gameObject, true);
        Bind<Image>(typeof(Define.Images));
        Bind<Button>(typeof(Define.Buttons));

        _out = GetImage((int)Define.Images.OutIcon);
        _cancel = GetButton((int)Define.Buttons.CancelButton);
        if (_cancel != null)
            AddUIEvent(_cancel.gameObject, (PointerEventData) =>
            {
                if (Manager.Battle.state == Define.BattleState.SELECTINGPPTARGET)
                {
                    Manager.Battle.changeTurn();
                    Manager.Battle.SetCharAlpha(null, 1f);
                }
                Manager.UI.ClosePopUpUI();
            }, Define.UIEvent.Click);

        if (_out != null)
            AddUIEvent(_out.gameObject, (PointerEventData) => { Manager.UI.ClosePopUpUI(); }, Define.UIEvent.Click);
    }
}

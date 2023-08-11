using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapDialog : UI_PopUp
{
    Text _dialog;

    enum GameObjects
    {
        DialogText,
    }

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject panel = Get<GameObject>((int)GameObjects.DialogText);
        _dialog = Util.FindChild<Text>(panel);
        Manager.UI.MapDialog = this;
    }

    public void Dialog(string dialog)
    {
        Coroutine evt = CoroutineHelper.StartCoroutine(CoroutineHelper.Typing(_dialog, dialog, 0.01f/* * Time.deltaTime*/));
        AddUIEvent(this.gameObject, (PointerEventData) =>
        {
            CoroutineHelper.StopCoroutine(evt);
            _dialog.text = dialog;
        });
    }
}

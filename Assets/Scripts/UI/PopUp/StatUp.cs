using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUp : UI_PopUp
{
    enum GameObjects
    {
        StatPanel
    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject stat = Get<GameObject>((int)GameObjects.StatPanel);
        Image hp = Util.FindChild<Image>(stat, "Hp");
        Image speed = Util.FindChild<Image>(stat, "Speed");
        Image damage = Util.FindChild<Image>(stat, "Damage");

        AddUIEvent(_cancel.gameObject, (PointerEventData) =>
        {
            Manager.Reward.choosestat = true;
        }, Define.UIEvent.Click);

        AddUIEvent(hp.gameObject, (PointerEventData) =>
        {
            Manager.Reward.nowstat.maxhp++;
            Manager.Reward.nowstat.currenthp++;
            Manager.Reward.choosestat = true;
            Manager.UI.SetDialog($"{Manager.Reward.nowstat.name}의 최대 체력 1 증가!");
            Manager.UI.ClosePopUpUI();
        }, Define.UIEvent.Click);

        AddUIEvent(speed.gameObject, (PointerEventData) =>
        {
            Manager.Reward.nowstat.speed += 1.5f;
            Manager.Reward.choosestat = true;
            Manager.UI.SetDialog($"{Manager.Reward.nowstat.name}의 스피드 1.5 증가!");
            Manager.UI.ClosePopUpUI();
        }, Define.UIEvent.Click);

        AddUIEvent(damage.gameObject, (PointerEventData) =>
        {
            Manager.Reward.nowstat.attack_power++;
            Manager.Reward.choosestat = true;
            Manager.UI.SetDialog($"{Manager.Reward.nowstat.name}의 공격력 1 증가!");
            Manager.UI.ClosePopUpUI();
        }, Define.UIEvent.Click);
    }
}

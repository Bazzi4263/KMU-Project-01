using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class SkillIcon : Icon
{
    Stat stat = new Stat();
    protected override void Init()
    {
        if (!Manager.UI.Issetting)
        {
            Player player = Manager.Battle.nowTurnPlayer;
            AddUIEvent(gameObject, (PointerEventData) =>
            {
                if (Manager.Battle.state == Define.BattleState.SELECTINGPPTARGET)
                {
                    PPRecover();
                }
                else if (player._status[Define.characterStatus.Inactive]) Manager.Battle.changeTurn();
            //else if(player._stats.currenthp == player._stats.maxhp && player._stats.skills[this.GetType().Name].type == Define.actionType.Recover && player._stats.skills[this.GetType().Name].target == Define.Target.Self)
            //{   
            //    Manager.UI.SetDialog($"이미 최대 체력입니다!");
            //}
            else
                {
                    int skilltype = CheckRequirement();
                    if (skilltype > 0) SetSkill(skilltype);
                    else if (skilltype == 0) Manager.Battle.changeTurn();
                }

                if (Manager.Battle.state != Define.BattleState.SELECTINGPPTARGET) Manager.UI.ClosePopUpUI();
            }, Define.UIEvent.Click);
            base.Init();
            destext.text = player._stats.skills[this.GetType().Name].description;
            image.sprite = Manager.Item.GetSprite(player._stats.skills[this.GetType().Name].spritenum);
            numtext.text = $"{player._stats.skills[this.GetType().Name].pp} / {player._stats.skills[this.GetType().Name].maxpp}";
        }

        else
        {
            base.Init();
            stat = Manager.Data.nowplayer;
            destext.text = stat.skills[this.GetType().Name].description;
            image.sprite = Manager.Item.GetSprite(stat.skills[this.GetType().Name].spritenum);
            numtext.text = $"{stat.skills[this.GetType().Name].pp} / {stat.skills[this.GetType().Name].maxpp}";
        }
    }

    protected void PPRecover()
    {
        Player player = Manager.Battle.nowTurnPlayer;
        switch (Manager.Battle._itemdata.itemtype)
        {
            case Define.itemType.Recover:
                if (player._stats.skills[this.GetType().Name].pp == player._stats.skills[this.GetType().Name].maxpp)
                {
                    Manager.UI.SetDialog($"해당 스킬은 pp가 이미 최대치입니다!");
                }
                else if(player._stats.skills[this.GetType().Name].isSpecial)
                {
                    Manager.UI.SetDialog($"특수 스킬에는 사용할 수 없습니다!");
                }
                else
                {
                    Manager.Battle._itemdata.pp--;
                    int recoveramount = player._stats.skills[this.GetType().Name].pp + (int)Manager.Battle._itemdata.rates[0]
                    > player._stats.skills[this.GetType().Name].maxpp ?
                    player._stats.skills[this.GetType().Name].maxpp - player._stats.skills[this.GetType().Name].pp
                    : (int)Manager.Battle._itemdata.rates[0];
                    player._stats.skills[this.GetType().Name].pp += recoveramount;
                    Manager.UI.SetDialog($"{player._stats.skills[this.GetType().Name].name}의 pp를 {recoveramount} 회복!");
                    Manager.Battle.nowTurnChar.onTurnEnd();
                    Manager.Battle.SetCharAlpha(null, 1f);
                    CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
                }
                break;
            case Define.itemType.Permanant:
                if (player._stats.skills[this.GetType().Name].isSpecial)
                {
                    Manager.UI.SetDialog($"특수 스킬에는 사용할 수 없습니다!");
                }
                else
                {
                    Manager.Battle._itemdata.pp--;
                    player._stats.skills[this.GetType().Name].maxpp += (int)Manager.Battle._itemdata.rates[0];
                    Manager.UI.SetDialog($"{player._stats.skills[this.GetType().Name].name}의 pp 최대치를 {(int)Manager.Battle._itemdata.rates[0]} 증가!");
                    Manager.Battle.nowTurnChar.onTurnEnd();
                    Manager.Battle.SetCharAlpha(null, 1f);
                    CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
                }
                break;
        }
    }

    protected virtual int CheckRequirement() // return 0: 스킬 사용 불가 / 1~: 스킬타입, 기본 1
    {
        Player player = Manager.Battle.nowTurnPlayer;
        if (player._stats.skills[this.GetType().Name].requireResource && player.resourcenum < player._stats.skills[this.GetType().Name].required_resourcenum)
        {
            Manager.UI.SetDialog($"해당 스킬을 사용하기 위한 자원이 부족합니다!");
            return 0;
        }
        if (player._stats.skills[this.GetType().Name].pp <= 0)
        {
            Manager.UI.SetDialog("pp가 부족하여 스킬을 사용할 수 없습니다!");
            return 0;
        }
        return 1;
    }
    protected virtual void SetSkill(int skilltype = 1)
    {
        Manager.Battle._data = Manager.Battle.nowTurnPlayer._stats.skills[this.GetType().Name];
        Manager.Battle.OnTarget();
    }

}

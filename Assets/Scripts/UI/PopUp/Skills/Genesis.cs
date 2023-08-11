using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Genesis : SkillIcon
{
    protected override void Init()
    {
        Player player = Manager.Battle.nowTurnPlayer;
        base.Init();
        List<GenesisBuff> b = player.bufflist.OfType<GenesisBuff>().ToList();
        if (b.Count == 1 && b[0].leftdur <= 0)
            destext.text = $"<color=red>(준비됨)</color> 모든 적에게 AP * 500%의 피해";
        else
            destext.text = player._stats.skills[this.GetType().Name].description;
    }

    void Start()
    {
        Init();
    }

    protected override int CheckRequirement()
    {
        Player player = Manager.Battle.nowTurnPlayer;
        List<GenesisBuff> b = player.bufflist.OfType<GenesisBuff>().ToList();
        if (b.Count == 1 && b[0].leftdur <= 0)
        {
            BuffManager.Instance.DestroyBuff(player, b[0]);
            return 2; //2 : 적 전체 공격
        }
        if (player._stats.skills[this.GetType().Name].pp == 0)
        {
            return 0;
        }
        return 1;
        
    }
    protected override void SetSkill(int skilltype = 1)
    {   
        Data data = Manager.Battle.nowTurnPlayer._stats.skills[this.GetType().Name];
        //if (skilltype == 1) Manager.Battle.nowTurnPlayer._stats.skills[this.GetType().Name].pp++;
        if (skilltype == 2)
        {
            Manager.Battle.nowTurnPlayer._stats.skills[this.GetType().Name].pp++;
            data.type = Define.actionType.Attack;
            data.target = Define.Target.AllEnemy;
        }
        Manager.Battle._data = data;
        Manager.Battle.OnTarget();
        data.type = Define.actionType.Buff;
        data.target = Define.Target.Self;
    }
}

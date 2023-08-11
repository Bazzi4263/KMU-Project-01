using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Define;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    private void Awake()
    {
        BuffManager.Instance = this;
    }
    public IEnumerator SetBuff(Data data, List<Character> target, Character caster, bool isChangeTurn = true)
    {
        List<Character> buffedTarget = new List<Character>();

        switch (data.bufftarget)
        {
            case buffTarget.Caster:
                buffedTarget.Add(caster);
                break;
            case buffTarget.Target:
                buffedTarget = target;
                break;
            case buffTarget.AllPlayer:
                break;
            case buffTarget.AllEnemy:
                break;
            default:
                break;
        }

        caster.SetSkill(data.name);
        Manager.Battle.state = Define.BattleState.IDLE;

        if (data.requireResource) // 특수 자원 필요 시 감소시키기
        {
            DecreaseBuffStack(data.required_resourcename, caster, -data.required_resourcenum);
        }

        if (data.type != Define.actionType.Recover)
        {
            switch (data.target)
            {
                case Define.Target.Self:
                    Manager.UI.SetDialog($"{caster._stats.name}이(가) 자신에게 {data.name}을(를) 적용!");
                    break;
                case Define.Target.AllEnemy:
                    Manager.UI.SetDialog($"{caster._stats.name}이(가) 적 전체에게 {data.name}을(를) 적용!");
                    break;
                case Define.Target.AllPlayer:
                    Manager.UI.SetDialog($"{caster._stats.name}이(가) 아군 전체에게 {data.name}을(를) 적용!");
                    break;
                default:
                    if (data.bufftarget != Define.buffTarget.Caster)
                        Manager.UI.SetDialog($"{caster._stats.name}이(가) {buffedTarget[0]._stats.name}에게 {data.name}을(를) 적용!");
                    break;
            }
        }
        else if (buffedTarget.Count == 1 && buffedTarget[0]._stats.maxhp == buffedTarget[0]._stats.currenthp) //대상이 1명인 회복 스킬이며 대상이 최대 체력일 시
        {
            data.pp += 1;
            Manager.UI.SetDialog($"이미 최대 체력입니다!");
            yield return Manager.Data.wfs10;
            Manager.Battle.changeTurn();
            Manager.Battle.SetCharAlpha(buffedTarget, 1f);
            yield break;
        }

        Manager.Battle.SetCharAlpha(buffedTarget);

        foreach (Character t in buffedTarget) //버프 추가
        {
            if (t._status[characterStatus.DebuffImmune] && data.type == actionType.Debuff)//디버프 면역 상태
            {
                t.CreateDmgText(0, "immune");
                continue;
            }
            else if (t._status[characterStatus.Dodge])
            {
                continue;
            }
            AddBuff($"{data.name}Buff", t);
        }
        
        foreach (Character t in buffedTarget) //즉시적용 버프 바로 삭제. 즉시 회복, 디버프 해제 등.
        {
            for (int i = t.bufflist.Count - 1; i >= 0; i--)
            {
                if (t.bufflist[i].bufftype == buffType.Instant)
                {
                    DestroyBuff(t, t.bufflist[i]);
                }
            }
        }
        if (isChangeTurn && data.attackType != attackType.melee)
        {
            caster.onTurnEnd(data, buffedTarget);
            yield return Manager.Data.wfs15;

            Manager.Battle.SetCharAlpha(buffedTarget, 1f);
        }
        if (isChangeTurn)
            CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
    }

    public void AddBuff(string buffname, Character target) //버프 추가
    {
        Type b = Type.GetType(buffname);
        //Basebuff testbuff = (Basebuff)Activator.CreateInstance(Type.GetType(buffname));
        Basebuff addingBuff = (Basebuff)target.gameObject.GetComponent(b);
        if (addingBuff != null)
        { // 이미 버프 있을 시
            Debug.Log(addingBuff.effect);
            target.GenerateParticle(addingBuff.particle);
            target.GenerateEffect(addingBuff.effect);
            if (!target._status[characterStatus.Cloak])
            {
                switch (addingBuff.bufftype)
                {
                    case buffType.Debuff:
                        target.SpriteBlink("bad");
                        break;
                    case buffType.Buff:
                        target.SpriteBlink("good");
                        break;
                    case buffType.Bind:
                        target.SpriteBlink("bad");
                        break;
                    default:
                        target.SpriteBlink("good");
                        break;
                }
            }
            if (addingBuff.stackable)
            {
                addingBuff.UpdateStack((int)(Manager.Battle._data.rates[0])); // 스택 가능한 버프일 시 스택 추가
            }
            if (addingBuff.bufftype == Define.buffType.Shield) // 쉴드 스킬일 시 쉴드 중첩
            {
                target.shield += addingBuff.amount;
            }

            if (addingBuff.bufftype != buffType.Permanant)
            {
                addingBuff.leftdur = addingBuff.duration; // 남은 지속시간 갱신
            }
            
        }
        else // 버프 없을 시 추가
        {
            addingBuff = (Basebuff)target.gameObject.AddComponent(b);
            Debug.Log(addingBuff.effect);
            addingBuff.ApplyBuffEffect();
            target.bufflist.Add(addingBuff);
            if (!target._status[characterStatus.Cloak])
            {
                switch (addingBuff.bufftype)
                {
                    case buffType.Debuff:
                        target.SpriteBlink("bad");
                        break;
                    case buffType.Buff:
                        target.SpriteBlink("good");
                        break;
                    case buffType.Bind:
                        target.SpriteBlink("bad");
                        break;
                    default:
                        target.SpriteBlink("good");
                        break;
                }
            }
        }
    }
    public void UpdateBuffStatus(Character chara, Type buff = null) // 버프 지속턴 감소시키고 0일 시 파괴
    {
        for (int i = chara.bufflist.Count - 1; i >= 0; i--)
        {
            if (buff != null && buff == chara.bufflist[i].GetType()) continue;
            if (chara.bufflist[i].bufftype != Define.buffType.Permanant) chara.bufflist[i].leftdur--;
            if ((chara.bufflist[i].leftdur <= 0  && chara.bufflist[i].bufftype != buffType.Charge )|| 
                (chara.bufflist[i].stackable && chara.bufflist[i].stack <= 0) || //버프 삭제: 지속턴 0 / 스택0 / 쉴드0 일 시
                (chara.bufflist[i].bufftype == Define.buffType.Shield && chara.shield <= 0))
            {
                DestroyBuff(chara,chara.bufflist[i]);
            }
        }
    }

    public void UpdateBuffStatus(Basebuff buff)
    {
        buff.leftdur--;
        if ((buff.leftdur <= 0 && buff.bufftype != buffType.Charge) ||
                (buff.stackable && buff.stack <= 0) || //버프 삭제: 지속턴 0 / 스택0 / 쉴드0 일 시
                (buff.bufftype == Define.buffType.Shield && buff.affectedchara.shield <= 0))
        {
            DestroyBuff(buff.affectedchara, buff);
        }
    }

    public void DecreaseBuffStack(string buffname, Character chara, int num) //버프 스택 감소
    {
        Type b = Type.GetType($"{buffname}Buff");
        Basebuff go = (Basebuff)chara.gameObject.GetComponent(b);
        go.UpdateStack(num);
        if (go.stack <= 0)
        {
            DestroyBuff(chara,go);
        }
    }

    public void DestroyBuff(Character chara, Basebuff buff) //버프 삭제
    {
        Basebuff temp = buff;
        chara.gui.DelBuffGUI(buff);
        chara.bufflist.Remove(buff);
        Destroy(temp);
    }
}

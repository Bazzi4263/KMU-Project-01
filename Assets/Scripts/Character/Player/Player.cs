using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class Player : Character
{
    public IEnumerator attack(List<Character> targets, Data data, float[] rates)
    {
        if(data.type != Define.actionType.Combined) SetSkill(data.name);
        float waitsec = 1f;
        switch (data.attackType)
        {
            case Define.attackType.melee:
                waitsec = 1f + 0.75f * (data.count - 1);
                break;
            case Define.attackType.ranged:
                waitsec = 0.5f + 0.75f * (data.count - 1);
                break;
        }
        if (data.requireResource) // 스킬이 특수 자원 필요 시 감소시키기
        {
            BuffManager.Instance.DecreaseBuffStack(data.required_resourcename, this, -data.required_resourcenum);
        }

        int dmg = Mathf.RoundToInt(_buffedstats.attack_power * rates[0]);

        if (_status[Define.characterStatus.Cloak]) //은신 상태 시 공격력 증가 및 은신 삭제
        {
            List<CloakingBuff> b = bufflist.OfType<CloakingBuff>().ToList();
            for (int i = b.Count-1; i >=0; i--)
            {
                CloakingBuff c = b[i];
                //dmg += c.amount;
                BuffManager.Instance.DestroyBuff(this, c);
            }
        }

        if (!data.issplash && !data.ispierce)
        {
            targets.Sort((x, y) =>
            {
                return x.transform.position.x.CompareTo(y.transform.position.x);
            });
        }

        if (data.target == Define.Target.OneEnemy) 
            Manager.UI.SetDialog($"{_stats.name}(이)가 {targets[0]._stats.name}을(를) 공격!");
        else if (data.target == Define.Target.AllEnemy)
            Manager.UI.SetDialog($"{_stats.name}(이)가 적 전체를 공격!");

        if (data.attackType == Define.attackType.melee) //근접 공격시 이동
        {
            MoveToTarget(targets[0]);
            yield return Manager.Data.wfs15;
        }

        foreach (Monster monster in targets)
        {
            if (data.ispierce)
            {
                StartCoroutine(monster.getDamaged(dmg, data.ignorearmor, data.count, effect: data.effect, particle:data.particle));
                dmg = Mathf.RoundToInt(dmg - dmg * data.rates[1]);
            }
            else if (data.issplash)
            {
                StartCoroutine(monster.getDamaged(dmg, data.ignorearmor, data.count, effect: data.effect, particle:data.particle));
                dmg = Mathf.RoundToInt(_buffedstats.attack_power * rates[1]);
            }
            else
            {
                StartCoroutine(monster.getDamaged(dmg, data.ignorearmor, data.count, effect: data.effect, particle: data.particle));
            }
        }
            
        if (data.type == Define.actionType.Combined) // 공격+(디)버프 스킬일 시 대상에 버프 추가
        {
            StartCoroutine(BuffManager.Instance.SetBuff(data, targets, this, true));
        }

        yield return new WaitForSeconds(waitsec);

        // Warrior Frantic 패시브 스택 증가.
        if (_stats.name == "Warrior")
        {
            if (_stats.level >= _stats.skills["Frantic"].required_level &&
                (Manager.Battle._data.type == Define.actionType.Attack || Manager.Battle._data.type == Define.actionType.Combined))
            {
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_stats.skills["Frantic"], new List<Character> { this }, this, false));
            }
        }

        if (data.attackType == Define.attackType.ranged && data.type != Define.actionType.Combined)
        {
            onTurnEnd();
        }
    }

    protected override void init()
    {
        base.init();
        //Util.GetOrAddComponent<SpriteRenderer>(_takeDamage).flipX = true;
    }
}

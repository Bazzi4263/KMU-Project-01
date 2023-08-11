using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public abstract class Monster : Character
{
    protected override void init()
    {
        _stats = Manager.Data.SetStatData(this.GetType().Name, false);
        base.init();
    }

    public virtual Player selectTarget(List<Player> player) // 타겟 선택
    {
        if (_stats.hasAI) // 플레이어 타겟 순위 1.몬스터가 현재 죽일 수 있는 적 2.프리스트 3. 현재 체력이 가장 낮은 캐릭터 
        {
            List<Player> diyingPlayers = player.FindAll(x => x._stats.currenthp + x._buffedstats.armor - _buffedstats.attack_power <= 0);
            if (diyingPlayers.Count != 0)
            {
                Player diyingPriest = diyingPlayers.Find(x => x._stats.name == "Priest");
                if (diyingPriest) return diyingPriest;
                else return diyingPlayers[0];
            }

            Player priest =  player.Find(x => x._stats.name == "Priest");
            if (priest) return priest;

            return player.OrderBy(x => x._stats.currenthp).ToList()[0];
        }
        else
        {
            List<Player> target = new List<Player>();
            foreach (Player p in player)
            {
                if (p._status[Define.characterStatus.Taunt]) return p;
                if (!p._status[Define.characterStatus.Cloak] && !p._stats.isdead) target.Add(p);
            }
            if (target.Count == 0) return null;
            return target[Random.Range(0, target.Count)];
        }
    }

    public virtual IEnumerator attack(List<Player> players)
    {
        Manager.UI.SetDialog($"{_stats.name}(이)가 {players[0]._stats.name}을(를) 공격!");
        MoveToTarget(players[0]);

        yield return Manager.Data.wfs15;

        int dmg = Mathf.RoundToInt(_buffedstats.attack_power);

        foreach (Player player in players)
        {
            if (_stats.hasSplash)
            {
                StartCoroutine(player.getDamaged(dmg, effect: "Blow", particle: Define.ParticleTypes.Blood));
                dmg = Mathf.RoundToInt(_buffedstats.attack_power * 0.75f);
            }
            else
            {
                StartCoroutine(player.getDamaged(dmg, effect: "Blow", particle:Define.ParticleTypes.Blood));
            }

            if (_stats.debuffname != null)
            {
                if (UnityEngine.Random.Range(0, 101) <= _stats.debuffPercentage)
                {
                    Manager.Battle._data = Manager.Data.MonsterSkillDict[_stats.debuffname];
                    CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(Manager.Battle._data, new List<Character> { player }, this, false));
                }
            }
        }
    }

    public override void Death()
    {
        base.Death();
        Manager.Battle.gold += GetGold();
        Manager.Battle.exp += GetExp();
        Invoke("DeActivate", 5f);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    public int GetGold()
    {   
        return Random.Range(_stats.mingold, _stats.maxgold);
    }

    public int GetExp()
    {
        return _stats.exp;
    }
}

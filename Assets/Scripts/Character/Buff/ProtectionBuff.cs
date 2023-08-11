using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProtectionBuff : Basebuff
{
    int changeddmg;
    protected override void init()
    {
        affectedStatus = Define.characterStatus.Taunt;
        base.init();

        amount = Mathf.RoundToInt(Manager.Battle._data.rates[0] * caster._buffedstats.attack_power);
        changeddmg = Mathf.RoundToInt(Manager.Battle._data.rates[1] * caster._buffedstats.attack_power);

        affectedchara._buffedstats.armor -= amount;
        affectedchara._buffedstats.attack_power -= changeddmg;
        affectedchara._status[Define.characterStatus.Taunt] = true;
        description = $"모든 적 도발, 방어력 {amount} 감소, 공격력 {changeddmg} 감소";
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.attack_power += changeddmg;
        affectedchara._buffedstats.armor += amount;
        base.OnDestroy();
    }
}


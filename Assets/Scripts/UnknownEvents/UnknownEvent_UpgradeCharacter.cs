using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnknownEvent_UpgradeCharacter : UnknownEvent
{
    public override void Effect()
    {
        Stat player = Manager.Data.playerstats[Random.Range(0, Manager.Data.playerstats.Count)];
        string effectInfo = "";

        switch (player.name)
        {
            case "Warrior":
                effectInfo = "잃은 체력의 50% 만큼 방어력이 상승합니다.";
                player.skills["BerserkII"].required_level = 1;
                break;
            case "Archer":
                effectInfo = "모든 공격스킬에 관통효과가 적용 됩니다. (한 적마다 관통시 대미지 25% 감소)";
                foreach (var skill in player.skills.Values)
                    if (skill.type == Define.actionType.Attack && skill.name != "Advanced_Piercing_Arrow" && skill.name != "Piercing_Arrow")
                    { 
                        skill.ispierce = true;
                        skill.rates = new float[] { skill.rates[0], 0.25f };
                    }
                break;
            case "Knight":
                effectInfo = "모든 디버프에 면역이 됩니다.";
                player.skills["FocusII"].required_level = 1;
                break;
            case "Magician":
                effectInfo = "공격력이 100% 증가합니다.";
                player.attack_power *= 2;
                break;
            case "Priest":
                effectInfo = "모든 버프 스킬의 효과가 50% 증가합니다.";
                foreach (var skill in player.skills.Values)
                    if (skill.type == Define.actionType.Buff && skill.name != "Guard" && skill.name !=  "Genesis") skill.rates[0] *= 1.5f;
                break;
            case "Thief":
                effectInfo = "모든 버프, 디버프 스킬의 지속시간이 50% 증가합니다.";
                foreach (var skill in player.skills.Values)
                    if (skill.type == Define.actionType.Buff || skill.type == Define.actionType.Debuff && skill.name != "Guard") skill.duration[0] = Mathf.RoundToInt(skill.duration[0] * 1.5f);
                break;
            case "Fighter":
                effectInfo = "처음 5턴동안 스피드가 500% 증가합니다.";
                player.skills["Speed_InfusionII"].required_level = 1;
                break;
        }

        Manager.UI.SetDialog($"{player.name} 이/가 능력이 강해져 {effectInfo}");
    }
}

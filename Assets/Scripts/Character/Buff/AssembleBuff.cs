using UnityEngine;

public class AssembleBuff : Basebuff
{
    protected override void init()
    {
        base.init();
        amount =  Mathf.RoundToInt(caster._buffedstats.attack_power * Manager.Battle._data.rates[0]);
        description = $"{duration}턴동안 방어력 {amount} 증가";
        affectedchara._buffedstats.armor += amount;
    }

    protected override void OnDestroy()
    {
        affectedchara._buffedstats.armor -= amount;
        base.OnDestroy();
    }
}

using UnityEngine;
using DG.Tweening;

public class Basebuff : MonoBehaviour
{   
    public Character affectedchara; // 버프 적용자
    public Character caster; // 버프 시전자
    public Sprite sprite;
    public int duration; // 지속 턴
    public int leftdur; // 남은 턴. 0이 되면 destroy
    public string buffname, description;
    public Define.buffType bufftype;
    public Define.characterStatus affectedStatus = Define.characterStatus.None;
    public bool stackable = false;
    public int stack, maxstack;
    public int amount; // 쉴드 양, 도트 뎀 양 등을 결정
    public string effect; // 발생시킬 이펙트명 = 스킬 data에 있음
    public Define.ParticleTypes particle; // 발생시킬 파티클 = 스킬 data에 있음

    void Awake()
    {
        init();
    }

    protected virtual void init()
    {
        caster = Manager.Battle.nowTurnChar;
        affectedchara = gameObject.GetComponent<Character>();
        sprite = Manager.Item.GetSprite(Manager.Battle._data.spritenum);
        duration = leftdur = Manager.Battle._data.duration?[0] ?? 0;
        affectedchara.gui.SetBuffGUI(this);
        buffname = Manager.Battle._data.krname;
        description = Manager.Battle._data.description;
        bufftype = Manager.Battle._data.bufftype;
        //amount = Mathf.RoundToInt(caster._stats.attack_power * Manager.Battle._data.rates[0]);
        effect = Manager.Battle._data.effect;
        particle = Manager.Battle._data.particle;
        affectedchara.GenerateParticle(particle);
        affectedchara.GenerateEffect(effect);
    }

    protected virtual void OnDestroy()
    {
        
    }

    public virtual void ApplyBuffEffect() // awake(init)이후에 적용되야 할 효과
    {

    }

    public virtual void UpdateStack(int num)
    {
        stack = Mathf.Min(stack + num, maxstack);
    }

    public virtual void EffectOnEffectedTurnEnd() // 버프 있는 상태로 턴 종료 시 효과. 도트뎀 등
    {

    }

    public virtual void EffectOnEffectedTurnStart() // 턴 시작시 효과 발동.
    {

    }

    public virtual void EffectOnCasterTurnStart()
    {

    }

    public virtual void EffectOnTurnStart()
    {

    }
}

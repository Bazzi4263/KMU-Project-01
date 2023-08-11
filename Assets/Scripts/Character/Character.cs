using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering;
using static Define;
using System.Linq;
using Unity.Mathematics;

/* Character -> Player, Monster
 Player -> Warrior, Magician...
 Monster -> Slime, Ghost...*/

public abstract class Character : MonoBehaviour
{
    public Stat _stats;
    public Stat _buffedstats;
    public float turngauge;
    public float shield;
    GameObject dmgtextgo;
    public CharGUI gui;
    public int resourcenum; //스킬 사용에 필요한 특수 자원 개수 (마법사의 기운 등)
    public List<Basebuff> bufflist = new List<Basebuff>();
    //protected Animator _anim;
    //protected GameObject _takeDamage;
    public Dictionary<Define.characterStatus, bool> _status = new Dictionary<Define.characterStatus, bool>(); // 은신, 도발 등 캐릭터 현재 상태 저장

    public bool isEnd = false; // 턴 버그 막기위한 변수

    private void Awake()
    {
        foreach (Define.characterStatus status in Enum.GetValues(typeof(Define.characterStatus)))
        {
            if (status != Define.characterStatus.None)
                _status[status] = false;
        }

    }

    private void Start()
    {
        init();
    }

    protected virtual void init()
    {
        _buffedstats = _stats.Clone();
        //SetDamagedEffect();
    }

    public virtual void Heal(float num) 
    {
        num = Mathf.RoundToInt(num);
        SpriteBlink("good");
        GenerateParticle(ParticleTypes.Green);
        _stats.currenthp = Mathf.Min(_stats.currenthp + num, _stats.maxhp);
        CreateDmgText(num, "heal");
    }

    public IEnumerator getDamaged(float rawdmg, bool ignorearmor = false, int count = 1, bool isBuffDamage = false, string effect = "None", ParticleTypes particle = ParticleTypes.None) //count = 공격 횟수 n데미지로 m번 공격할 경우를 표시하기 위해
    {
        Debug.Log(effect);
        // Thief 회피 체크
        if (_stats.name == "Thief" && !isBuffDamage)
        {
            if (_stats.level >= _stats.skills["Fake"].required_level && UnityEngine.Random.Range(0, 1.0f) <= _stats.skills["Fake"].rates[0])
            {
                _status[Define.characterStatus.Dodge] = true;
                CreateDmgText(0, "miss");
                yield return new WaitForSeconds(0.3f);
                _status[Define.characterStatus.Dodge] = false;
                yield break;
            }
        }

        float realdmg;
        if (!ignorearmor) realdmg = rawdmg - _buffedstats.armor < 0 ? 0 : rawdmg - _buffedstats.armor;
        else realdmg = rawdmg;

        realdmg = realdmg == 0 ? 1 : realdmg;

        for (int i = 0; i < count; i++)
        {
            GenerateEffect(effect);
            GenerateParticle(particle);
            CreateDmgText(realdmg, "attack");
            if(shield > 0)
            {   
                /*
                 * 쉴드를 거는 버프스킬이 2개 이상 생길 시 코드 수정 필요
                 */
                float decreased_total_hp = shield - realdmg <= 0 ? shield : realdmg;
                shield -= decreased_total_hp;
                realdmg -= decreased_total_hp;
                if(shield <= 0)
                {   
                    for(int j = bufflist.Count - 1; j>=0; j--)
                    {
                        if (bufflist[j].bufftype == Define.buffType.Shield)
                        {
                            BuffManager.Instance.DestroyBuff(this, bufflist[j]);
                        }
                    }
                }
            }
            _stats.currenthp = _stats.currenthp - realdmg < 0 ? 0 : _stats.currenthp - realdmg;
            SpriteBlink("bad");
            Manager.Sound.Play("Effect/Stab");
            yield return new WaitForSeconds(0.1f);

            // Thief Drain 패시브 효과
            if (!isBuffDamage)
            {
                if (Manager.Battle._data != null)
                {
                    if (Manager.Battle._data.name == "Drain" && Manager.Battle.nowTurnChar._stats.name == "Thief")
                    {
                        Manager.Battle.nowTurnChar.GenerateEffect("Heal");
                        Manager.Battle.nowTurnChar.GenerateParticle(ParticleTypes.Green);
                        Manager.Battle.nowTurnChar.Heal(Mathf.Min(realdmg * Manager.Battle._data.rates[0], Manager.Battle.nowTurnChar._stats.maxhp / 2));
                    }
                }
            }
        }

        yield return Manager.Data.wfs5;

        if(_stats.currenthp <= 0)
        {
            onDeath();
        }
    }

    public void Revive() //부활
    {
        Manager.Battle.player.Add((Player)this);
        _stats.currenthp = _stats.maxhp;
        _stats.isdead = false;
        gameObject.GetComponent<SpriteRenderer>().DOFade(1, 1.5f);
        gui.GetComponent<CanvasGroup>().DOFade(1, 1.5f);
        SpriteBlink("good");
        GenerateEffect(Manager.Battle._data.effect);
        GenerateParticle(Manager.Battle._data.particle);
    }

    protected void MoveToTarget(Component target)
    {
        Vector3 adjustPos = new Vector3();
        Vector3 originalPos = transform.position;

        float waitTime = 0;

        switch (target)
        {
            case Monster:
                adjustPos.Set(-(target.GetComponent<BoxCollider2D>().size.x * target.transform.localScale.x / 2
                    + GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2), 0, 0);
                waitTime = 1.2f + Manager.Battle._data.count * 0.1f;
                break;
            case Player:
                adjustPos.Set(target.GetComponent<BoxCollider2D>().size.x * target.transform.localScale.x / 2
                    + GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 2, 0, 0);
                waitTime = 1.2f;
                break;
            default:
                adjustPos.Set(0, 0, 0);
                break;
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(target.transform.position.x + adjustPos.x, transform.position.y), 1f).SetEase(Ease.OutQuart))
        .AppendInterval(waitTime)
        .Append(transform.DOMove(originalPos, 1f).SetEase(Ease.OutQuart).OnComplete( () => { onTurnEnd(); } ));
    }

    void onDeath()
    {
        if (_stats.name == "Warrior")
        {
            if (_stats.level >= _stats.skills["Reincarnation"].required_level && _stats.skills["Reincarnation"].pp > 0)
            {
                _stats.skills["Reincarnation"].pp--;
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_stats.skills["Reincarnation"], new List<Character> { this }, this, false));
            }
            else if (bufflist.Find(x => x.GetType().Name == "ReincarnationBuff"))
            {

            }
            else
            {
                Death();
            }
        }
        else
        {
            Death();
        }
    }

    public virtual void Death()
    {
        _stats.isdead = true;
        turngauge = 0;
        gameObject.GetComponent<SpriteRenderer>().DOFade(0, 1.5f);
        gui.GetComponent<CanvasGroup>().DOFade(0, 1.5f);
        resourcenum = 0;
        foreach (Basebuff b in bufflist) //버프 초기화
        {
            gui.DelBuffGUI(b);
            Destroy(b);
        }
        bufflist.Clear();
        foreach (Define.characterStatus status in Enum.GetValues(typeof(Define.characterStatus))) // 상태 초기화
        {
            _status[status] = false;
        }
        if (this is Player)
        {
            Manager.Battle.player.Remove((Player)this);
        }
        else if (this is Monster)
        {
            Manager.Battle.enemy.Remove((Monster)this);
        }
        if (Manager.Battle.nowTurnChars.Contains(this)) Manager.Battle.nowTurnChars.Remove(this);
    }

    public void onTurnEnd(Data data = null, List<Character> targets = null) //턴 종료 시
	{
        turngauge -= 100;

        if (data != null) // Empress's Ring 유물 효과 적용
        {
            if (Manager.Artefact.currentArtefacts.FindIndex(x => x.name == "Empress's Ring") != -1 && data.bufftype == Define.buffType.Buff && data.name != "Guard")
            {
                this.turngauge = 100;
                Manager.Data.time += (int)Math.Round(3f * Math.Sqrt(20f / _buffedstats.speed), 2);
                return;
            }
        }

        foreach (Basebuff buff in bufflist)
        {   
            buff.EffectOnEffectedTurnEnd();
        }

        Manager.Data.time += (int)Math.Round(3f * Math.Sqrt(20f / _buffedstats.speed), 2);

        foreach(Basebuff buff in bufflist)
        {
            if(buff.affectedStatus != Define.characterStatus.None && !(buff.bufftype == Define.buffType.Charge && buff.leftdur <= 0))
            {
                _status[buff.affectedStatus] = true; // 같은 status를 활성화하는 버프가 2개 이상 있을 경우를 대비해서 status 활성화
            }
        }

        if ( data!=null && data.type != Define.actionType.Recover && targets.Contains(this))// 지금 턴에 자신에게 추가된 버프는 지속턴 감소시키지 않도록함
            BuffManager.Instance.UpdateBuffStatus(this, Type.GetType($"{data.name}Buff"));
        else
            BuffManager.Instance.UpdateBuffStatus(this);
        if(_stats.currenthp <= 0)
        {
            onDeath();
        }
    }

    public void onTurnStart(Character nowTurnCharacter, Data data = null, List<Character> targets = null)
    {
        List<Basebuff> temp = bufflist.ToList();

        foreach (Basebuff buff in temp)
        {
            if (this == nowTurnCharacter)
            {
                buff.EffectOnEffectedTurnStart();
            }
            buff.EffectOnTurnStart();
        }
    }

    private void OnMouseEnter()
    {
        if (BaseScene.Scene != Define.Scenes.BattleScene || gui == null || (_stats.isdead && Manager.Battle.state != BattleState.SELECTINGDEADTARGET)) { return; }
        gui.ShowGUI();

        if(Manager.Battle.state == Define.BattleState.PLAYERTURN || Manager.Battle.state == Define.BattleState.ENEMYTURN
            || Manager.Battle.state == Define.BattleState.TURNPROCESSING || Manager.Battle.state == BattleState.WON)
        {
            CharTooltipWindow.Instance.chara = this;
            CharTooltipWindow.Instance.show(bufflist);
        }
    }
    private void OnMouseExit()
    {
        if (BaseScene.Scene != Define.Scenes.BattleScene || gui == null) { return; }
        gui.HideGUI();
        CharTooltipWindow.Instance.hide();
    }

    public virtual void SetSkill(string skill)
    {
        if (!_stats.skills.ContainsKey(skill))
        {
            return;
        }

        if (_stats.skills[skill].pp <= 0)
        {
            return;
        }
        else
        {
            _stats.skills[skill].pp--;
        }
    }
   
    public void UseItem(Item itemdata)
    {
        itemdata.pp--;

        Manager.Battle.state = Define.BattleState.IDLE;
        switch (itemdata.itemtype)
        {
            case Define.itemType.Recover:
                if (_stats.currenthp == _stats.maxhp)
                {
                    Manager.UI.SetDialog("대상은 이미 최대 체력입니다!");
                    Manager.Battle.SetCharAlpha(null, 1f);
                    Manager.Battle.changeTurn();
                }
                else
                {
                    float amount = _stats.maxhp >= (_stats.currenthp + itemdata.rates[0]) ? itemdata.rates[0] : _stats.maxhp - _stats.currenthp;
                    Manager.UI.SetDialog($"{_stats.name}의 체력을 {amount} 회복!");
                    Heal(amount);
                    Manager.Battle.SetCharAlpha(null, 1f);
                    Manager.Battle.nowTurnPlayer.onTurnEnd();
                    CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
                }
                break;
            case Define.itemType.InstantDmg:
                _stats.currenthp -= itemdata.rates[0];
                SpriteBlink("bad");
                CreateDmgText(itemdata.rates[0], "attack");
                Manager.Battle.SetCharAlpha(null, 1f);
                Manager.Battle.nowTurnPlayer.onTurnEnd();
                CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
                break;
            case Define.itemType.Buff:
                break;
            case Define.itemType.Permanant:
                Manager.UI.SetDialog($"{_stats.name}의 {itemdata.affectedstat}(이)가 영구적으로 {itemdata.rates[0]} 증가!");
                SpriteBlink("good");
                switch (itemdata.affectedstat)
                {   
                    case Define.affectedStat.hp:
                        _stats.maxhp += (int)itemdata.rates[0];
                        _buffedstats.maxhp += (int)itemdata.rates[0];
                        _stats.currenthp += (int)itemdata.rates[0];
                        _buffedstats.currenthp += (int)itemdata.rates[0];
                        break;
                    case Define.affectedStat.speed:
                        _stats.speed += (int)itemdata.rates[0];
                        _buffedstats.speed += (int)itemdata.rates[0];
                        break;
                    case Define.affectedStat.armor:
                        _stats.armor += (int)itemdata.rates[0];
                        _buffedstats.armor += (int)itemdata.rates[0];
                        break;
                    case Define.affectedStat.attack_power:
                        _stats.attack_power += (int)itemdata.rates[0];
                        _buffedstats.attack_power += (int)itemdata.rates[0];
                        break;

                }
                Manager.Battle.nowTurnPlayer.onTurnEnd();
                Manager.Battle.SetCharAlpha(null, 1f);
                CoroutineHelper.StartCoroutine(Manager.Battle.TurnProcess());
                break;
        }

    }

    public void CreateDmgText(float dmg, string type)
    {
        dmgtextgo = Manager.Resource.Instantiate("battlescene/dmg");
        dmgtextgo.transform.SetParent(gui.transform);
        dmgtext dmgtext = dmgtextgo.GetComponent<dmgtext>();
        switch (type)
        {
            case "attack":
                dmgtext.dmg = $"-{dmg}";
                dmgtext.color = Color.red;
                break;
            case "heal":
                dmgtext.dmg = $"+{dmg}";
                dmgtext.color = Color.green;
                break;
            case "immune":
                dmgtext.dmg = $"면역";
                dmgtext.color = Color.white;
                break;
            case "miss":
                dmgtext.dmg = $"회피";
                dmgtext.color = Color.yellow;
                break;
        }

    }

    public void SpriteBlink(string type)
    {
        Sequence sequence = DOTween.Sequence();
        switch (type)
        {
            case "bad":
                sequence.Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f));
                break;
            case "good":
                sequence.Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.blue, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.blue, 0.1f))
                .AppendInterval(0.1f)
                .Append(gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f));
                break;
        }
    }

    //void SetDamagedEffect()
    //{
    //    _takeDamage = Manager.Resource.Instantiate("Effect/TakeDamage", default, gameObject.transform);
    //    _takeDamage.transform.position = gameObject.transform.position;
    //    Debug.Log(gameObject.transform.localScale.y);
    //    if(this is Monster) _takeDamage.transform.position += new Vector3(0, gameObject.transform.localScale.y, 0);
    //    _anim = Util.GetOrAddComponent<Animator>(_takeDamage);
    //    _anim.runtimeAnimatorController = Manager.Resource.Load<RuntimeAnimatorController>("Animations/DamageAnimController");
    //    Util.GetOrAddComponent<SpriteRenderer>(_takeDamage).sortingLayerName = "Effect";
    //}

    public void GenerateParticle(ParticleTypes particle)
    {
        if (particle == ParticleTypes.None) return;
        var p = Manager.Resource.Instantiate($"Effect/Particle/{particle}", default, gameObject.transform);
        p.transform.position = gameObject.transform.position;
        if (this is Monster && particle == ParticleTypes.Blood) p.transform.position += new Vector3(0, gameObject.transform.localScale.y, 0);
        else if (this is Player && particle != ParticleTypes.Blood) p.transform.position -= new Vector3(0, gameObject.transform.localScale.y/2, 0);
    }

    public void GenerateEffect(string effect)
    {
        Debug.Log(effect);
        if (effect.Equals("None")) return;
        var p = Manager.Resource.Instantiate($"Effect/RealEffect/{effect}", default, gameObject.transform);
        p.transform.position = gameObject.transform.position;
        if (this is Monster) p.transform.position += new Vector3(0, gameObject.transform.localScale.y, 0);
        else if (this is Player) p.transform.position += new Vector3(0, 0.3f, 0);
    }
}

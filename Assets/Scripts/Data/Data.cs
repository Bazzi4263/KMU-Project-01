using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class DictionaryMapTileInfo : SerializeDictionary<Vector3Int, Tiledata> { }

[Serializable]
public class DictionaryItemInfo : SerializeDictionary<string, Item> { }

[Serializable]
public class DictionarySkillInfo : SerializeDictionary<string, Skill> 
{
    public DictionarySkillInfo Clone()
    {
        DictionarySkillInfo skill = new DictionarySkillInfo();

        foreach(KeyValuePair<string, Skill> s in this)
        {
            skill.Add(s.Key, s.Value.Clone());
        }

        return skill;
    }
}

[Serializable]
public class Option
{
    public float totalvolume;
    public float effectvolume;
    public float bgmvolume;

    public Option()
    {
        totalvolume = 0;
        effectvolume = 0;
        bgmvolume = 0;
    }
}

[Serializable]
public class Save
{
    public Define.Scenes currentscene;
    public Define.STAGE currentstage;
    public Define.battleType currentbattletype;
    public Stat dungeonMercenaryStat = new Stat();
    public List<Stat> players = new List<Stat>();
    public List<string> monsternames;
    public int time, gold;
    public DictionaryItemInfo items = new DictionaryItemInfo();
    public DictionaryMapTileInfo tileDic = new DictionaryMapTileInfo();
    public DictionaryMapTileInfo underTileDic = new DictionaryMapTileInfo();
    public List<int> monsterCount = new List<int>();
    public List<Define.MonsterType> monsterType = new List<Define.MonsterType>();
    public List<Vector3> monsterpos = new List<Vector3>();
    public List<Artefact> artefact = new List<Artefact>();
    public Vector3 characterPos = new Vector3();
}

[Serializable]
public class Level
{
    public int level, exp;
}

[Serializable]
public class LevelData : ILoader<int, Level>
{
    public List<Level> levels;

    public Dictionary<int, Level> Load()
    {
        Dictionary<int, Level> dict = new Dictionary<int, Level>();
        foreach (Level level in levels)
            dict.Add(level.level,level);

        return dict;
    }
}

#region Stat
[Serializable]
public class Stat
{
    public string name, debuffname;
    public float speed, attack_power, armor, currenthp, debuffPercentage;
    public int maxhp, range, mingold, maxgold, exp, level;
    private int upgradeCount;
    public bool isdead, isboss, hasSplash, hasAI;
    public DictionarySkillInfo skills = new DictionarySkillInfo();
    //public Dictionary<string, Skill> skills;
    public List<Sprite> sprites;

    public int UpgradeCount { get => upgradeCount; set { upgradeCount = value; if (upgradeCount % 2 == 0) attack_power++; } }

    public Stat Clone()
    {
        Stat statclone = new Stat();
        statclone.name = this.name;
        statclone.debuffname = this.debuffname;
        statclone.debuffPercentage = this.debuffPercentage;
        statclone.maxgold = this.maxgold;
        statclone.mingold = this.mingold;
        statclone.maxhp = this.maxhp;
        statclone.currenthp = this.currenthp;
        statclone.attack_power = this.attack_power;
        statclone.armor = this.armor;
        statclone.range = this.range;
        statclone.speed = this.speed;
        statclone.isdead = this.isdead;
        statclone.upgradeCount = this.upgradeCount;
        statclone.skills = this.skills.Clone();
        statclone.sprites = this.sprites;
        statclone.isboss = this.isboss;
        statclone.exp = this.exp;
        statclone.level = this.level;
        statclone.hasSplash = this.hasSplash;
        statclone.hasAI = this.hasAI;
        return statclone;
    }
}

[Serializable]
public class MonsterStat : Stat
{
    
}

[Serializable]
public class PlayerStat : Stat
{

}

[Serializable]
public class StatData : ILoader<string, Stat>
{
    public List<Stat> stats;

    public Dictionary<string, Stat> Load()
    {
        Dictionary<string, Stat> dict = new Dictionary<string, Stat>();
        foreach (Stat stat in stats)
            dict.Add(stat.name, stat);

        return dict;
    }
}
#endregion

public class Data
{
    public string name;
    public string krname;
    public int spritenum;
    public string description;
    public bool ignorearmor = false;
    public float[] rates = {1f};
    public int count;
    public int[] duration = {0};
    public int pp;
    public int maxpp;
    public Define.actionType type;
    public Define.Target target;
    public Define.attackType attackType;
    public Define.buffType bufftype;
    public Define.buffTarget bufftarget = Define.buffTarget.Target;
    public bool requireResource = false; // 특수 자원 필요 여부
    public int required_resourcenum = 0; // 특수 자원 필요 개수
    public string required_resourcename = null; // 특수 자원을 관리하는 스킬 이름
    public bool ispierce = false;
    public bool issplash = false;
    public string effect = "None";
    public Define.ParticleTypes particle = Define.ParticleTypes.None;
}

#region Skill
[Serializable]
public class Skill : Data 
{   
    public int required_level;
    public bool isSpecial = false;

    public Skill Clone()
    {
        Skill clone = new Skill();
        clone.name = this.name;
        clone.krname = krname;
        clone.spritenum = this.spritenum;
        clone.description = this.description;
        clone.pp = this.pp;
        clone.maxpp = this.maxpp;
        clone.count = this.count;
        clone.type = this.type;
        clone.duration = duration;
        clone.ignorearmor = this.ignorearmor;
        clone.target = this.target;
        clone.bufftarget = this.bufftarget;
        clone.rates = this.rates;
        clone.attackType = this.attackType;
        clone.bufftype = bufftype;
        clone.requireResource = this.requireResource;
        clone.required_resourcenum = this.required_resourcenum;
        clone.required_resourcename = this.required_resourcename;
        clone.required_level = this.required_level;
        clone.ispierce = this.ispierce;
        clone.issplash = this.issplash;
        clone.isSpecial = this.isSpecial;
        clone.effect = this.effect;
        clone.particle = this.particle;
        return clone;
    }
}

[Serializable]
public class SkillData : ILoader<string, Skill>
{
    public List<Skill> skills;

    public Dictionary<string, Skill> Load()
    {
        Dictionary<string, Skill> result = new Dictionary<string, Skill>();
        foreach (Skill skill in skills)
        {
            result.Add(skill.name, skill);
        }
        return result;
    }
}
#endregion

#region Item
[Serializable]
public class Item : Data
{
    public int gold;
    public Define.itemType itemtype;
    public Define.affectedStat affectedstat;
}

[Serializable]
public class ItemData : ILoader<string, Item>
{
    public List<Item> items;

    public Dictionary<string, Item> Load()
    {
        Dictionary<string, Item> result = new Dictionary<string, Item>();
        foreach (Item item in items)
            result.Add(item.name, item);

        return result;
    }
}
#endregion

#region Artifact

[Serializable]
public class Artefact : Data
{
    public string name_KR;
    public string classname;
    public int gold;
    public Define.grade grade;
    public Define.artefactType artefacttype;
    public Define.affectedStat affectedstat;
    public Define.artefactTarget artefacttarget;
}

[Serializable]
public class ArtefactData : ILoader<string, Artefact>
{
    public List<Artefact> artefacts;

    public Dictionary<string, Artefact> Load()
    {
        Dictionary<string, Artefact> result = new Dictionary<string, Artefact>();
        foreach (Artefact artefact in artefacts)
            result.Add(artefact.name, artefact);

        return result;
    }
}

#endregion


using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
/*
 * 전반적인 데이터 관리, 오토세이브와 수동세이브
 * 저장할 데이터 목록을 SaveData Class에서 제시하며 일정 시간마다 변수에 세이브 and 원할 때 세이브
 * 맵씬 오토세이브는 이동이 끝난 후, 전투 오토세이브는 전투 시작 시
 * 수동세이브는 필요한가 잘 모르겠음
 */
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> Load();
}

[Serializable]
public class SerializeDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<K> keys = new List<K>();

    [SerializeField]
    List<V> values = new List<V>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<K, V> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0, icount = keys.Count; i < icount; ++i)
        {
            this.Add(keys[i], values[i]);
        }
    }
}

public class DataManager
{
    public Dictionary<string, Skill> SkillDict { get; private set; } = new Dictionary<string, Skill>();
    public Dictionary<string, Skill> MonsterSkillDict { get; private set; } = new Dictionary<string, Skill>();
    public Dictionary<string, Skill> ArtefactSkillDict { get; private set; } = new Dictionary<string, Skill>();
    public Dictionary<string, Stat> PlayerStatDict { get; private set; } = new Dictionary<string, Stat>();
    public Dictionary<string, Stat> MonsterStatDict { get; private set; } = new Dictionary<string, Stat>();
    public Dictionary<int, Level> LevelDict { get; private set; } = new Dictionary<int, Level>();
    public List<Stat> playerstats { get; set; } = new List<Stat>();
    public Stat nowplayer = new Stat();
    public int time;
    public string _filename;
    Save _save = new Save();
    Option _option = new Option();

    public WaitForSeconds wfs5 = new WaitForSeconds(0.5f);
    public WaitForSeconds wfs10 = new WaitForSeconds(1.0f);
    public WaitForSeconds wfs15 = new WaitForSeconds(1.5f);
    public WaitForSeconds wfs20 = new WaitForSeconds(2.0f);
    public WaitForSeconds wfs30 = new WaitForSeconds(3.0f);

    public void Init()
    {
        Sprite[] sprites = Manager.Resource.LoadAll<Sprite>("Art/Icon/icons_full_32");

        for (int i = 0; i < sprites.Length; i++)
            Manager.Item.SetSprite(sprites[i]);

        for (int i = 0; i < sprites.Length; i++) // 중복되기 때문에 스프라이트를 아이템이나 유물에서 말고 다른곳에서 관리해야할듯.
            Manager.Artefact.Sprites.Add(sprites[i]);

        Array.Clear(sprites, 0, sprites.Length);

        SkillDict = LoadJson<SkillData, string, Skill>("Skill").Load();
        MonsterSkillDict = LoadJson<SkillData, string, Skill>("Skill", "Monster").Load();
        ArtefactSkillDict = LoadJson<SkillData, string, Skill>("Skill", "Artefact").Load();
        Manager.Item.items = LoadJson<ItemData, string, Item>("Item").Load();
        Manager.Artefact._artefacts = LoadJson<ArtefactData, string, Artefact>("Artefact").Load();

        PlayerStatDict = LoadJson<StatData, string, Stat>("Stat", "Player").Load();
        foreach (Stat stat in PlayerStatDict.Values)
        {
            Dictionary<string, Skill> skill = new Dictionary<string, Skill>();
            skill = LoadJson<SkillData, string, Skill>("Skill", stat.name).Load();

            foreach (KeyValuePair<string, Skill> skilldata in skill)
            {
                stat.skills.Add(skilldata.Key, skilldata.Value);
            }
        }
        MonsterStatDict = LoadJson<StatData, string, Stat>("Stat", "Monster").Load();
        LevelDict = LoadJson<LevelData, int, Level>("Level").Load();
        foreach (Stat s in PlayerStatDict.Values)
        {
            sprites = Manager.Resource.LoadAll<Sprite>($"Art/Characters/{s.name}");
            for (int i = 0; i < sprites.Length; i++)
                s.sprites.Add(sprites[i]);
        }

        time = 360;

        if(File.Exists(Application.dataPath + "/Resources/Data/Save/SaveData_Option"))
        {
            _option = LoadOptionJson<Option>();
            Manager.Sound.totalvolume = _option.totalvolume;
            Manager.Sound.bgmvolume = _option.bgmvolume;
            Manager.Sound.effectvolume = _option.effectvolume;
        }
    }

    public Stat AddPlayer(string playertype)
    {
        Stat stat = SetStatData(playertype);

        // 원래있는 플레이어의 유물 효과 제거
        foreach (Artefact artefact in Manager.Artefact.currentArtefacts)
            Manager.Artefact.TakeEffectArtefact(artefact, true);

        playerstats.Add(stat);
        _save.players.Add(stat);

        // 새 플레이어를 포함하여 다시 유물 효과 적용
        foreach (Artefact artefact in Manager.Artefact.currentArtefacts)
            Manager.Artefact.TakeEffectArtefact(artefact, false);

        return stat;
    }

    public Dictionary<string, Skill> SetSkillData(string player)
    {
        return LoadJson<SkillData, string, Skill>("Skill", player).Load();
    }

    public Stat SetStatData(string player, bool isplayer = true)
    {
        if (isplayer)
            return PlayerStatDict[player].Clone();
        else
            return MonsterStatDict[player].Clone();
    }

    Loader LoadJson<Loader, Key, Value>(string value, string path = "") where Loader : ILoader<Key, Value>
    {
        TextAsset data = Manager.Resource.Load<TextAsset>($"Data/{value}/{value}Data_{path}");
        return JsonUtility.FromJson<Loader>(data.text);
    }

    Save LoadSaveJson<Save>()
    {
        TextAsset data = Manager.Resource.Load<TextAsset>($"Data/Save/SaveData_Auto");
        return JsonUtility.FromJson<Save>(data.text);
    }

    Option LoadOptionJson<Option>()
    {
        TextAsset data = Manager.Resource.Load<TextAsset>($"Data/Save/SaveData_Option");
        return JsonUtility.FromJson<Option>(data.text);
    }

    public int[] GetTime()
    {
        int[] time = new int[2];
        time[0] = (this.time / 60 >= 24) ? this.time / 60 - 24 : this.time / 60;
        time[1] = this.time % 60;

        return time;
    }

    //#region Save
    //public void SaveGameData()
    //{
    //    //_filename = "SaveData_Auto.json";
    //    //string ToJsonData = JsonUtility.ToJson(_save);
    //    //string path = Application.dataPath + "/Resources/Data/Save/" + _filename;

    //    //if (File.Exists(path))
    //    //{
    //    //    File.Delete(path);
    //    //    File.WriteAllText(path, ToJsonData);
    //    //}
    //    //else
    //    //{
    //    //    File.WriteAllText(path, ToJsonData);
    //    //}
    //}

    //void SaveOptionData()
    //{
    //    _filename = "SaveData_Option.json";
    //    string ToJsonData = JsonUtility.ToJson(_option);
    //    string path = Application.dataPath + "/Resources/Data/Save/" + _filename;

    //    if (File.Exists(path))
    //    {
    //        File.Delete(path);
    //        File.WriteAllText(path, ToJsonData);
    //    }
    //    else
    //    {
    //        File.WriteAllText(path, ToJsonData);
    //    }

    //}

    //public void Load()
    //{
    //    _save = Manager.Data.LoadSaveJson<Save>();
    //    MapManager.Instance.currentStage = _save.currentstage;
    //    MapManager.Instance.isLoadData = true;
    //    MapManager.Instance.save = _save;
    //    if (_save.currentscene == Define.Scenes.BattleScene)
    //    {
    //        Manager.Battle._path = _save.monsternames;
    //        Manager.Battle.battletype = _save.currentbattletype;
    //        if (_save.currentbattletype == Define.battleType.dungeon)
    //            Manager.Battle.dungeonMercenaryStat = _save.dungeonMercenaryStat.Clone();
    //    }

    //    MapManager.Instance.tileDic.Clear();
    //    MapManager.Instance.underTileDic.Clear();
    //    foreach (KeyValuePair<Vector3Int, Tiledata> tile in _save.tileDic) 
    //        MapManager.Instance.tileDic.Add(tile.Key, new TileInfo(tile.Value));
    //    foreach (KeyValuePair<Vector3Int, Tiledata> tile in _save.underTileDic) 
    //        MapManager.Instance.underTileDic.Add(tile.Key, new TileInfo(tile.Value));
    //    foreach (TileInfo tile in MapManager.Instance.tileDic.Values)
    //        tile.UpdateNeighbourTiles();

    //    Manager.Data.time = _save.time;
    //    foreach (KeyValuePair<string, Item> item in _save.items)
    //        Manager.Item.items[item.Key] = item.Value;
    //    Manager.Item.gold = _save.gold;
    //    Manager.Artefact.currentArtefacts = _save.artefact;
    //    foreach (Stat player in _save.players)
    //        playerstats.Add(player.Clone());
    //    Manager.Scene.LoadScene(_save.currentscene);
    //}

    ///// <summary>  
    ///// MapScene 입장 시 사용
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void MapSceneSave()
    //{
    //    _save.currentscene = BaseScene.Scene;
    //    SaveItem();
    //    SaveGold();
    //    SaveMonster();
    //    SaveTime();
    //}

    ///// <summary>  
    ///// Battle Scene에 입장시 BattleScene에 필요한 수치들 저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void BattleSceneSave()
    //{
    //    _save.currentscene = BaseScene.Scene;
    //    _save.monsternames = Manager.Battle._path;
    //    _save.currentbattletype = Manager.Battle.battletype;
    //    if (_save.currentbattletype == Define.battleType.dungeon)
    //        _save.dungeonMercenaryStat = Manager.Battle.dungeonMercenaryStat;
    //}

    ///// <summary>  
    ///// Player 위치 이동 시 사용
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void SavePlayerPos(Vector3 playerpos = default(Vector3))
    //{
    //    _save.characterPos = (playerpos != default(Vector3)) ? playerpos : _save.characterPos;
    //    SaveTime();
    //}

    ///// <summary>  
    ///// Artefact 획득 시 획득한 artefact저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void SaveArtefact(Artefact artefact)
    //{
    //    _save.artefact.Add(artefact);
    //}

    ///// <summary>  
    ///// Volume높낮이 조정하면 저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void SaveVolume()
    //{
    //    _option.effectvolume = Manager.Sound.effectvolume;
    //    _option.totalvolume = Manager.Sound.totalvolume;
    //    _option.bgmvolume = Manager.Sound.bgmvolume;
    //    SaveOptionData();
    //}

    ///// <summary>  
    ///// Portion 사용 or 획득 시 저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //void SaveItem()
    //{
    //    foreach (KeyValuePair<string, Item> item in Manager.Item.items)
    //        if (_save.items.TryAdd(item.Key, item.Value)) _save.items[item.Key] = item.Value;
    //}

    ///// <summary>  
    ///// Gold는 BattleScene-MapScene넘어갈 때와 TownScene-MapScene넘어갈 때 저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //void SaveGold()
    //{
    //    _save.gold = Manager.Item.gold;
    //}

    //void SaveTime()
    //{
    //    _save.time = time;
    //}

    ///// <summary>  
    ///// Stage가 바뀔 때마다 사용
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void MapSceneInitSave()
    //{
    //    _save.tileDic.Clear();
    //    _save.underTileDic.Clear();

    //    foreach (KeyValuePair<Vector3Int, TileInfo> tile in MapManager.Instance.tileDic)
    //        _save.tileDic.Add(tile.Key, tile.Value.tiledata);

    //    foreach (KeyValuePair<Vector3Int, TileInfo> tile in MapManager.Instance.underTileDic)
    //        _save.underTileDic.Add(tile.Key, tile.Value.tiledata);

    //    _save.currentstage = MapManager.Instance.currentStage;
    //    SaveMonster();
    //    SaveGold();
    //    SaveTime();
    //    _save.currentscene = BaseScene.Scene;
    //}

    ///// <summary>  
    ///// Monster는 Monster가 움직일 때, BattleScene-MapScene이동 시 저장
    ///// </summary>
    ///// <param name="lbl"></param>
    //public void SaveMonster()
    //{
    //    /*
    //     * 여러가지 방법을 시도하다가 어쩔 수 없이 몬스터 위치(Vector3), 그 위치의 몬스터 수(List<int>), 몬스터 종류 리스트(List<int>)로 나눴습니다
    //     * 이미 currentStage에 스테이지를 물고 있으니 몬스터 종류 리스트는 Stage에 맞는 값을 물고 있습니다.
    //     * ex) currentStage가 GrassLand일 때 monsterinfo[0]이 3이면 grssland의 enum값이 3인 블루슬라임인 것
    //     * 몬스터 종류 리스트를 몬스터 수만큼 잘라서 쓰세요
    //     */
    //    _save.monsterCount.Clear();
    //    _save.monsterType.Clear();
    //    _save.monsterpos.Clear();

    //    foreach (MonsterInfo monster in MapManager.Instance.MonsterList)
    //    {
    //        _save.monsterCount.Add(monster.monsterTypes.Count);

    //        foreach (Define.MonsterType monsterType in monster.monsterTypes)
    //        {
    //            _save.monsterType.Add(monsterType);
    //        }

    //        _save.monsterpos.Add(monster.transform.position);
    //    }
    //}
    //#endregion

    public void ClearGame()
    {
        Manager.Data.playerstats.Clear();
        Manager.UI.Clear();
        Manager.Item.gold = 100;
        foreach (var item in Manager.Item.items.Values)
            item.pp = 0;
        Manager.Item.items["HPPortion"].pp = 5;
        Manager.Item.items["PPPortion"].pp = 1;
        MapManager.Instance.ClearGame(true);
        Manager.Artefact.ClearArtefact();
        UnityEngine.Object.Destroy(MapManager.Instance.gameObject);
    }
}
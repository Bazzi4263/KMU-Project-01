using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using DG.Tweening;
using static Define;
using UnityEditor;
using System.Xml.Linq;
using static UnityEngine.GraphicsBuffer;

public class BattleManager : MonoBehaviour
{
    public battleType battletype;
    Transform enemyspawnpoint;
    List<Transform> playerspawnpoints = new List<Transform>();
    public List<string> _path = new List<string>();
    public List<Character> nowTurnChars = new List<Character>();
    public List<Monster> enemy = new List<Monster>();
    public List<Player> player = new List<Player>();
    [SerializeField] public List<Character> characters = new List<Character>(); //배틀씬에 있는 플레이어+몬스터
    public float turnspeed = 1f;
    public Stat dungeonMercenaryStat = null;
    Player dungeonMercenary;
    public BattleState state;
    public Character nowTurnChar;
    [SerializeField]  public int gold, exp;
    int wave = 0;
    int maxWave = 0;
    SpriteRenderer fadebg;
    SpriteRenderer endbg;
    /*
     * 실행한 스킬 or 아이템의 컴포넌트를 받아온다. 여기에 타겟정보(전체, 자기자신, 팀원 등)와 이름 등을 가져오기 위함
     */
    public Data _data;
    public Item _itemdata;
    public Player nowTurnPlayer { get; set; }
    public List<Monster> Enemy { get => enemy;}
    public List<Player> Player { get => player; set => player = value; }

    int turnCount;
    Random _rand = new Random();

    bool turnStartEffect = true; // 턴 시작시 발동되는 버프효과 중복 방지 변수

    public void SetUpPath(List<MonsterType> monsterTypes)
    {
        _path.Clear();
        foreach (MonsterType monsterType in monsterTypes)
        {
            string path = null;
            switch (MapManager.Instance.currentStage)
            {
                case STAGE.GRASSLAND:
                    path += $"GrassLand/{monsterType.grassLand}";
                    break;
                case Define.STAGE.DESERT:
                    path += $"Desert/{monsterType.desert}";
                    break;
                case Define.STAGE.SNOWLAND:
                    path += $"SnowLand/{monsterType.snowLand}";
                    break;
            }
            if (path != null)
               _path.Add(path);
        }
    }

    void SetUpMonster()
    {
        for (int i = 0; i < _path.Count; i++)
        {
            enemy.Add(Util.GetOrAddComponent<Monster>(Manager.Resource.Instantiate($"Monsters/{_path[i]}", enemyspawnpoint.transform.position)));
            enemy[i].GetComponent<SpriteRenderer>().DOFade(0, 0f);
            enemy[i].GetComponent<SpriteRenderer>().DOFade(1, 2f);
            enemy[i].tag = "BattleEnemy";
            enemy[i].gui = Manager.Resource.Instantiate("battlescene/CharGUI", enemy[i].transform.position, enemy[i].transform).GetComponent<CharGUI>();
            characters.Add(enemy[i]);
        }
        //스프라이트 크기에 따라 스폰 지점을 변경
        for (int i = 1; i < _path.Count; i++)
        {
            enemy[i].transform.position = new Vector3(enemy[i-1].transform.position.x -(enemy[i - 1].GetComponent<BoxCollider2D>().size.x * enemy[i - 1].transform.localScale.x / 2
               + enemy[i].GetComponent<BoxCollider2D>().size.x * enemy[i].transform.localScale.x / 2) - 1, enemy[i].transform.position.y);

        }
    }

    public IEnumerator SetupBattle()
    {
        wave++;
        maxWave = UnityEngine.Random.Range(4, 7);
        SetupBG();
        /*
         * 전투 리세마라를 방지하기 위해 Save에서 _path와 Mercenary Stat을 저장해두었습니다
         * Load시 _path와 Mercenary Stat에 도로 넣었고, 이럴 경우 밑의 작업은 필요없기에 if를 넣었습니다
         */
        if (_path.Count == 0)
        {
            if (battletype == battleType.dungeon)
            {
                SetDungeonMercenary();
                SetUpPath(GenerateRandomMonsters(wave));
            }
            if (battletype == battleType.town) SetUpPath(GenerateBossMonsters());
        }
        yield return Manager.Data.wfs5;
        fadebg.DOFade(0f, 1f);
        SetUpMonster();

        for (int i = 0; i < Manager.Data.playerstats.Count; i++)
        {
            if (!Manager.Data.playerstats[i].isdead)
            {
                Player p = Util.GetOrAddComponent<Player>(Manager.Resource.Instantiate($"battlescene/battlesceneTestplayer", playerspawnpoints[i].transform.position));
                p._stats = Manager.Data.playerstats[i];
                p.gui = Manager.Resource.Instantiate("battlescene/CharGUI", p.transform.position, p.transform).GetComponent<CharGUI>();
                p.tag = "BattlePlayer";
                p.GetComponent<SpriteRenderer>().sprite = p._stats.sprites[7];
                if (i == Manager.Data.playerstats.Count - 1 && battletype == battleType.dungeon)
                {
                    dungeonMercenary = p;
                    Util.GetOrAddComponent<SpriteRenderer>(dungeonMercenary.gameObject).DOFade(0, 0);
                    dungeonMercenary.gameObject.SetActive(false);
                }
                else
                {
                    player.Add(p);
                    characters.Add(p);
                }
            }
        }
        enemyspawnpoint.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            playerspawnpoints[i].gameObject.SetActive(false);
        }
        //Manager.UI.SetButton(state);
        yield return Manager.Data.wfs10;
        if (battletype == battleType.dungeon) DungeonInfoDialog();
        else Manager.UI.SetDialog("전투 시작!");
        yield return Manager.Data.wfs5;
        //foreach(string arti in Manager.Artefact._artefacts.Keys)
        //{
        //    Debug.Log(arti);
        //}


        for (int i = 0; i < player.Count; i++)
        {
            if (!player[i]._stats.isdead)
            {
                StartSkill(player[i]);
            }
        }

        AddArtefactBuff(player);

        CoroutineHelper.StartCoroutine(TurnProcess());
    }

    void StartSkill(Player player)
    {
        if (player._stats.name == "Magician") // 매지션일 시 시작 기운 1 증가
        {
            Manager.Battle._data = player._stats.skills["Meditation"];
            CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(player._stats.skills["Meditation"], new List<Character> { player }, player, false));
            for (int j = 0; j < player.resourcenum / 2; j++)
            {
                BuffManager.Instance.DecreaseBuffStack("Meditation", player, -1);
            }
        }
        else if (player._stats.name == "Warrior") // 워리어 Berserk 패시브 추가
        {
            if (player._stats.level >= player._stats.skills["Berserk"].required_level)
            {
                Manager.Battle._data = player._stats.skills["Berserk"];
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(player._stats.skills["Berserk"], new List<Character> { player }, player, false));
            }
            if (player._stats.level >= player._stats.skills["BerserkII"].required_level)
            {
                Manager.Battle._data = player._stats.skills["BerserkII"];
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(player._stats.skills["BerserkII"], new List<Character> { player }, player, false));
            }
        }
        else if (player._stats.name == "Knight")
        {
            if (player._stats.level >= player._stats.skills["FocusII"].required_level)
            {
                Manager.Battle._data = player._stats.skills["FocusII"];
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(player._stats.skills["FocusII"], new List<Character> { player }, player, false));
            }
        }
        else if (player._stats.name == "Fighter")
        {
            if (player._stats.level >= player._stats.skills["Speed_InfusionII"].required_level)
            {
                Manager.Battle._data = player._stats.skills["Speed_InfusionII"];
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(player._stats.skills["Speed_InfusionII"], new List<Character> { player }, player, false));
            }
        }

    }

    void AddArtefactBuff(List<Player> players)
    {
        // 유물 버프 효과
        foreach (var artefact in Manager.Artefact.currentArtefacts.FindAll(x => x.artefacttype == artefactType.Buff))
        {
            Manager.Battle._data = Manager.Data.ArtefactSkillDict[artefact.classname];
            CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(Manager.Data.ArtefactSkillDict[artefact.classname], players.ConvertAll(new Converter < Player, Character >(x => x)), players[0], false));
        }
    }

    void SetupBG()
    {
        GameObject bg = new GameObject();
        switch (battletype)
        {
            case Define.battleType.normal:
                switch (MapManager.Instance.currentStage)
                {
                    case Define.STAGE.GRASSLAND:
                        bg = Manager.Resource.Instantiate("battlescene/bg/bg_grassland", new Vector3(0, 14, 0));
                        break;
                    case Define.STAGE.DESERT:
                        bg = Manager.Resource.Instantiate("battlescene/bg/bg_desert", new Vector3(0, 14, 0));
                        break;
                    case Define.STAGE.SNOWLAND:
                        bg = Manager.Resource.Instantiate("battlescene/bg/bg_snowland", new Vector3(0, 14, 0));
                        break;
                }
                break;
            case Define.battleType.dungeon:
                bg = Manager.Resource.Instantiate("battlescene/bg/bg_dungeon", new Vector3(0, 14, 0));
                break;
            case Define.battleType.town:
                bg = Manager.Resource.Instantiate("battlescene/bg/bg_townBattle", new Vector3(0, 14, 0));
                break;
        }
        fadebg = bg.transform.Find("fadebg").gameObject.GetComponent<SpriteRenderer>();
        fadebg.DOFade(1f, 0);
        if (MapManager.Instance.currentStage == STAGE.SNOWLAND) endbg = bg.transform.Find("endbg").gameObject.GetComponent<SpriteRenderer>();
        enemyspawnpoint = bg.transform.Find("enemyspawn");
        foreach (Transform sp in bg.transform.Find("playerspawn")) { playerspawnpoints.Add(sp); }
    }

    void SetDungeonMercenary()
    {
        List<string> mercenaryNotOpened = new List<string>();

        foreach(string name in Manager.Data.PlayerStatDict.Keys)
        {
            bool notopened = true;
            foreach(Stat stat in Manager.Data.playerstats)
            {
                if (stat.name.Equals(name)) notopened=false;
            }

            if (notopened) {
                mercenaryNotOpened.Add(name); 
            }
        }
        if (mercenaryNotOpened.Count == 0) return;
        string selected = mercenaryNotOpened[UnityEngine.Random.Range(0, mercenaryNotOpened.Count)];
        dungeonMercenaryStat = Manager.Data.AddPlayer(selected);
        switch (MapManager.Instance.currentStage)
        {
            case Define.STAGE.GRASSLAND:
                dungeonMercenaryStat.exp = Manager.Data.LevelDict[3].exp;
                dungeonMercenaryStat.level = 4;
                break;
            case Define.STAGE.DESERT:
                dungeonMercenaryStat.exp = Manager.Data.LevelDict[7].exp;
                dungeonMercenaryStat.level = 8;
                break;
            case Define.STAGE.SNOWLAND:
                dungeonMercenaryStat.exp = Manager.Data.LevelDict[9].exp;
                dungeonMercenaryStat.level = 10;
                break;
        }
        SetStatMercenary();
    }

    void DungeonInfoDialog()
    {
        switch (dungeonMercenaryStat.name)
        {
            case "Warrior":
                Manager.UI.SetDialog("어디선가 검이 팅기는 소리가 들린다...");
                break;
            case "Archer":
                Manager.UI.SetDialog("어디선가 활을 쏘는 소리가 들린다...");
                break;
            case "Knight":
                Manager.UI.SetDialog("어디선가 방패로 공격을 막아내는 소리가 들린다...");
                break;
            case "Magician":
                Manager.UI.SetDialog("어디선가 공격적인 마법을 사용하는 소리가 들린다...");
                break;
            case "Thief":
                Manager.UI.SetDialog("어디선가 날렵한 단검 소리가 들린다...");
                break;
            case "Fighter":
                Manager.UI.SetDialog("어디선가 주먹을 휘두르는 소리가 들린다...");
                break;
            case "Priest":
                Manager.UI.SetDialog("어디선가 신성한 마법을 사용하는 소리가 들린다...");
                break;
            default:
                break;
        }
    }

    #region SetStatMercenary
    private void SetStatMercenary()
    {
        switch (dungeonMercenaryStat.name)
        {
            case "Warrior":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        break;
                    case 8:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        break;
                    case 10:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        break;
                }
                break;
            case "Archer":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                    case 8:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        break;
                    case 10:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            case "Knight":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.currenthp++;
                        break;
                    case 8:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        break;
                    case 10:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            case "Magician":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                    case 8:
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                    case 10:
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            case "Thief":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                    case 8:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                    case 10:
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            case "Fighter":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        break;
                    case 8:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        break;
                    case 10:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            case "Priest":
                switch (dungeonMercenaryStat.level)
                {
                    case 4:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        break;
                    case 8:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        break;
                    case 10:
                        dungeonMercenaryStat.attack_power++;
                        dungeonMercenaryStat.maxhp++;
                        dungeonMercenaryStat.currenthp++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.speed++;
                        dungeonMercenaryStat.attack_power++;
                        break;
                }
                break;
            default:
                break;
        }
    }
    #endregion

    IEnumerator playerAttack(Player player, List<Character> targets)
    {
        // 관통 공격 체크
        if (_data.ispierce)
        {
            Monster monster = targets[0] as Monster;
            int index = Manager.Battle.Enemy.FindIndex(a => a == monster);
            targets.Clear();
            for (int i = index; i >= 0; i--)
            {
                targets.Add(Manager.Battle.Enemy[i]);
            }
        }

        if (_data.issplash)
        {
            Monster monster = targets[0] as Monster;
            int index = Manager.Battle.Enemy.FindIndex(a => a == monster);
            targets.Clear();

            if (index == 0)
            {
                targets.Add(Manager.Battle.Enemy[index]);
                if (Manager.Battle.Enemy.Count != 1)
                    targets.Add(Manager.Battle.Enemy[++index]);
            }
            else if (index == Manager.Battle.Enemy.Count - 1)
            {
                targets.Add(Manager.Battle.Enemy[index]);
                if (Manager.Battle.Enemy.Count != 1)
                    targets.Add(Manager.Battle.Enemy[--index]);
            }
            else
            {
                targets.Add(Manager.Battle.Enemy[index]);
                targets.Add(Manager.Battle.Enemy[index - 1]);
                targets.Add(Manager.Battle.Enemy[index + 1]);
            }
        }

        state = Define.BattleState.IDLE;
        SetCharAlpha(targets);
        Manager.UI.SetDialog("");
        CoroutineHelper.StartCoroutine(player.attack(targets, _data, _data.rates));
        yield return Manager.Data.wfs30;


        SetCharAlpha(targets, 1f);
        if (_data.type != actionType.Combined) CoroutineHelper.StartCoroutine(TurnProcess());
    }

    IEnumerator enemyAttack(Monster monster)
    {
        yield return Manager.Data.wfs20;
        Manager.UI.SetDialog("");
        Player target = monster.selectTarget(player);
        List<Player> targets = new List<Player>();
        targets.Add(target);

        if (monster._stats.hasSplash)
        {
            int index = Manager.Battle.Player.FindIndex(a => a == target);

            if (index == 0)
            {
                if (Manager.Battle.Player.Count != 1)
                    targets.Add(Manager.Battle.Player[++index]);
            }
            else if (index == Manager.Battle.Player.Count - 1)
            {
                if (Manager.Battle.Player.Count != 1)
                    targets.Add(Manager.Battle.Player[--index]);
            }
            else
            {
                targets.Add(Manager.Battle.Player[index - 1]);
                targets.Add(Manager.Battle.Player[index + 1]);
            }
        }

        if (target == null)
        {
            Manager.UI.SetDialog($"{monster.name}(은)는 대상을 찾지 못했다!");
            monster.onTurnEnd();
            CoroutineHelper.StartCoroutine(TurnProcess());
            yield break;
        }

        SetCharAlpha(targets.ConvertAll(new Converter<Player, Character>(x => x)));
        CoroutineHelper.StartCoroutine(monster.attack(targets));
        yield return Manager.Data.wfs30;
        SetCharAlpha(targets.ConvertAll(new Converter<Player, Character>(x => x)), 1f);
        CoroutineHelper.StartCoroutine(TurnProcess());
    }

    public void OnTarget()
    {
        if (state != Define.BattleState.PLAYERTURN) return;
        else
        {
            if (_data.target == Define.Target.AllEnemy) /*&& state != Define.BattleState.IDLE)*/ // <- 전체공격 연속 사용 막음
            {
                AllEnemyTarget();
            }
            else if (_data.target == Define.Target.Self)
            {
                Self();
            }
            else if (_data.target == Define.Target.AllPlayer)
            {
                AllPlayerTarget();
            }
            else
            {
                Manager.UI.SetDialog("대상을 선택하세요.");
                state = Define.BattleState.SELECTINGTARGET;
            }
            Manager.UI.SetButton(state);
        }
    }

    void PlayerTurn(Player player)
    {
        state = Define.BattleState.PLAYERTURN;
        Manager.UI.SetButton(state);
    }

    void EnemyTurn(Monster monster)
    {
        state = Define.BattleState.ENEMYTURN;
        CoroutineHelper.StartCoroutine(enemyAttack(monster));
    }

    public IEnumerator TurnProcess(float delay = 0.05f, bool isfirstroop = true) // 턴 진행
    {   
        state = Define.BattleState.TURNPROCESSING;

        if(isfirstroop) yield return Manager.Data.wfs20;

        if (enemy.Count == 0)
        {
            if (player.Count == 0)
            {
                CoroutineHelper.StartCoroutine(Defeat());
                yield break;
            }
            else
            {
                CoroutineHelper.StartCoroutine(Win());
                yield break;
            }
        }
        else if (player.Count == 0)
        {
            CoroutineHelper.StartCoroutine(Defeat());
            yield break;
        }

        Manager.UI.SetDialog("");

        foreach (Character c in characters)
        {
            if (c.turngauge >= 100 && !nowTurnChars.Contains(c) && !c._stats.isdead)
            {
                nowTurnChars.Add(c);
            }
        }

        nowTurnChars.Sort((x, y) =>
        {
            int ret = y.turngauge.CompareTo(x.turngauge);
            return ret != 0 ? ret : y._buffedstats.speed.CompareTo(x._buffedstats.speed);
        });

        if (nowTurnChars.Count != 0)
        {
            nowTurnChar = nowTurnChars[0];
            nowTurnChars.Remove(nowTurnChar);
            turnCount++;
            turnStartEffect = true;
            changeTurn();
            yield break;
        }

        foreach (Character c in characters)
        {
            if (!c._stats.isdead)
            {
                c.turngauge = Mathf.Min(c.turngauge + turnspeed * 1.5f * (float)Math.Round(Math.Sqrt(c._buffedstats.speed), 2), 100f);
            }
        }
        yield return new WaitForSeconds(delay);
        CoroutineHelper.StartCoroutine(TurnProcess(delay,false));
    }

    public void changeTurn()
    {
        if (turnStartEffect)
        {
            foreach (var character in characters)
            {
                character.onTurnStart(nowTurnChar);
            }

            turnStartEffect = false;
        }

        if (nowTurnChar._stats.currenthp <= 0)
        {
            nowTurnChar.onTurnEnd();
            CoroutineHelper.StartCoroutine(TurnProcess());
            return;
        }

        Manager.UI.SetDialog($"턴 {turnCount}: {nowTurnChar._stats.name}의 턴입니다.");

        if (nowTurnChar._status[characterStatus.Inactive])
        {
            Manager.UI.SetDialog($"턴 {turnCount}: {nowTurnChar._stats.name}은(는) 현재 행동할 수 없다!");
            nowTurnChar.onTurnEnd();
            CoroutineHelper.StartCoroutine(TurnProcess());
            return;
        }
        if (nowTurnChar.CompareTag("BattleEnemy"))
        {
            EnemyTurn((Monster)nowTurnChar);
        }
        else
        {
            nowTurnPlayer = (Player)nowTurnChar;
            PlayerTurn((Player)nowTurnChar);
        }
    }

    public void EndBattle()
    {
        state = Define.BattleState.IDLE;
        foreach (Character c in characters)
        {
            foreach (Basebuff b in c.bufflist) //버프 초기화
            {
                c.gui.DelBuffGUI(b);
                Destroy(b);
            }
            c.bufflist.Clear();
            foreach (Define.characterStatus status in Enum.GetValues(typeof(Define.characterStatus))) // 상태 초기화
            {
               c._status[status] = false;
            }
        }
        enemy.Clear();
        player.Clear();
        characters.Clear();
        enemyspawnpoint = null;
        playerspawnpoints.Clear();
        turnCount = 0;
        nowTurnChars.Clear();
        nowTurnChar = null;
        wave = 0;
        maxWave = 0;
        _path.Clear();
        gold = 0;
        exp = 0;
    }

    void NextWave()
    {
        wave++;
        enemy.Clear();
        nowTurnChars.Clear();
        nowTurnChar = null;
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            if (characters[i].GetType() == typeof(Monster))
            {
                characters.RemoveAt(i);
            }
        }
        if(wave == 2)
        {
            player.Add(dungeonMercenary);
            characters.Add(dungeonMercenary);
            Manager.UI.SetDialog($"{dungeonMercenary._stats.name}(이)가 고마움을 표하며 전투에 합류했다!");
            dungeonMercenary.gameObject.SetActive(true);
            dungeonMercenary.gameObject.GetComponent<SpriteRenderer>().DOFade(1, 1f);
            StartSkill(dungeonMercenary);
        }
        SetUpPath(GenerateRandomMonsters(wave));
        SetUpMonster();
        CoroutineHelper.StartCoroutine(TurnProcess());
    }

    public IEnumerator Defeat()
    {
        state = Define.BattleState.LOST;
        Manager.Sound.Play("BGM/Lose", Define.Sound.Bgm);
        //Manager.Resource.Instantiate("battlescene/bg/bg_Defeat", new Vector3(0, 14, 1));
        Manager.UI.SetDialog("전투에서 패배했다..");
        Manager.UI.SetButton(state);
        yield return Manager.Data.wfs30;
        EndBattle();
        Manager.Data.ClearGame();
        Manager.Resource.Instantiate("battlescene/bg/bg_Defeat", new Vector3(0, 14, 1));

        yield return Manager.Data.wfs30;
        //Destroy(MapManager.Instance);
        Manager.Scene.LoadScene(Define.Scenes.StartScene);
    }

    public IEnumerator Win()
    {
        state = Define.BattleState.WON;
        Manager.UI.SetButton(state);
        yield return Manager.Data.wfs5;
        if (battletype == battleType.dungeon && wave != maxWave)
        {
            Manager.UI.SetDialog("적이 더 몰려온다..");
            yield return Manager.Data.wfs10;
            NextWave();
        }
        else
        {
            foreach (var c in player)
            {
                if (!c._stats.isdead && c._stats.currenthp <= 0)
                {
                    c._stats.currenthp = 1;
                }
            }

            Manager.UI.SetDialog("전투에서 승리했다!");
            yield return Manager.Data.wfs10;
            Manager.UI.ShowPopUpUI<Reward>();
        }
    }

    public void Run()
    {
        state = Define.BattleState.IDLE;
        int count = _rand.Next(0, 100);
        if (count >= 67)
        {
            CoroutineHelper.StartCoroutine(SuccessRun());
        }
        else
        {
            CoroutineHelper.StartCoroutine(FailRun());
        }

    }
    public IEnumerator SuccessRun()
    {
        state = Define.BattleState.RUN;
        Manager.UI.SetButton(state);
        Manager.UI.SetDialog("적에게서 도망쳤다!");
        yield return Manager.Data.wfs30;
        Manager.UI.ShowPopUpUI<Reward>();
    }

    public IEnumerator FailRun()
    {
        Manager.UI.SetDialog("적에게서 도망치지 못했다...");
        Manager.UI.SetButton(state);
        foreach (Player p in player)
        {
            p.turngauge = 0;
        }
        for(int i = nowTurnChars.Count - 1; i>=0; i--)
        {
            if (nowTurnChars[i].CompareTag("BattlePlayer"))
            {
                nowTurnChars.RemoveAt(i);
            }
        }
        yield return Manager.Data.wfs10;
        CoroutineHelper.StartCoroutine(TurnProcess());
    }


    void Self()
    {
        switch (_data.type)
        {
            case Define.actionType.Buff:
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { nowTurnChar }, nowTurnChar));
                break;
            case Define.actionType.Recover:
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { nowTurnChar }, nowTurnChar));
                break;
        }
    }

    void AllPlayerTarget()
    {
        List<Character> target = new List<Character>();
        foreach (Player p in player)
        {
            if (!p._stats.isdead)
            {
                target.Add(p);
            }
        }
        switch (_data.type)
        {
            case Define.actionType.Buff:
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, target, nowTurnChar));
                break;
        }
    }

    void OnePlayerTarget()
    {
        SetCharAlpha(player.ConvertAll(p => (Character)p));
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "BattlePlayer" && !hit.collider.gameObject.GetComponent<Player>()._stats.isdead)
            {
                Player p = hit.collider.gameObject.GetComponent<Player>();
                switch (_data.type)
                {
                    case Define.actionType.Buff:
                        CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { p }, nowTurnChar));
                        break;
                    case Define.actionType.Debuff:
                        CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { p }, nowTurnChar));
                        break;
                    case Define.actionType.Recover:
                        CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { p }, nowTurnChar));
                        break;
                    case Define.actionType.Item:
                        if (_itemdata.affectedstat == Define.affectedStat.pp)
                        {
                            nowTurnPlayer = p;
                            state = Define.BattleState.SELECTINGPPTARGET;
                            Manager.UI.ShowPopUpUI<UI_Skill>();
                            Manager.UI.SetDialog("스킬을 선택하세요.");
                        }
                        else
                        {
                            p.UseItem(_itemdata);
                        }
                        break;
                }
            }
        }
    }

    void OneDeadPlayerTarget()
    {
        state = BattleState.SELECTINGDEADTARGET;
        SetCharAlpha(player.ConvertAll(p => (Character)p));
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "BattlePlayer" && hit.collider.gameObject.GetComponent<Player>()._stats.isdead)
            {
                Player p = hit.collider.gameObject.GetComponent<Player>();
                switch (_data.type)
                {
                    case Define.actionType.Revive:
                        p.Revive();
                        nowTurnChar.SetSkill(_data.name);
                        nowTurnChar.onTurnEnd();
                        Manager.UI.SetDialog($"{p._stats.name}가 부활했다!");
                        SetCharAlpha(null, 1f);
                        CoroutineHelper.StartCoroutine(TurnProcess());
                        break;
                    case Define.actionType.Item:
                        break;
                }
            }
        }
    }

    void AllEnemyTarget()
    {
        state = Define.BattleState.IDLE; // <- 전체공격 연속 사용 막음
        List<Character> target = new List<Character>();
        foreach (Monster m in enemy)
        {
            if (!m._stats.isdead)
            {
                target.Add(m);
            }
        }
        switch (_data.type)
        {
            case Define.actionType.Attack:
                CoroutineHelper.StartCoroutine(playerAttack((Player)nowTurnChar, target));
                break;
            case Define.actionType.Debuff:
                CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, target, nowTurnChar));
                break;
            case Define.actionType.Combined:
                CoroutineHelper.StartCoroutine(playerAttack((Player)nowTurnChar, target));
                break;
        }
    }

    void OneEnemyTarget()
    {
        SetCharAlpha(enemy.ConvertAll(e => (Character)e));
        if (Input.GetMouseButtonDown(0)) // 클릭한 오브젝트 받아오기
        {
            Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.tag == "BattleEnemy" && !hit.collider.gameObject.GetComponent<Monster>()._stats.isdead)
            {
                Monster m = hit.collider.gameObject.GetComponent<Monster>();
                switch (_data.type)
                {
                    case Define.actionType.Attack:
                        EnemyTargetAttack(m);
                        break;
                    case Define.actionType.Debuff:
                        CoroutineHelper.StartCoroutine(BuffManager.Instance.SetBuff(_data, new List<Character>() { m }, nowTurnChar));
                        break;
                    case Define.actionType.Item:
                        switch (_itemdata.itemtype)
                        {
                            case Define.itemType.InstantDmg:
                                Manager.UI.SetDialog($"{nowTurnChar}(이)가 {m._stats.name}에게 {_itemdata.name}사용!");
                                m.UseItem(_itemdata);
                                break;
                            case Define.itemType.Debuff:
                                break;
                        }
                        break;
                    case Define.actionType.Combined:
                        EnemyTargetAttack(m);
                        break;
                }
            }
        }

        else if (Input.GetKey(KeyCode.Escape))
        {
            changeTurn();
        }
    }

    void EnemyTargetAttack(Monster monster)
    {
        state = Define.BattleState.IDLE;
        Manager.UI.SetDialog("");
        if (_data.GetType().Name == "Skill")
        {
            CoroutineHelper.StartCoroutine(playerAttack((Player)nowTurnChar, new List<Character>() { monster }));
        }
        else if (_data.GetType().Name == "Item")
            return;
    }

    public void SetCharAlpha(List<Character> target, float alpha = 0.3f, bool deadreset = false) // target: 투명하게 하지 않을 대상, deadreset: true일 시 죽은 캐릭터들 다시 투명화
    {
        if (target == null) target = new List<Character>() { };
        if (_data == null) return;

        fadebg.DOFade(1 - alpha, 0.1f);
        foreach (Character c in characters)
        {   
            if(_data.target == Define.Target.OneDeadPlayer && c._stats.isdead && c is Player)
            {
                if (!deadreset)
                {
                    c.GetComponent<SpriteRenderer>().DOFade(0.8f, 0.1f);
                    c.gui.GetComponent<CanvasGroup>().DOFade(0.8f, 0.1f);
                } else
                {
                    c.GetComponent<SpriteRenderer>().DOFade(0f, 0.1f);
                    c.gui.GetComponent<CanvasGroup>().DOFade(0f, 0.1f);
                }
            }
            if (c != nowTurnChar && !target.Contains(c) && !c._stats.isdead && !c._status[characterStatus.Cloak])
            {
                c.GetComponent<SpriteRenderer>().DOFade(alpha, 0.1f);
                c.gui.GetComponent<CanvasGroup>().DOFade(alpha, 0.1f);
            }
        }
    }

    List<MonsterType> GenerateBossMonsters()
    {
        List<MonsterType> monsterTypes = GenerateRandomMonsters(1);
        switch (MapManager.Instance.currentStage)
        {
            case STAGE.GRASSLAND:
                    monsterTypes.Add(new MonsterType(MonsterType.GrassLand.Minotaur, 0, 0));
                    break;
            case STAGE.DESERT:
                    monsterTypes.Add(new MonsterType(0, MonsterType.Desert.Genius, 0));
                    break;
            case STAGE.SNOWLAND:
                    monsterTypes.Add(new MonsterType(0, 0, MonsterType.SnowLand.BlackMagician));
                break;
        }
        return monsterTypes;
    }

    List<MonsterType> GenerateRandomMonsters(int w)
    {
        List<MonsterType> monsterTypes = new List<MonsterType>();
        int monstercount = UnityEngine.Random.Range(4, 6);
        if (wave == 1)
        {
            monstercount = 3;
        }
        for (int i = 0; i < monstercount; i++)
        {
            switch (MapManager.Instance.currentStage)
            {
                case STAGE.GRASSLAND:
                    Define.MonsterType.GrassLand monsterType = (Define.MonsterType.GrassLand)(UnityEngine.Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.GrassLand)).Length - 1));
                    monsterTypes.Add(new Define.MonsterType(monsterType, 0, 0));
                    break;
                case STAGE.DESERT:
                    Define.MonsterType.Desert monsterTypeD = (Define.MonsterType.Desert)(UnityEngine.Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.Desert)).Length - 1));
                    monsterTypes.Add(new Define.MonsterType(0, monsterTypeD, 0));
                    break;
                case STAGE.SNOWLAND:
                    Define.MonsterType.SnowLand monsterTypeS = (Define.MonsterType.SnowLand)(UnityEngine.Random.Range(1, Enum.GetNames(typeof(Define.MonsterType.Desert)).Length - 1));
                    monsterTypes.Add(new Define.MonsterType(0, 0, monsterTypeS));
                    break;
            }
        }
        return monsterTypes;
    }
    

    public void UpdateBattle()
    {
        if (state == Define.BattleState.SELECTINGTARGET || state == BattleState.SELECTINGDEADTARGET)
        {
            Manager.UI.SetButton(state);
            switch (_data.target)
            {
                case Define.Target.OneEnemy:
                    OneEnemyTarget();
                    break;
                case Define.Target.OnePlayer:
                    OnePlayerTarget();
                    break;
                case Define.Target.OneDeadPlayer:
                    OneDeadPlayerTarget();
                    break;
            }
            Manager.UI.SetButton(state);
        }
    }

    public void EndGame()
    {
        Manager.Data.ClearGame();

        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(endbg.DOFade(1, 2)).AppendInterval(3).OnComplete( () => { Manager.Scene.LoadScene(Define.Scenes.StartScene); } );
    }
}

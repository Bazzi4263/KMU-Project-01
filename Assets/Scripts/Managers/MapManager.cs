using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using FunkyCode;
using DG.Tweening;
using UnityEngine.Rendering;
using System.Threading;
using static Define;
using UnityEngine.XR;
using FunkyCode.Rendering.Light;
using Tile = UnityEngine.Tilemaps.Tile;

public class MapManager: MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap underTilemap;
    [SerializeField] private Tilemap glowTilemap;

    [SerializeField] private Tile[] groundTiles;
    [SerializeField] private Tile[] mountainTiles;
    [SerializeField] private Tile waterTile;
    [SerializeField] private GameObject castlePrefab;
    [SerializeField] private GameObject[] forestTileObjs;
    [SerializeField] private GameObject forestTileObjectsCollection; // 숲 오브젝트들을 하나의 빈 오브젝트 자식으로 설정
    [SerializeField] private GameObject dungeonPrefab;
    [SerializeField] private GameObject unknownPrefab;
    [SerializeField] private Tile underTile;
    [SerializeField] private Tile waterUnderTile;
    [SerializeField] private Tile glowTile;

    [SerializeField] private int forestPercentage;
    [SerializeField] private int riverCount;
    [SerializeField] private int riverLenMin;
    [SerializeField] private int riverLenMax;

    [SerializeField] private int castleMinDistance;
    [SerializeField] private int castleMaxDistance;

    [SerializeField] private int dungeonMinDistance;
    [SerializeField] private int dungeonMaxDistance;

    [SerializeField] private int monsterCount;
    [SerializeField] private int easyMonsterCount;

    [SerializeField] private int dungeonCount;
    [SerializeField] private int unknownCount;

    [SerializeField] private int movementTime; // 이동할 때 흐르는 시간 양

    [SerializeField] private RenderTexture renderTexture;

    [SerializeField] private Light2D light2d; // 시간에 따라 light 설정할 수 있는 변수

    [SerializeField] private int monsterWalkableCountNight;

    public Texture2D savedMapTexture;

    public bool isPlayerTurn = true;
    public bool isLoadData = false;
    private bool isEndStage = false;
    private bool isNight = false;
    public PlayerController playerController;
    public Define.STAGE currentStage = Define.STAGE.GRASSLAND;

    private EventListner[] eventListners;

    public List<MonsterInfo> MonsterList = new List<MonsterInfo>();
    public Dictionary<Vector3Int, TileInfo> tileDic = new Dictionary<Vector3Int, TileInfo>();
    public Dictionary<Vector3Int, TileInfo> underTileDic = new Dictionary<Vector3Int, TileInfo>();
    public List<MonsterController> chaisingMonsters = new List<MonsterController>();

    private GameObject castleObj;
    private List<GameObject> dungeonObjs = new List<GameObject>();
    private List<GameObject> unknownObjs = new List<GameObject>();

    private static MapManager _instance;
    public Save save;

    // 조명 설정 저장
    private float lightSize;
    private Color lightColor;

    public static MapManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(MapManager)) as MapManager;
                if (!_instance)
                {
                    Manager.Resource.Instantiate("@MapManager", Vector3.zero);
                }
            }
            return _instance;
        }
    }

    public int EasyMonsterCount { get => easyMonsterCount; }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Manager.Artefact.GetArtefact("Archangel's Book");

        if (!isLoadData)
        {
            forestTileObjectsCollection = GameObject.Find("ForestTileObjectsCollection");
            CreateMap();

            lightSize = light2d.size;
            lightColor = light2d.color;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        if (scene.name != "MapScene")
        {
            foreach (var item in dungeonObjs)
            {
                item.SetActive(false);
            }

            foreach (var item in unknownObjs)
            {
                item.SetActive(false);
            }

            castleObj.SetActive(false);

            return;
        }

        GameObject grid = GameObject.FindObjectOfType<Grid>().gameObject;

        this.grid = grid.GetComponent<Grid>();
        tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();
        underTilemap = grid.transform.GetChild(1).GetComponent<Tilemap>();
        glowTilemap = grid.transform.GetChild(2).GetComponent<Tilemap>();
        forestTileObjectsCollection = GameObject.Find("ForestTileObjectsCollection");

        if (isEndStage)
        {
            isEndStage = false;
            currentStage += 1;

            ClearGame(); 

            foreach (var player in Manager.Data.playerstats)
            {
                player.currenthp = player.maxhp;
                player.isdead = false;
                foreach (var skill in player.skills.Values)
                {
                    skill.pp = skill.maxpp;
                }
            }

            CreateMap();
        }
        else
        {
            foreach (var item in dungeonObjs)
                item.SetActive(true);
            foreach (var item in unknownObjs)
                item.SetActive(true);
            castleObj.SetActive(true);

            RestoreMap();
        }
    }

    public void CreateMap()
    {
        Manager.Data.time = 360;

        ClearTileMap();

        switch (currentStage)
        {
            case Define.STAGE.GRASSLAND:
                groundTiles = new Tile[] { Manager.Resource.Load<Tile>("HexTile/hexPlains00"), Manager.Resource.Load<Tile>("HexTile/hexPlains01"),
                Manager.Resource.Load<Tile>("HexTile/hexPlains02"), Manager.Resource.Load<Tile>("HexTile/hexPlains03")};
                mountainTiles = new Tile[] { Manager.Resource.Load<Tile>("HexTile/hexMountain00") };
                waterTile = Manager.Resource.Load<Tile>("HexTile/hexOceanCalm00");
                for (int i = 0; i < forestTileObjs.Length; i++)
                    forestTileObjs[i].GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>($"HexTile/hexForestBroadleaf0{i}");
                castlePrefab.GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>("HexTile/hexPlainsCastle00");
                easyMonsterCount = 3;
                break;
            case Define.STAGE.DESERT:
                groundTiles = new Tile[] { Manager.Resource.Load<Tile>("DesertTiles/hexDesertYellowCactiForest00"), Manager.Resource.Load<Tile>("DesertTiles/hexDesertYellowDirtDunes01"),
                Manager.Resource.Load<Tile>("DesertTiles/hexDesertYellowDirt00"), Manager.Resource.Load<Tile>("DesertTiles/hexDesertYellowDirtDunes00")};
                mountainTiles = new Tile[] { Manager.Resource.Load<Tile>("DesertTiles/hexDesertRedMountains00"), Manager.Resource.Load<Tile>("DesertTiles/hexDesertRedMountains01"),
                Manager.Resource.Load<Tile>("DesertTiles/hexDesertRedMountains02"), Manager.Resource.Load<Tile>("DesertTiles/hexDesertRedMountains03")};
                waterTile = Manager.Resource.Load<Tile>("HexTile/hexOceanCalm00");
                for (int i = 0; i < forestTileObjs.Length; i++)
                    forestTileObjs[i].GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>($"HexTile/hexForestBroadleaf0{i}");
                castlePrefab.GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>("HexTile/hexPlainsCastle00");
                riverCount = 1;
                riverLenMin = 1;
                riverLenMax = 3;
                forestPercentage = 10;
                monsterCount = 7;
                easyMonsterCount = 4;
                dungeonCount = 1;
                break;
            case Define.STAGE.SNOWLAND:
                groundTiles = new Tile[] { Manager.Resource.Load<Tile>("SnowTiles/hexSnowField00"), Manager.Resource.Load<Tile>("SnowTiles/hexSnowField01"),
                Manager.Resource.Load<Tile>("SnowTiles/hexSnowField02"), Manager.Resource.Load<Tile>("SnowTiles/hexSnowField03")};
                mountainTiles = new Tile[] {    Manager.Resource.Load<Tile>("SnowTiles/hexMountainSnow00"), Manager.Resource.Load<Tile>("SnowTiles/hexMountainSnow01"),
                Manager.Resource.Load<Tile>("SnowTiles/hexMountainSnow02"), Manager.Resource.Load<Tile>("SnowTiles/hexMountainSnow03")};
                waterTile = Manager.Resource.Load<Tile>("SnowTiles/hexOceanIceBergs00");
                for (int i = 0; i < forestTileObjs.Length; i++)
                    forestTileObjs[i].GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>($"SnowTiles/hexForestPineSnowCovered0{i}");
                castlePrefab.GetComponent<SpriteRenderer>().sprite = Manager.Resource.Load<Sprite>("SnowTiles/hexSnowFieldIcePalace00");
                riverCount = 2;
                riverLenMin = 2;
                riverLenMax = 5;
                forestPercentage = 40;
                monsterCount = 9;
                easyMonsterCount = 5;
                dungeonCount = 1;
                break;
            case Define.STAGE.TOWN:
                break;
            default:
                break;
        }

        for (int i = 0; i < castleMaxDistance + 10; i++)
        {
            for (int j = 0; j < castleMaxDistance + 10; j++)
            {
                if (i < castleMaxDistance && j < castleMaxDistance)
                {
                    if (i % 2 != 1 || j != castleMaxDistance - 1)
                    {
                        continue;
                    }
                }

                if ((i == castleMaxDistance && j <= castleMaxDistance) || (j == castleMaxDistance && i <= castleMaxDistance))
                {
                    if (i % 2 == 1 && j == castleMaxDistance)
                    {
                        CreateBackGround(new Vector3Int(j, i));
                        CreateBackGround(new Vector3Int(j, -i));
                        CreateBackGround(new Vector3Int(-j, -i), true);
                        CreateBackGround(new Vector3Int(-j, i), true);
                    }
                    else
                    {
                        CreateBackGround(new Vector3Int(j, i), true);
                        CreateBackGround(new Vector3Int(-j, i), true);
                        CreateBackGround(new Vector3Int(j, -i), true);
                        CreateBackGround(new Vector3Int(-j, -i), true);
                    }
                }
                else
                {
                    if (i % 2 == 1 && j == castleMaxDistance - 1 && i <= castleMaxDistance)
                    {
                        CreateBackGround(new Vector3Int(j, i), true);
                        CreateBackGround(new Vector3Int(j, -i), true);
                    }
                    else
                    {
                        CreateBackGround(new Vector3Int(j, i));
                        CreateBackGround(new Vector3Int(-j, i));
                        CreateBackGround(new Vector3Int(j, -i));
                        CreateBackGround(new Vector3Int(-j, -i));
                    }
                }
            }
        }

        for (int i = 0; i < castleMaxDistance; i++)
        {
            for (int j = 0; j < castleMaxDistance; j++)
            {
                if (i % 2 == 1 && j == castleMaxDistance - 1)
                {
                    CreateGround(new Vector3Int(-j, i));
                    CreateGround(new Vector3Int(-j, -i));
                    continue;
                }

                CreateGround(new Vector3Int(j, i));
                CreateGround(new Vector3Int(-j, i));
                CreateGround(new Vector3Int(j, -i));
                CreateGround(new Vector3Int(-j, -i));
            }
        }

        for (int i = 0; i < riverCount; i++)
            CreateRiver();
        CreateCastle();
        //CreateUnderTile();
        CreateDungeon();
        CreateUnknown();

        foreach (TileInfo tile in tileDic.Values)
        {
            tile.UpdateNeighbourTiles();
        }

        CreatePlayer();
        CreateMonster();

        eventListners = FindObjectsOfType<EventListner>();
        ClearEventListnerMaterial();
        //Manager.Data.MapSceneInitSave();
        //Manager.Data.SaveGameData();
        //UpdateEventListener();
    }

    public void CreatePlayer(Vector3 pos = default(Vector3))
    {
        if (playerController == null)
        {
            GameObject player = Manager.Resource.Instantiate("Player");
            playerController = Util.GetOrAddComponent<PlayerController>(player);
            player.GetComponent<SpriteRenderer>().sprite = Manager.Data.playerstats[0].sprites[1];
        }
        else
        {
            playerController.gameObject.transform.position = pos;
            playerController.UpdateCurrentTile();
        }
    }

    private void RestoreMap()
    {
        foreach (TileInfo tile in tileDic.Values)
        {
            switch (tile.tiledata.type)
            {
                case Define.TILE.GROUND:
                    tilemap.SetTile(tile.tiledata.pos, groundTiles[tile.tiledata.tileNum]);
                    break;
                case Define.TILE.WATER:
                    underTilemap.SetTile(tile.tiledata.pos, waterTile);
                    break;
                case Define.TILE.CASTLE:
                    tilemap.SetTile(tile.tiledata.pos, groundTiles[tile.tiledata.tileNum]);
                    break;
                case Define.TILE.FOREST:
                    GameObject forestObj = Instantiate(forestTileObjs[tile.tiledata.tileNum], tile.tiledata.worldPos, Quaternion.identity);
                    forestObj.transform.parent = forestTileObjectsCollection.transform;
                    break;
                case Define.TILE.Dungeon:
                    tilemap.SetTile(tile.tiledata.pos, groundTiles[tile.tiledata.tileNum]);
                    break;
                case TILE.Unknown:
                    tilemap.SetTile(tile.tiledata.pos, groundTiles[tile.tiledata.tileNum]);
                    break;
                case TILE.MOUNTAIN:
                    tilemap.SetTile(tile.tiledata.pos, mountainTiles[tile.tiledata.tileNum]);
                    break;
                default:
                    break;
            }
        }

        foreach (TileInfo tile in underTileDic.Values)
        {
            switch (tile.tiledata.type)
            {
                case Define.TILE.UNDERGROUND:
                    underTilemap.SetTile(tile.tiledata.pos, underTile);
                    break;
                case Define.TILE.UNDERWATER:
                    underTilemap.SetTile(tile.tiledata.pos, waterUnderTile);
                    break;
            }
        }

        if (isLoadData)
        {
            // 플레이어 로드
            CreatePlayer(save.characterPos);

            // 몬스터 로드
            int startNum = 0;

            for (int i = 0; i < save.monsterCount.Count; i++)
            {
                List<Define.MonsterType> monsterType = new List<Define.MonsterType>();

                for (int j = 0; j < save.monsterCount[i]; j++)
                {                
                    monsterType.Add(save.monsterType[startNum++]);
                }

                MonsterInfo monster = Manager.Resource.Instantiate("Monsters/Monster(MapScene)", save.monsterpos[i]).GetComponent<MonsterInfo>();
                monster.monsterTypes = monsterType;
                monster.RestoreMonster();
            }

            isLoadData = false;
        }
    }

    private void ClearTileMap()
    {
        for (int i = 0; i < tileDic.Count; i++)
        {
            DestroyTile((tileDic.Keys.ToList()[i]));
        }

        for (int i = 0; i < forestTileObjectsCollection.transform.childCount; i++)
        {
            Destroy(forestTileObjectsCollection.transform.GetChild(i).gameObject);
        }

        foreach (GameObject monster in GameObject.FindGameObjectsWithTag("BattleEnemy"))
        {
            Destroy(monster);
        }
    }

    private void CreateBackGround(Vector3Int vector3Int, bool isMountain = false)
    {
        DestroyTile(vector3Int);

        int rand;

        if (isMountain)
        {
            rand = Random.Range(0, mountainTiles.Length);

            tilemap.SetTile(vector3Int, mountainTiles[rand]);

            if (tileDic.ContainsKey(vector3Int))
                tileDic[vector3Int] = new TileInfo(vector3Int, grid, Define.TILE.MOUNTAIN, null, false);
            else
                tileDic.Add(vector3Int, new TileInfo(vector3Int, grid, Define.TILE.MOUNTAIN, null, false));
        }
        else
        {
            rand = Random.Range(0, 4);

            if (Random.Range(0, 100) < forestPercentage)
            {
                tilemap.SetTile(vector3Int, null);
                GameObject forestObj = Instantiate(forestTileObjs[rand], grid.CellToWorld(vector3Int), Quaternion.identity);
                forestObj.transform.parent = forestTileObjectsCollection.transform;

                if (tileDic.ContainsKey(vector3Int))
                    tileDic[vector3Int] = new TileInfo(vector3Int, grid, Define.TILE.FOREST, forestObj, false);
                else
                    tileDic.Add(vector3Int, new TileInfo(vector3Int, grid, Define.TILE.FOREST, forestObj, false));
            }
            else
            {
                tilemap.SetTile(vector3Int, groundTiles[rand]);
                if (tileDic.ContainsKey(vector3Int))
                    tileDic[vector3Int] = new TileInfo(vector3Int, grid, Define.TILE.GROUND, null, false);
                else
                    tileDic.Add(vector3Int, new TileInfo(vector3Int, grid, Define.TILE.GROUND, null, false));
            }
        }

        tileDic[vector3Int].tiledata.tileNum = rand;
    }

    private void CreateGround(Vector3Int vector3Int)
    {
        int rand = Random.Range(0, 4);

        DestroyTile(vector3Int);

        if (Random.Range(0, 100) < forestPercentage)
        {
            tilemap.SetTile(vector3Int, null);
            GameObject forestObj = Instantiate(forestTileObjs[rand], grid.CellToWorld(vector3Int), Quaternion.identity);
            forestObj.transform.parent = forestTileObjectsCollection.transform;

            if (tileDic.ContainsKey(vector3Int))
                tileDic[vector3Int] = new TileInfo(vector3Int, grid, Define.TILE.FOREST, forestObj);
            else
                tileDic.Add(vector3Int, new TileInfo(vector3Int, grid, Define.TILE.FOREST, forestObj));
        }
        else
        {
            DestroyTile(vector3Int);
            tilemap.SetTile(vector3Int, groundTiles[rand]);
            if (tileDic.ContainsKey(vector3Int))
                tileDic[vector3Int] = new TileInfo(vector3Int, grid, Define.TILE.GROUND);
            else
                tileDic.Add(vector3Int, new TileInfo(vector3Int, grid, Define.TILE.GROUND));
        }

        tileDic[vector3Int].tiledata.tileNum = rand;
    }

    private void CreateRiver()
    {
        int riverLen = Random.Range(riverLenMin, riverLenMax + 1);
        Vector3Int pos = new Vector3Int(Random.Range(2, castleMaxDistance - 1) * (Random.Range(0, 2) == 0 ? -1 : 1),
                               Random.Range(2, castleMaxDistance - 1) * (Random.Range(0, 2) == 0 ? -1 : 1));

        int count = 0;

        for (int i = 0; i < riverLen; i++)
        {
            while (count < 30)
            {
                Vector3Int nextPos = pos;

                if (Random.Range(0, 2) == 0) // 위로 이동
                {
                    nextPos.y += 1;
                    if (Random.Range(0, 2) == 0) // 오른쪽으로 이동
                    {
                        if (nextPos.y % 2 == 0) nextPos.x += 1;
                        else nextPos.x += 0;
                    }
                    else // 왼쪽으로 이동
                    {
                        if (nextPos.y % 2 == 0) nextPos.x -= 0;
                        else nextPos.x -= 1;
                    }
                }
                else // 아래로 이동
                {
                    nextPos.y -= 1;
                    if (Random.Range(0, 2) == 0) // 오른쪽으로 이동
                    {
                        if (nextPos.y % 2 == 0) nextPos.x += 1;
                        else nextPos.x += 0;
                    }
                    else // 왼쪽으로 이동
                    {
                        if (nextPos.y % 2 == 0) nextPos.x -= 0;
                        else nextPos.x -= 1;
                    }
                }


                if (nextPos == new Vector3Int(0,0))
                {
                    ++count;
                    continue;
                }
                if (!tileDic.ContainsKey(nextPos))
                {
                    ++count;
                    continue;
                }
                if (tileDic[nextPos].tiledata.type == Define.TILE.WATER)
                {
                    ++count;
                    continue;
                }

                pos = nextPos;
                count = 0;
                break;
            }

            DestroyTile(pos);
            underTilemap.SetTile(pos, waterTile);
            tileDic[pos] = null;
            tileDic[pos] = new TileInfo(pos, grid, Define.TILE.WATER);
        }
    }

    private void CreateCastle()
    {
        Vector3Int pos = new Vector3Int(Random.Range(castleMinDistance, castleMaxDistance) * (Random.Range(0, 2) == 0 ? -1 : 1),
                               Random.Range(castleMinDistance, castleMaxDistance - 1) * (Random.Range(0, 2) == 0 ? -1 : 1));

        if (tileDic.ContainsKey(pos))
        {
            DestroyTile(pos);
            tilemap.SetTile(pos, groundTiles[0]);
            castleObj = Instantiate(castlePrefab, grid.CellToWorld(pos), Quaternion.identity);
            DontDestroyOnLoad(castleObj);
            tileDic[pos] = null;
            tileDic[pos] = new TileInfo(pos, grid, Define.TILE.CASTLE, castleObj);
        }
        else
        {
            CreateCastle();
        }
    }

    private void CreateUnderTile()
    {
        for (int i = -castleMaxDistance ; i < castleMaxDistance - 1; i++)
        {
            if (Mathf.Abs(i) % 2 == 1)
            {
                if (tileDic[new Vector3Int(castleMaxDistance - 1, i + 1)].tiledata.type == Define.TILE.WATER)
                {
                    underTilemap.SetTile(new Vector3Int(castleMaxDistance - 2, i), waterUnderTile);

                    if (underTileDic.ContainsKey(new Vector3Int(castleMaxDistance - 2, i)))
                        underTileDic[new Vector3Int(castleMaxDistance - 2, i)] = new TileInfo(new Vector3Int(castleMaxDistance - 2, i), grid, Define.TILE.UNDERWATER);
                    else
                        underTileDic.Add(new Vector3Int(castleMaxDistance - 2, i), new TileInfo(new Vector3Int(castleMaxDistance - 2, i), grid, Define.TILE.UNDERWATER));
                }
                else
                {
                    underTilemap.SetTile(new Vector3Int(castleMaxDistance - 2, i), underTile);

                    if (underTileDic.ContainsKey(new Vector3Int(castleMaxDistance - 2, i)))
                        underTileDic[new Vector3Int(castleMaxDistance - 2, i)] = new TileInfo(new Vector3Int(castleMaxDistance - 2, i), grid, Define.TILE.UNDERGROUND);
                    else
                        underTileDic.Add(new Vector3Int(castleMaxDistance - 2, i), new TileInfo(new Vector3Int(castleMaxDistance - 2, i), grid, Define.TILE.UNDERGROUND));
                }

                if (tileDic[new Vector3Int(-castleMaxDistance + 1, i + 1)].tiledata.type == Define.TILE.WATER)
                {
                    underTilemap.SetTile(new Vector3Int(-castleMaxDistance, i), waterUnderTile);

                    if (underTileDic.ContainsKey(new Vector3Int(-castleMaxDistance, i)))
                        underTileDic[new Vector3Int(-castleMaxDistance, i)] = new TileInfo(new Vector3Int(-castleMaxDistance, i), grid, Define.TILE.UNDERWATER);
                    else
                        underTileDic.Add(new Vector3Int(-castleMaxDistance, i), new TileInfo(new Vector3Int(-castleMaxDistance, i), grid, Define.TILE.UNDERWATER));

                }
                else
                {
                    underTilemap.SetTile(new Vector3Int(-castleMaxDistance, i), underTile);

                    if (underTileDic.ContainsKey(new Vector3Int(-castleMaxDistance, i)))
                        underTileDic[new Vector3Int(-castleMaxDistance, i)] = new TileInfo(new Vector3Int(-castleMaxDistance, i), grid, Define.TILE.UNDERGROUND);
                    else
                        underTileDic.Add(new Vector3Int(-castleMaxDistance, i), new TileInfo(new Vector3Int(-castleMaxDistance, i), grid, Define.TILE.UNDERGROUND));
                }
            }
        }

        for (int i = -castleMaxDistance; i < castleMaxDistance - 1; i++)
        {
            if (castleMaxDistance % 2 == 0)
            {
                if (!tileDic.ContainsKey(new Vector3Int(i + 1, -castleMaxDistance + 1)))
                {
                    continue;
                }

                if (tileDic[new Vector3Int(i + 1, -castleMaxDistance + 1)].tiledata.type == Define.TILE.WATER)
                {
                    underTilemap.SetTile(new Vector3Int(i + 1, -castleMaxDistance), waterUnderTile);

                    if (underTileDic.ContainsKey(new Vector3Int(i + 1, -castleMaxDistance)))
                        underTileDic[new Vector3Int(i + 1, -castleMaxDistance)] = new TileInfo(new Vector3Int(i + 1, -castleMaxDistance), grid, Define.TILE.UNDERWATER);
                    else
                        underTileDic.Add(new Vector3Int(i + 1, -castleMaxDistance), new TileInfo(new Vector3Int(i + 1, -castleMaxDistance), grid, Define.TILE.UNDERWATER));
                }
                else
                {
                    underTilemap.SetTile(new Vector3Int(i + 1, -castleMaxDistance), underTile);

                    if (underTileDic.ContainsKey(new Vector3Int(i + 1, -castleMaxDistance)))
                        underTileDic[new Vector3Int(i + 1, -castleMaxDistance)] = new TileInfo(new Vector3Int(i + 1, -castleMaxDistance), grid, Define.TILE.UNDERGROUND);
                    else
                        underTileDic.Add(new Vector3Int(i + 1, -castleMaxDistance), new TileInfo(new Vector3Int(i + 1, -castleMaxDistance), grid, Define.TILE.UNDERGROUND));
                }
            }
            else
            {
                if (!tileDic.ContainsKey(new Vector3Int(i, -castleMaxDistance + 1)))
                {
                    continue;
                }

                if (tileDic[new Vector3Int(i, -castleMaxDistance + 1)].tiledata.type == Define.TILE.WATER)
                {
                    underTilemap.SetTile(new Vector3Int(i - 1, -castleMaxDistance), waterUnderTile);

                    if (underTileDic.ContainsKey(new Vector3Int(i - 1, -castleMaxDistance)))
                        underTileDic[new Vector3Int(i - 1, -castleMaxDistance)] = new TileInfo(new Vector3Int(i - 1, -castleMaxDistance), grid, Define.TILE.UNDERWATER);
                    else
                        underTileDic.Add(new Vector3Int(i - 1, -castleMaxDistance), new TileInfo(new Vector3Int(i - 1, -castleMaxDistance), grid, Define.TILE.UNDERWATER));
                }
                else
                {
                    underTilemap.SetTile(new Vector3Int(i - 1, -castleMaxDistance), underTile);

                    if (underTileDic.ContainsKey(new Vector3Int(i - 1, -castleMaxDistance)))
                        underTileDic[new Vector3Int(i - 1, -castleMaxDistance)] = new TileInfo(new Vector3Int(i - 1, -castleMaxDistance), grid, Define.TILE.UNDERGROUND);
                    else
                        underTileDic.Add(new Vector3Int(i - 1, -castleMaxDistance), new TileInfo(new Vector3Int(i - 1, -castleMaxDistance), grid, Define.TILE.UNDERGROUND));
                }
            }

        }

    }

    private void CreateDungeon()
    {
        for (int i = 0; i < dungeonCount; i++)
        {
            while (true)
            {
                Vector3Int pos = new Vector3Int(Random.Range(dungeonMinDistance, dungeonMaxDistance) * (Random.Range(0, 2) == 0 ? -1 : 1),
                               Random.Range(dungeonMinDistance, dungeonMaxDistance - 1) * (Random.Range(0, 2) == 0 ? -1 : 1));

                if (tileDic.ContainsKey(pos))
                {
                    if (tileDic[pos].tiledata.type == Define.TILE.GROUND && tileDic[pos].tiledata.objectOnTile == null && tileDic[pos].tiledata.worldPos != Vector3.zero && tileDic[pos].tiledata.Walkable)
                    {
                        GameObject dungeon = Instantiate(dungeonPrefab, tileDic[pos].tiledata.worldPos, Quaternion.identity);
                        dungeonObjs.Add(dungeon);
                        DontDestroyOnLoad(dungeon);

                        tileDic[pos] = null;
                        tileDic[pos] = new TileInfo(pos, grid, Define.TILE.Dungeon, dungeon);
                        break;
                    }
                }       
            }
        }
    }

    private void CreateUnknown()
    {
        for (int i = 0; i < unknownCount; i++)
        {
            while (true)
            {
                TileInfo tile = tileDic.ElementAt(Random.Range(0, tileDic.Count)).Value;

                if (tile.tiledata.type == Define.TILE.GROUND && tile.tiledata.objectOnTile == null && tile.tiledata.worldPos != Vector3.zero && tile.tiledata.Walkable)
                {
                    GameObject unknown = Instantiate(unknownPrefab, tile.tiledata.worldPos, Quaternion.identity);
                    DontDestroyOnLoad(unknown);
                    unknownObjs.Add(unknown);

                    Vector3Int pos = tile.tiledata.pos;

                    tileDic[pos] = new TileInfo(pos, grid, Define.TILE.Unknown, unknown);                   
                    break;
                }
            }
        }
    }

    private void CreateMonster()
    {
        for (int i = 0; i < monsterCount; i++)
        {
            while (true)
            {
                TileInfo tile = tileDic.ElementAt(Random.Range(0, tileDic.Count)).Value;

                if (tile.tiledata.objectOnTile == null && tile.tiledata.Walkable && tile.tiledata.worldPos != Vector3.zero && tile.tiledata.obj == null && (tile.tiledata.type == TILE.GROUND || tile.tiledata.type == TILE.FOREST))
                {
                    bool isSame = false;

                    foreach (TileInfo item in tileDic[playerController.GetPlayerPos()].neighboursTiles)
                    {
                        if (tile == item)
                        {
                            isSame = true;
                        }
                    }

                    if (!isSame)
                    {
                        GameObject monster = Manager.Resource.Instantiate("Monsters/Monster(MapScene)", tile.tiledata.worldPos);
                        MonsterList.Add(Util.GetOrAddComponent<MonsterInfo>(monster));
                        break;
                    }
                }
            }
        }
    }

    public void GlowTile(List<Vector3Int> poses, bool enable)
    {
        if (enable)
            foreach (Vector3Int pos in poses)
                glowTilemap.SetTile(pos, glowTile);
        else
            foreach (Vector3Int pos in poses)
                glowTilemap.SetTile(pos, null);
    }

    public void StartBattleScene(List<Define.MonsterType> monsterTypes)
    {
        SaveMapImage();

        Manager.Battle.SetUpPath(monsterTypes);
        Manager.Battle.battletype = Define.battleType.normal;
        Manager.Scene.LoadScene(Define.Scenes.BattleScene);
    }

    private void SaveMapImage()
    {
        RenderTexture.active = renderTexture;
        var texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        savedMapTexture = texture2D;
        savedMapTexture.Apply();
    }

    public void StartTownScene()
    {
        isEndStage = true;
        Manager.Battle.battletype = Define.battleType.town;
        Manager.Scene.LoadScene(Define.Scenes.BattleScene);
    }

    public void StartDungeonScene()
    {
        SaveMapImage();

        Manager.Battle.battletype = Define.battleType.dungeon;
        Manager.Scene.LoadScene(Define.Scenes.BattleScene);
    }

    public void UnknownEvent()
    {
        Manager.Unknown.Event();
        unknownObjs.Remove(playerController.currentTile.tiledata.obj);
        Destroy(playerController.currentTile.tiledata.obj);
    }

    public void UpdateEventListener()
    {
        if (eventListners == null)
        {
            eventListners = FindObjectsOfType<EventListner>();
        }

        foreach (EventListner eventListner in eventListners)
        {
            eventListner.UpdateListner();
        }
    }

    public void ClearEventListnerMaterial()
    {
        if (eventListners == null)
        {
            eventListners = FindObjectsOfType<EventListner>();
        }

        foreach (EventListner eventListner in eventListners)
        {
            eventListner.ClearMateial();
        }
    }

    private void DestroyTile(Vector3Int _pos)
    {
        if (!tileDic.ContainsKey(_pos))
        {
            return;
        }

        switch (tileDic[_pos].tiledata.type)
        {
            case Define.TILE.GROUND:
                tilemap.SetTile(_pos, null);
                break;
            case Define.TILE.WATER:
                tilemap.SetTile(_pos, null);
                break;
            case Define.TILE.CASTLE:
                Destroy(tileDic[_pos].tiledata.obj);
                break;
            case Define.TILE.FOREST:
                Destroy(tileDic[_pos].tiledata.obj);
                break;
            case Define.TILE.Dungeon:
                Destroy(tileDic[_pos].tiledata.obj);
                break;
            default:
                break;
        }
    }

    public IEnumerator ChasePlayerCoroutine()
    {
        List<MonsterController> temp = chaisingMonsters.ToList();

        foreach (MonsterController monster in temp)
        {
            if (monster.isBattled && temp.LastOrDefault().Equals(monster))
            {
                isPlayerTurn = true;
                continue;
            }
            else if (monster.isBattled)
            {
                continue;
            }

            if (isNight)
            {
                yield return Manager.Data.wfs5;
            }
            else
            {
                yield return Manager.Data.wfs5;
            }

            monster.ChasePlayer(playerController.GetPlayerPos());

            if (temp.LastOrDefault().Equals(monster))
            {
                isPlayerTurn = true;
            }
        }
    }

    public void AddTime()
    {
        Manager.Data.time += movementTime;
    }

    public void NightEvent()
    {
        if (!light2d)
        {
            light2d = FindObjectOfType<Light2D>();
        }

        if(Manager.Data.GetTime()[0] >= 17 && Manager.Data.GetTime()[0] < 20)
        {
            if (light2d.color == new Color32(255, 55, 0, 255))
            {
                return;
            }
            else
            {
                DOTween.To(() => light2d.color, x => light2d.color = x, new Color32(255, 55, 0, 255), 1);
            }
        }
        else if (Manager.Data.GetTime()[0] >= 20)
        {
            isNight = true;

            chaisingMonsters = FindObjectsOfType<MonsterController>().ToList();
            chaisingMonsters = chaisingMonsters.Distinct().ToList();

            foreach (MonsterController monster in chaisingMonsters)
            {
                monster.WalkableCost = 30;
                monster.WalkableCount = monsterWalkableCountNight;
            }

            if (light2d.color == new Color32(100, 100, 100, 255))
            {
                return;
            }
            else
            {
                DOTween.To(() => light2d.color, x => light2d.color = x, new Color32(100, 100, 100, 255), 1);
                DOTween.To(() => light2d.size, x => light2d.size = x, 1, 1);
            }
        }
    }

    public void ClearGame(bool isReset = false)
    {
        foreach (var item in MonsterList)
        {
            Destroy(item.gameObject);
        }
        MonsterList.Clear();

        foreach (var item in dungeonObjs)
        {
            Destroy(item.gameObject);
        }
        dungeonObjs.Clear();

        foreach (var item in unknownObjs)
        {
            Destroy(item.gameObject);
        }
        unknownObjs.Clear();

        Destroy(castleObj);

        if (isReset)
        {
            Destroy(playerController.gameObject);
            tileDic.Clear();
            isEndStage = false;
        }

        light2d.color = lightColor;
        light2d.size = lightSize;
    }
}

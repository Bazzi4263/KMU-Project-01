using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int walkableCost;
    [SerializeField] private float movementTime;
    private Grid grid;
    public TileInfo currentTile;
    private TileInfo targetTile;
    private List<Vector3Int> walkablePoses;
    Animator _anim;
    AnimatorOverrideController _animatorOverrideController;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private BFSResult movementRanage = new BFSResult();
    private bool isSelected;
    private bool isDetected;
    string movedir;

    private void Awake()
    {
        grid = FindObjectOfType(typeof(Grid)) as Grid;
        UpdateCurrentTile();
        DontDestroyOnLoad(this.gameObject);
        SetAnim();
        /*
         * 해당 캐릭터의 animation clip을 만듭니다
         * 애니메이터 컨트롤러를 만들 때 사용하면 편해요
         */
        //MakeAnimClip();
    }

    private void Start()
    {
        Camera.main.transform.parent = this.transform;
        Camera.main.transform.localPosition = new Vector3(0,0,-10f);
        Camera.main.transform.SetAsFirstSibling();
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateCurrentTile();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MapScene")
        {
            StopAllCoroutines();
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
            isSelected = false;

            foreach (var camera in Camera.allCameras)
            {
                if (camera.transform != this.transform.GetChild(0))
                {
                    Destroy(camera.gameObject);
                }
            }

            grid = FindObjectOfType(typeof(Grid)) as Grid;
            MapManager.Instance.isPlayerTurn = true;
            isDetected = false;
            CheckMonster();
            CheckPlayerTurn();
            StartCoroutine(MapManager.Instance.ChasePlayerCoroutine());
        }
    }

    private void CheckPlayerTurn()
    {
        MapManager.Instance.isPlayerTurn = !isDetected;
    }

    private void Update()
    {
        if (!MapManager.Instance.isPlayerTurn)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && !Manager.UI.Issetting)
        {
            Vector3Int pos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pos.z = 0;

            if (pos == currentTile.tiledata.pos)
            {
                if (isSelected)
                {
                    MapManager.Instance.GlowTile(walkablePoses, false);
                    isSelected = false;
                }
                else
                {
                    movementRanage = PathFinding.BFSGetRanage(pos, walkableCost);
                    walkablePoses = new List<Vector3Int>(movementRanage.GetRangePositions());
                    MapManager.Instance.GlowTile(walkablePoses, true);
                    isSelected = true;
                }
            }
            else if (isSelected)
            {
                foreach (Vector3Int _pos in walkablePoses)
                {
                    if (pos == _pos)
                    {
                        targetTile = MapManager.Instance.tileDic[pos];
                        MoveThroughPath(movementRanage.GetPathTo(pos).Select(x => grid.CellToWorld(x)).ToList());
                        MapManager.Instance.GlowTile(walkablePoses, false);
                        isSelected = false;
                    }
                }
            }
        }
    }

    public void UpdateCurrentTile()
    {
        if (currentTile != null)
        {
            if (currentTile.tiledata.objectOnTile == this.gameObject)
            {
                currentTile.tiledata.objectOnTile = null;
            }
        }

        if (grid == null)
        {
            grid = FindObjectOfType(typeof(Grid)) as Grid;
        }

        currentTile = MapManager.Instance.tileDic[grid.WorldToCell(transform.position)];

        if (currentTile.tiledata.objectOnTile != null)
        {
            if (currentTile.tiledata.objectOnTile.tag == "BattleEnemy")
            {
                GameObject monster = currentTile.tiledata.objectOnTile;
                monster.GetComponent<MonsterController>().isBattled = true;
                currentTile.tiledata.objectOnTile = this.gameObject;
                MapManager.Instance.StartBattleScene(monster.GetComponent<MonsterInfo>().monsterTypes);
            }
        }
        else
        {
            currentTile.tiledata.objectOnTile = this.gameObject;
        }

        if (currentTile.tiledata.type == Define.TILE.CASTLE && currentTile == targetTile)
        {
            MapManager.Instance.StartTownScene();
        }
        else if (currentTile.tiledata.type == Define.TILE.Dungeon && !currentTile.tiledata.isEntered && targetTile == currentTile)
        {
            currentTile.tiledata.isEntered = true;
            MapManager.Instance.StartDungeonScene();
        }
        else if (currentTile.tiledata.type == Define.TILE.Unknown && !currentTile.tiledata.isEntered && targetTile == currentTile)
        {
            currentTile.tiledata.isEntered = true;
            MapManager.Instance.UnknownEvent();
        }
    }

    void MoveThroughPath(List<Vector3> currentPath)
    {
        MapManager.Instance.isPlayerTurn = false;
        isDetected = false;
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 nowpos = gameObject.transform.position;
        float animstarttime = 0.0f;
        Sequence movementSequence = DOTween.Sequence()
            .OnComplete(() => 
            {
                MapManager.Instance.isPlayerTurn = true;
                _anim.Play($"{movedir}Idle");
                isDetected = MapManager.Instance.chaisingMonsters.Count > 0 ? true : false;

                if (isDetected)
                {
                    MapManager.Instance.isPlayerTurn = false;
                    CoroutineHelper.StartCoroutine(MapManager.Instance.ChasePlayerCoroutine());
                }
            });

        while (pathPositions.Count > 0)
        {
            Vector3 target = pathPositions.Dequeue();
            Vector3 vec = target - nowpos;

            nowpos = target;

            movementSequence.Append(transform.DOMove(target, movementTime).SetEase(Ease.OutQuart).OnComplete(() 
                => { CheckMonster(); MapManager.Instance.AddTime(); MapManager.Instance.NightEvent();
                    MapManager.Instance.UpdateEventListener();
                }))
                .InsertCallback(animstarttime, () => { moveanim(vec); });
            animstarttime = movementTime;
            //if (pathPositions.Count == 0)
            //{
            //    Manager.Data.SavePlayerPos(target);
            //    Manager.Data.SaveGameData();
            //}
        }
    }

    void moveanim(Vector3 pos)
    {
        if (pos == Vector3.left)
            movedir = "Left";
        else if (pos == Vector3.right)
            movedir = "Right";
        else if (pos.y > Vector3.zero.y)
            movedir = "Back";
        else
            movedir = "Front";

        switch (movedir)
        {
            case "Left":
                _anim.Play("Left1");
                break;
            case "Right":
                _anim.Play("Right1");
                break;
            case "Back":
                _anim.Play("Back1");
                break;
            case "Front":
                _anim.Play("Front1");
                break;
        }
    }

    private void CheckMonster()
    {
        UpdateCurrentTile();

        if (currentTile.tiledata.type == Define.TILE.FOREST)
        {
            return;
        }

        foreach (TileInfo tile in currentTile.neighboursTiles)
        {
            if (tile.tiledata.objectOnTile != null)
                if (tile.tiledata.objectOnTile.tag == "BattleEnemy")
                {
                    isDetected = true;
                    MapManager.Instance.chaisingMonsters.Add(Util.GetOrAddComponent<MonsterController>(tile.tiledata.objectOnTile));
                    MapManager.Instance.chaisingMonsters = MapManager.Instance.chaisingMonsters.Distinct().ToList();
                    break;
                }          
        }
    }

    public Vector3Int GetPlayerPos()
    {
        return currentTile.tiledata.pos;
    }

    void SetAnim()
    {
        _anim = Util.GetOrAddComponent<Animator>(gameObject);
        _animatorOverrideController = Manager.Resource.Load<AnimatorOverrideController>("Animations/MapCharacterMove/MapCharacterAOC");
        _anim.runtimeAnimatorController = _animatorOverrideController;

        for (int i = 0; i < 2; i++)
            for(int j = 0; j < 4; j++)
            {
                AnimationClip clip = Manager.Resource.Load<AnimationClip>($"Animations/MapCharacterMove/{Manager.Data.playerstats[0].name}/Move{i}_{j}");
                _animatorOverrideController[$"Move{i}_{j}"] = clip;
            }
    }

    //void MakeAnimClip()
    //{
    //    for (int i = 0; i < 2; i++)
    //        for (int j = 0; j < 4; j++)
    //        {
    //            AnimationClip clip = new AnimationClip();
    //            EditorCurveBinding spriteBinding = new EditorCurveBinding();
    //            spriteBinding.type = typeof(SpriteRenderer);
    //            spriteBinding.path = "";
    //            spriteBinding.propertyName = "m_Sprite";

    //            ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[4];

    //            for (int k = 0; k < 3; k++)
    //            {
    //                spriteKeyFrames[k] = new ObjectReferenceKeyframe();
    //                spriteKeyFrames[k].time = (movementTime / 4) * k;
    //                if (i % 2 == 0)
    //                {
    //                    spriteKeyFrames[k].value = Manager.Data.playerstats[0].sprites[3 * j + k];
    //                }
    //                else
    //                    spriteKeyFrames[k].value = Manager.Data.playerstats[0].sprites[3 * j + 1];
    //            }
    //            spriteKeyFrames[3] = new ObjectReferenceKeyframe();
    //            spriteKeyFrames[3].time = movementTime;
    //            spriteKeyFrames[3].value = Manager.Data.playerstats[0].sprites[3 * j + 1];
    //            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
    //            clip.name = $"{Manager.Data.playerstats[0].name}Move{i}_{j}";
    //            if (!Directory.Exists($"assets/Resources/Animations/MapCharacterMove/{Manager.Data.playerstats[0].name}"))
    //                Directory.CreateDirectory($"assets/Resources/Animations/MapCharacterMove/{Manager.Data.playerstats[0].name}");
    //            AssetDatabase.CreateAsset(clip, $"assets/Resources/Animations/MapCharacterMove/{Manager.Data.playerstats[0].name}/{clip.name}.anim");

    //            //AssetDatabase.AddObjectToAsset(clip, _anim);
    //            AssetDatabase.SaveAssets();
    //            AssetDatabase.Refresh();
    //        }
    //}
}

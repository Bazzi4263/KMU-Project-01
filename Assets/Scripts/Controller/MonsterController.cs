using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private int walkableCost;
    [SerializeField] private int walkableCount;
    [SerializeField] private float movementTime;
    private Grid grid;
    private TileInfo currentTile;
    private TileInfo targetTile;
    private List<Vector3Int> walkablePoses;

    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private BFSResult movementRanage = new BFSResult();

    public MonsterInfo monsterInfo;
    public bool isBattled = false;
    public bool isChasing = false;

    public int WalkableCost {set => walkableCost = value; }
    public int WalkableCount {set => walkableCount = value; }

    private void Awake()
    {
        monsterInfo = GetComponent<MonsterInfo>();
        grid = FindObjectOfType(typeof(Grid)) as Grid;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateCurrentTile();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MapScene")
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            if (isBattled)
            {
                MapManager.Instance.chaisingMonsters.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                this.gameObject.SetActive(true);
                grid = FindObjectOfType(typeof(Grid)) as Grid;
            }
        }
    }

    private void UpdateCurrentTile()
    {
        if (currentTile != null)
        {
            if (currentTile.tiledata.objectOnTile == this.gameObject)
            {
                currentTile.tiledata.objectOnTile = null;
            }
        }

        currentTile = MapManager.Instance.tileDic[grid.WorldToCell(transform.position)];

        if (currentTile.tiledata.objectOnTile != null)
        {
            if (currentTile.tiledata.objectOnTile.tag == "BattlePlayer")
            {
                isBattled = true;
                MapManager.Instance.StopAllCoroutines();
                MapManager.Instance.StartBattleScene(monsterInfo.monsterTypes);
            }
        }
        else
        {
            currentTile.tiledata.objectOnTile = this.gameObject;
        }

    }

    private void OnDestroy()
    {
        if (currentTile.tiledata.objectOnTile == this.gameObject)
        {
            currentTile.tiledata.objectOnTile = null;
        }

        MapManager.Instance.MonsterList.Remove(monsterInfo);
        MapManager.Instance.chaisingMonsters.Remove(this);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChasePlayer(Vector3Int pos)
    {
        if (!this)
        {
            return;
        }

        if (!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
        }

        movementRanage = PathFinding.BFSGetRanage(currentTile.tiledata.pos, walkableCost);
        walkablePoses = new List<Vector3Int>(movementRanage.GetRangePositions());

        foreach (Vector3Int _pos in walkablePoses)
        {
            if (pos == _pos)
            {
                isChasing = true;
                targetTile = MapManager.Instance.tileDic[pos];
                if (grid == null)
                {
                    return;
                }
                MoveThroughPath(movementRanage.GetPathTo(pos).Select(x => grid.CellToWorld(x)).ToList());
            }
            else if (walkablePoses.LastOrDefault().Equals(_pos) && !isChasing)
            {
                MapManager.Instance.chaisingMonsters.Remove(this);
                isChasing = false;
            }
        }
    }

    private void MoveThroughPath(List<Vector3> currentPath)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        Sequence movementSequence = DOTween.Sequence().OnComplete(() =>{ UpdateCurrentTile();});

        if (walkableCount < pathPositions.Count)
        {
            for (int i = 0; i < walkableCount; i++)
            {
                Vector3 target = pathPositions.Dequeue();

                if (i == walkableCount - 1)
                {
                    TileInfo lastTile = MapManager.Instance.tileDic[grid.WorldToCell(target)];

                    if (lastTile.tiledata.objectOnTile)
                    {
                        if (lastTile.tiledata.objectOnTile.tag == "BattleEnemy")
                        {
                            foreach (TileInfo neighborTile in lastTile.neighboursTiles)
                            {
                                if (!neighborTile.tiledata.objectOnTile && neighborTile.tiledata.Walkable)
                                {
                                    target = neighborTile.tiledata.worldPos;
                                    break;
                                }

                                if (lastTile.neighboursTiles.LastOrDefault().Equals(neighborTile))
                                {
                                    return;
                                }
                            } 
                        }
                    }
                }

                movementSequence.Append(transform.DOMove(target, movementTime).SetEase(Ease.OutQuart));

            }
        }
        else
        {
            while (pathPositions.Count > 0)
            {
                Vector3 target = pathPositions.Dequeue();
                movementSequence.Append(transform.DOMove(target, movementTime).SetEase(Ease.OutQuart));
            }
        }
    }
}

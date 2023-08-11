using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//직렬화를 위해 TileInfo에서 List를 제외한 다른 값들을 새로 class로 제작
[Serializable]
public class Tiledata
{
    public Vector3Int pos;
    public Vector3 worldPos;
    public Define.TILE type;
    public int tileNum = 0;
    public bool Walkable = true;
    public int cost;

    public GameObject obj;
    public GameObject objectOnTile = null;
    public bool isEntered = false;
}

public class TileInfo
{
    public Tiledata tiledata = new Tiledata();
    public List<TileInfo> neighboursTiles = new List<TileInfo>();

    public TileInfo(Tiledata tile)
    {
        this.tiledata = tile;
    }

    public TileInfo(Vector3Int _pos, Grid _grid, Define.TILE _type, GameObject _obj = null, bool walkable = true)
    {
        tiledata.pos = _pos;
        tiledata.worldPos = _grid.CellToWorld(_pos);
        tiledata.type = _type;

        tiledata.Walkable = walkable;
        if (tiledata.type == Define.TILE.WATER)
        {
            tiledata.Walkable = false;
        }

        tiledata.obj = _obj;

        tiledata.cost = (int)tiledata.type;

        if (tiledata.type == Define.TILE.CASTLE)
        {
            tiledata.cost = 1;
        }
        else if (tiledata.type == Define.TILE.Dungeon)
        {
            tiledata.cost = 1;
        }
        else if (tiledata.type == Define.TILE.Unknown)
        {
            tiledata.cost = 1;
        }

    }

    public void UpdateNeighbourTiles()
    {
        foreach (Vector3Int dir in Direction.GetDirectionList(tiledata.pos.y))
        {
            if (MapManager.Instance.tileDic.ContainsKey(tiledata.pos + dir))
            {
                neighboursTiles.Add(MapManager.Instance.tileDic[tiledata.pos + dir]);
            }
        }
    }
}

public static class Direction
{
    public static List<Vector3Int> directionsOffsetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 0)
    };

    public static List<Vector3Int> directionsOffsetEven = new List<Vector3Int>
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0)
    };

    public static List<Vector3Int> GetDirectionList(int y)
        => y % 2 == 0 ? directionsOffsetOdd : directionsOffsetEven;
}
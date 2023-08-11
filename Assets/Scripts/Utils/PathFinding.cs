using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    public static BFSResult BFSGetRanage(Vector3Int startPos, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();

        nodesToVisitQueue.Enqueue(startPos);
        costSoFar.Add(startPos, 0);
        visitedNodes.Add(startPos, null);

        while (nodesToVisitQueue.Count > 0)
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (TileInfo neighbourTile in MapManager.Instance.tileDic[currentNode].neighboursTiles)
            {
                if (!neighbourTile.tiledata.Walkable)
                    continue;

                int nodeCost = neighbourTile.tiledata.cost;
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbourTile.tiledata.pos))
                    {
                        visitedNodes[neighbourTile.tiledata.pos] = currentNode;
                        costSoFar[neighbourTile.tiledata.pos] = newCost;
                        nodesToVisitQueue.Enqueue(neighbourTile.tiledata.pos);
                    }
                    else if (costSoFar[neighbourTile.tiledata.pos] > newCost)
                    {
                        costSoFar[neighbourTile.tiledata.pos] = newCost;
                        visitedNodes[neighbourTile.tiledata.pos] = currentNode;
                    }
                }
            }
        }
        return new BFSResult { visitedNodesDict = visitedNodes };
    }

    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodesDict[current] != null)
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}

public struct BFSResult
{
    public Dictionary<Vector3Int, Vector3Int?> visitedNodesDict;

    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (visitedNodesDict.ContainsKey(destination) == false)
            return new List<Vector3Int>();
        return PathFinding.GeneratePathBFS(destination, visitedNodesDict);
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodesDict.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions()
        => visitedNodesDict.Keys;
}





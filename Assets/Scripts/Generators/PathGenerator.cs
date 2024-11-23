using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public class PathGenerator
{
    private const float MOVE_STRAIGHT_COST = 1f;
    private const float MOVE_DIAGONAL_COST = 1.4f;

    private TerrainConfig _terrainConfig;
    private List<PathNode> _pathNodes = new List<PathNode>();

    //private Dictionary<Vector2Int, PathNode> _myPathNodes = new Dictionary<Vector2Int, PathNode>();

    [Inject]
    private void Initialize(TerrainConfig terrainConfig)
    {
        _terrainConfig = terrainConfig;
    }

    /// <summary>
    /// Generate paths from each structure to <paramref name="targetPosition"/>
    /// </summary>
    /// <param name="chunks">List of chunks in the map</param>
    /// <param name="targetPosition">Position where all paths will lead to</param>
    /// <returns></returns>
    public List<Chunk> GeneratePaths(List<Chunk> chunks, Vector2Int targetPosition)
    {
        _pathNodes.Clear();

        //Get all path nodes
        for (int c = 0; c < chunks.Count; c++)
        {
            for(int x = 0; x < chunks[c].Tiles.Count; x++)
            {                
                for(int y = 0; y < chunks[c].Tiles[x].Count; y++)
                {
                    TileData tileData = chunks[c].Tiles[x][y];

                    //t Cost is the tiles distance from the grass height
                    float tCost = _terrainConfig.GetGrassHeight() - tileData.Height;

                    //Create the node
                    if(tileData.TileType == MapConstants.TileType.Grass)
                    {
                        PathNode node = new PathNode(tileData.WorldPosition, tileData, tCost, new Vector2Int(x,y));
                        _pathNodes.Add(node);
                    }
                    
                }
            }
        }

        for(int c = 0; c < chunks.Count; c++)
        {
            for(int s = 0; s < chunks[c].Structures.Count; s++)
            {                
                if (chunks[c].Structures[s].type == MapConstants.StructureType.Enemy)
                {
                    List<PathNode> path = FindStructurePath(chunks[c].Structures[s].position, targetPosition);
                    if(path != null)
                    {
                        chunks = AddPathToMap(chunks, path);
                    }
                }
            }
        }

        return chunks;
    }

    private List<Chunk> AddPathToMap(List<Chunk> chunks, List<PathNode> path)
    {

        //Convert chunk list to tile list
        List<TileData> flattenedTiles = new List<TileData>();
        for(int c = 0; c < chunks.Count; c++)
        {
            for(int x = 0; x < chunks[c].Tiles.Count; x++)
            {
                for(int y = 0; y < chunks[c].Tiles[x].Count; y++)
                {
                    flattenedTiles.Add(chunks[c].Tiles[x][y]);
                }
            }
        }
        for(int i = 0; i < path.Count; i++)
        {
            path[i].RefTile.SetType(MapConstants.TileType.Path);
        }

        return chunks;
    }

    private List<PathNode> FindStructurePath(Vector2Int startPosition, Vector2Int endPosition)
    {
#nullable enable
        PathNode? startNode = null;
        PathNode? endNode = null;
#nullable disable        

        //Get the start and end node
        for (int i = 0; i < _pathNodes.Count; i++)
        {
            //Found start node
            if(_pathNodes[i].worldPosition == startPosition && startNode == null)
            {
                startNode = _pathNodes[i];   
            }

            //Found end node
            if (_pathNodes[i].worldPosition == endPosition && endNode == null)
            {
                endNode = _pathNodes[i];
            }            
        }

        // Initiialise open (with start node) and closed list
        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        //Init all nodes
        for(int i = 0; i < _pathNodes.Count; i ++)
        {
            _pathNodes[i].Reset();
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                //Reach final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<PathNode> neighbours = GetNeighbourNodes(currentNode);
            for(int i = 0; i < neighbours.Count; i++)
            {
                PathNode neighbourNode = neighbours[i];

                if (closedList.Contains(neighbourNode)) continue;

                float tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.parentNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();
                }

                if (!openList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbourNodes(PathNode currentNode)
    {
        List<PathNode> NeighbourNodes = new List<PathNode>
        {
            //Left
            GetNeighbourInDirection(Vector2Int.left),

            //Right
            GetNeighbourInDirection(Vector2Int.right),

            //Up
            GetNeighbourInDirection(Vector2Int.up),

            //Down
            GetNeighbourInDirection(Vector2Int.down)
        };

        //Remove null tiles
        for (int i = NeighbourNodes.Count - 1; i >= 0; i--)
        {
            if (NeighbourNodes[i] == null) NeighbourNodes.RemoveAt(i);
        }
        
        PathNode GetNeighbourInDirection(Vector2Int direction)
        {
            PathNode neighbour = null;

            //neighbour = _pathNodes
            
            Vector2Int neighbourPosition = new Vector2Int(currentNode.worldPosition.x + direction.x, currentNode.worldPosition.y + direction.y);
            neighbour = _pathNodes.Where(node => node.worldPosition == neighbourPosition).FirstOrDefault();



            return neighbour;
        }



        return NeighbourNodes;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes = new List<PathNode> { endNode };
        PathNode currentNode = endNode;

        while(currentNode.parentNode != null)
        {
            pathNodes.Add(currentNode.parentNode);
            currentNode = currentNode.parentNode;
        }
        pathNodes.Reverse();

        return pathNodes;
    }

    private float CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.worldPosition.x - b.worldPosition.x);
        int yDistance = Mathf.Abs(a.worldPosition.y - b.worldPosition.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> nodes)
    {
        PathNode lowestFCostNode = nodes[0];

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodes[i];
            }
        }

        return lowestFCostNode;
    }
    private class PathNode
    {        
        public Vector2Int worldPosition { get; private set; }
        public Vector2Int index { get; private set; }
        public TileData RefTile;
        public PathNode parentNode;
        public float gCost; // The length of the path from the start node to this node
        public float hCost; // The straight-line distance from this node to the end node
        public float fCost { get; private set; } // An estimate of the total distance if taking this route (F = G + H)
        public float tCost { get; private set; } // Terrain cost, used to bias paths towards grass                            

        public PathNode(Vector2Int worldPosition, TileData refTile, float tCost, Vector2Int index)
        {
            this.worldPosition = worldPosition;
            RefTile = refTile;
            this.tCost = tCost;
            this.index = index;

            Reset();
        }

        public void Reset()
        {
            gCost = int.MaxValue;
            CalculateFCost();
            parentNode = null;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PathGenerator
{
    private const float MOVE_STRAIGHT_COST = 1f;
    private const float MOVE_DIAGONAL_COST = 1.4f;

    private Map _map;
    private TerrainConfig _terrainConfig;
    private List<PathNodeChunk> _chunkedPathNodes = new List<PathNodeChunk>();

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
    public List<Chunk> GeneratePaths(Map map, Vector2Int targetPosition)
    {
        _map = map;
        List<Chunk> chunks = map.Chunks;

        _chunkedPathNodes.Clear();

        //Create and chunk all path nodes
        for (int c = 0; c < chunks.Count; c++)
        {
            List<PathNode> pathNodes = new List<PathNode>();
            //Get all tiles in chunk and make a pathnode for them
            for(int x = 0; x < chunks[c].Tiles.Count; x++)
            {
                for(int y = 0; y < chunks[c].Tiles[x].Count; y++)
                {
                    TileData tileData = chunks[c].Tiles[x][y];

                    //Create the node only if this is a grass tile (no pathing over oceans or mountains)
                    if(tileData.TileType == MapConstants.TileType.Grass)
                    {
                        PathNode newNode = new PathNode(tileData);
                        pathNodes.Add(newNode);
                    }
                }
            }

            //Create and add new chunk
            PathNodeChunk newChunk = new PathNodeChunk(pathNodes, chunks[c].GridPosition);
            _chunkedPathNodes.Add(newChunk);
        }

        //Find paths for all structures
        for(int c = 0; c < chunks.Count; c++)
        {
            for(int s = 0; s < chunks[c].Structures.Count; s++)
            {
                if (chunks[c].Structures[s].type == MapConstants.StructureType.Enemy)
                {
                    List<PathNode> path = FindStructurePath(chunks[c].Structures[s].position, targetPosition);
                    if (path != null)
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
        for(int c = 0; c < _chunkedPathNodes.Count; c++)
        {
            for (int n = 0; n < _chunkedPathNodes[c].Nodes.Count; n++)
            {
                PathNode node = _chunkedPathNodes[c].Nodes[n];

                //Found start node
                if (node.RefTile.WorldPosition == startPosition && startNode == null)
                {
                    startNode = node;
                }

                //Found end node
                if (node.RefTile.WorldPosition == endPosition && endNode == null)
                {
                    endNode = node;
                }
            }
        }


        // Initiialise open (with start node) and closed list
        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        //Init all nodes
        for(int i = 0; i < _chunkedPathNodes.Count; i ++)
        {
            _chunkedPathNodes[i].InitAllNodes();
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
            
            Vector2Int neighbourPosition = new Vector2Int(currentNode.RefTile.WorldPosition.x + direction.x, currentNode.RefTile.WorldPosition.y + direction.y);

            //Reduce search to a single chunk for speed
            PathNodeChunk chunkToSearch = GetNodeChunkNearPosition(neighbourPosition);

            if (chunkToSearch != null)
            {
                neighbour = chunkToSearch.Nodes.Where(node => node.RefTile.WorldPosition == neighbourPosition).FirstOrDefault();
            }

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
        int xDistance = Mathf.Abs(a.RefTile.WorldPosition.x - b.RefTile.WorldPosition.x);
        int yDistance = Mathf.Abs(a.RefTile.WorldPosition.y - b.RefTile.WorldPosition.y);
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

    /// <summary>
    /// Gets the chunk near given world position
    /// </summary>
    /// <param name="worldPosition">World position to find chunk near</param>
    /// <returns></returns>
    private PathNodeChunk GetNodeChunkNearPosition(Vector2Int worldPosition)
    {
        int chunkSize = _map.GetChunkSize();

        //Convert world position to chunk position       
        Vector2Int chunkPosition = new Vector2Int(
            worldAxisToChunkAxis(worldPosition.x),
            worldAxisToChunkAxis(worldPosition.y)
            );

        //Find chunk
        PathNodeChunk chunk = _chunkedPathNodes.Where(chunk => chunk.ChunkPosition == chunkPosition).FirstOrDefault();
        return chunk;

        //Convert world axis to chunk axis (eg. x portion of world pos to x portion of chunk pos)
        int worldAxisToChunkAxis(int worldAxis) => Mathf.FloorToInt((worldAxis + (chunkSize / 2f)) / chunkSize);
    }

    /// <summary>
    /// Basically just a map tile with extra info for path finding
    /// </summary>
    private class PathNode
    {
        public TileData RefTile;
        public PathNode parentNode;
        public float gCost; // The length of the path from the start node to this node
        public float hCost; // The straight-line distance from this node to the end node
        public float fCost { get; private set; } // An estimate of the total distance if taking this route (F = G + H)                                   

        public PathNode(TileData refTile)
        {
            RefTile = refTile;
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

    /// <summary>
    /// Used to chunk path nodes for faster searching
    /// </summary>
    private class PathNodeChunk
    {
        public List<PathNode> Nodes { get; private set; }
        public Vector2Int ChunkPosition { get; private set; }

        public PathNodeChunk(List<PathNode> nodes, Vector2Int chunkPosition)
        {
            Nodes = nodes;
            ChunkPosition = chunkPosition;
        }

        public void InitAllNodes()
        {
            for(int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Reset();
            }
        }
    }
}

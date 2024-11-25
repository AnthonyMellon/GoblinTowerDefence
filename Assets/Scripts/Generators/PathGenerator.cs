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
    /// Generates paths for all structures on the map. The map should already have structures before calling this.
    /// </summary>
    /// <param name="map">The map to generate paths for</param>
    /// <returns></returns>
    public List<Chunk> GeneratePaths(Map map)
    {
        _map = map;
        List<Chunk> chunks = map.Chunks;

        //These are the points we are wanting to path find towards. Any point connected to the player should be added to this list
        List<StructureBase> connectedStructures = _map.GetAllPlayerStructures();


        //Create chunks of path nodes. Path nodes are chunked to improve search times when pathfinding
        _chunkedPathNodes.Clear();
        for (int c = 0; c < chunks.Count; c++)
        {
            PathNodeChunk newChunk = new PathNodeChunk(chunks[c]);
            _chunkedPathNodes.Add(newChunk);
        }

        //Generate Paths for all structures
        for(int c = 0; c < chunks.Count; c++) // Each chunk
        {
            for(int s = 0; s < chunks[c].Structures.Count; s++) // Each structure in chunk
            {
                StructureBase structure = chunks[c].Structures[s];
                if (structure is EnemyStructure)
                {
                    Vector2Int startPosition = structure.OwnerTile.WorldPosition;
                    List<PathNode> path = FindStructurePath(startPosition, findBestPathfindPoint(startPosition));

                    //Path for structure found, add it to the map and record it as a connected position
                    if (path != null)
                    {
                        chunks = AddPathToMap(chunks, path);
                        connectedStructures.Add(structure);
                    }
                }
            }
        }

        return chunks;


        //TODO: Implement a better way of finding closest structures (take into account terrain)
        // Used to find the most appropriate point a structure should path find to
        Vector2Int findBestPathfindPoint(Vector2Int startPosition)
        {
            Vector2Int bestPoint = connectedStructures[0].OwnerTile.WorldPosition;

            //Find position of closest structure already pathed to player
            for(int i = 0; i < connectedStructures.Count; i++)
            {
                Vector2Int structurePosition = connectedStructures[i].OwnerTile.WorldPosition;
                float distance = Vector2Int.Distance(startPosition, structurePosition);
                if (distance < Vector2Int.Distance(startPosition, bestPoint))
                {
                    bestPoint = structurePosition;
                }
            }
            
            return bestPoint;
        }
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

        //Find start and end node matching start and end positions
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


        // Lists used to track which nodes have and have not been checked when pathing        
        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        //Init all nodes ready for pathing
        for(int i = 0; i < _chunkedPathNodes.Count; i ++)
        {
            _chunkedPathNodes[i].InitAllNodes();
        }

        // Setup start node
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);

        //Actual pathing
        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                //Path found - build it into a list of nodes
                return BuildPath(endNode);
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

        // Out of nodes on the openList - no path found
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

        //Null tiles could be found at the edge of the map or near un-pathable terrain
        for (int i = NeighbourNodes.Count - 1; i >= 0; i--)
        {
            if (NeighbourNodes[i] == null) NeighbourNodes.RemoveAt(i);
        }
        
        PathNode GetNeighbourInDirection(Vector2Int direction)
        {
            PathNode neighbour = null;
            Vector2Int neighbourPosition = new Vector2Int(currentNode.RefTile.WorldPosition.x + direction.x, currentNode.RefTile.WorldPosition.y + direction.y);

            //Find the chunk that cointains the pathnode to avoid searching the whole map
            PathNodeChunk chunkToSearch = GetNodeChunkNearPosition(neighbourPosition);

            if (chunkToSearch != null)
            {
                neighbour = chunkToSearch.Nodes.Where(node => node.RefTile.WorldPosition == neighbourPosition).FirstOrDefault();
            }

            return neighbour;
        }
        return NeighbourNodes;
    }

    private List<PathNode> BuildPath(PathNode endNode)
    {
        List<PathNode> pathNodes = new List<PathNode> { endNode };
        PathNode currentNode = endNode;

        // Recurse throug node tree to build path
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

        public PathNodeChunk(Chunk chunk)
        {
            ChunkPosition = chunk.GridPosition;
            CreateNodesFromChunk(chunk);
        }

        private void CreateNodesFromChunk(Chunk chunk)
        {
            List<PathNode> newNodes = new List<PathNode>();

            for (int x = 0; x < chunk.Tiles.Count; x++)
            {
                for (int y = 0; y < chunk.Tiles[x].Count; y++)
                {
                    TileData tileData = chunk.Tiles[x][y];

                    //Create the node only if this is a grass tile (no pathing over oceans or mountains)
                    if (tileData.TileType == MapConstants.TileType.Grass)
                    {
                        PathNode newNode = new PathNode(tileData);
                        newNodes.Add(newNode);
                    }
                }
            }

            Nodes?.Clear();
            Nodes = newNodes;
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

using UnityEngine;
using System.Collections.Generic;
using static MapConstants;
using Zenject;

public class ChunkGenerator
{
    private ChunkConfig _config;

    public int GetChunkSize() => _config.GetChunkSize();

    [Inject]
    private void Initialize(ChunkConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Generates empty chunks
    /// </summary>
    /// <returns>List of chunks</returns>
    public List<Chunk> GenerateChunks()
    {
        List<Chunk> chunks = new List<Chunk>();

        //Generate Center Chunk
        chunks.Add(GenerateChunk(new Vector2Int(0, 0), ChunkOwner.Player, true));

        //Generate Chunks in a square spiral pattern
        for (int r = 0; r <= _config.GetMapRadius(); r++)
        {
            bool revealed = r <= _config.GetInitialRevealRadius();

            for (int x = 0 - r; x < r; x++) //Sweep across top left to top right
            {
                chunks.Add(GenerateChunk(indexToChunkWorldPos(x, r), startRevealed: revealed));
            }

            for (int y = r; y > 0 - r; y--) //Sweep top right to bottom right
            {
                chunks.Add(GenerateChunk(indexToChunkWorldPos(r, y), startRevealed: revealed));
            }

            for (int x = r; x > 0 - r; x--) //Sweep bottom right to bottom left
            {
                chunks.Add(GenerateChunk(indexToChunkWorldPos(x, 0 - r), startRevealed: revealed));
            }

            for (int y = 0 - r; y < r; y++) //Sweep bottom left to top left
            {
                chunks.Add(GenerateChunk(indexToChunkWorldPos(0 - r, y), startRevealed: revealed));
            }
        }
        return chunks;

        Vector2Int indexToChunkWorldPos(int x, int y) => new Vector2Int(x, y) * _config.GetChunkSize();
    }

    /// <summary>
    /// Generate a chunk full of unset tiles
    /// </summary>
    /// <param name="chunkWorldPos">World position of the center of the chunk</param>
    /// <param name="startRevealed">Should the chunk start revealed? (no fog)</param>
    /// <returns>A new chunk</returns>
    private Chunk GenerateChunk(Vector2Int chunkWorldPos, ChunkOwner owner = ChunkOwner.Enemy,  bool startRevealed = false)
    {
        List<List<TileData>> tiles = new List<List<TileData>>();

        //Generate tile values for chunk
        for (int x = 0; x < _config.GetChunkSize(); x++)
        {
            List<TileData> column = new List<TileData>();
            for (int y = 0; y < _config.GetChunkSize(); y++)
            {
                //Calculate tiles position in world
                Vector2Int tileWorldPosition = new Vector2Int(
                    chunkWorldPos.x + x - (_config.GetChunkSize() / 2),
                    chunkWorldPos.y + y - (_config.GetChunkSize() / 2)
                    );

                TileData tile = new TileData(0, TileType.Invalid, tileWorldPosition);
                column.Add(tile);
            }
            tiles.Add(column);
        }

        return new Chunk(tiles, chunkWorldPos, _config.GetChunkSize(), owner, startRevealed);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using static MapConstants;

public class StructureGenerator
{
    public (List<Chunk> chunks, Vector2Int playerPosition) GenerateStructures(List<Chunk> chunks)
    {
        List<Chunk> playerChunks = chunks.Where(chunk => chunk.Owner == ChunkOwner.Player).ToList();
        List<Chunk> enemyChunks = chunks.Where(chunk => chunk.Owner == ChunkOwner.Enemy).ToList();
        
        //Generate Structures
        GenerateStructuresOfType(playerChunks, StructureType.Player, forceToCenterOfChunk: true);
        GenerateStructuresOfType(enemyChunks, StructureType.Enemy);

        // Get Player Structure Position
        Vector2Int playerPos;
        // Ensure there is a player chunk with at least one structure in it
        if (playerChunks.Count >= 1 && playerChunks[0].Structures.Count >= 1)
        {
            // Get the first player structure
            playerPos = playerChunks[0].Structures[0].position;
        }
        else
        {
            // No player structure, default to middle of the map
            playerPos = Vector2Int.zero;
        }

        return (chunks, playerPos);
    }

    private void GenerateStructuresOfType(List<Chunk> chunks, StructureType type, bool forceToCenterOfChunk = false)
    {
        int targetNumStructures = chunks.Count;
        int generatedStructures = 0;

        // List of chunks that contain at least one valid structure spawn spot
        List<Chunk> validChunks = new List<Chunk>();

        // Loop through all chunks and check attempt to spawn a structure
        // This should ensure a strucute in all chunks with a valid spawn space
        for (int i = 0; i < chunks.Count; i++)
        {

            TileData tileData;
            if (forceToCenterOfChunk) tileData = chunks[i].GetCenterTile();
            else tileData = chunks[i].GetRandomGrassTile();

            //Ensure a valid tile was found and that it does not already contain a structure
            if (tileData != null && tileData.HasStructure == false)
            {
                //Make a new structure
                AddStructure(tileData, chunks[i], type);

                validChunks.Add(chunks[i]);
                generatedStructures++;
            }
        }

        

        // If all the structures have been generate then we're done
        if (generatedStructures == targetNumStructures)
        {
            Debug.Log($"Generated <color=green>{generatedStructures}/{targetNumStructures}</color> structures of type <color=cyan>{type}</color>");
            return;
        }

        Debug.Log($"Generated <color=green>{generatedStructures}/{targetNumStructures}</color> structures of type <color=cyan>{type}</color>");

        // Second pass of spawning, if not enough structures could be spawned with one per chunk
        // Start spawning more structures in random valid chunks
        int attempts = 0;
        int maxAttempts = 100;
        while(attempts < maxAttempts && generatedStructures <= targetNumStructures)
        {
            //TODO: Implement
            attempts++;
        }
    }

    private void AddStructure(TileData tileData, Chunk chunk, StructureType type)
    {
        tileData.SetHasStructure(true);        
        chunk.AddStrucuture(tileData.WorldPosition, type);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using static MapConstants;

public class StructureGenerator
{
    private PlayerStructure.Factory _playerStructureFacotry;
    private EnemyStructure.Factory _enemyStructureFacotry;
    private Transform _structureContainer;

    [Inject]
    private void Initialize(PlayerStructure.Factory playerStructureFactory, EnemyStructure.Factory enemyStructureFactory)
    {
        _playerStructureFacotry = playerStructureFactory;
        _enemyStructureFacotry = enemyStructureFactory;
    }

    public (List<Chunk> chunks, Vector2Int playerPosition) GenerateStructures(List<Chunk> chunks, Transform structureContainer)
    {
        _structureContainer = structureContainer;

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
            // Get the position of the first player structure (there should only ever be one in normal gameplay)
            playerPos = playerChunks[0].Structures[0].OwnerTile.WorldPosition;
        }
        else
        {
            // No player structure found (bad), default to middle of the map
            Debug.LogWarning("Failed to find player structure... that's not a good thing");
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
                GenerateStructure(tileData, chunks[i], type);

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

    private void GenerateStructure(TileData ownerTile, Chunk ownerChunk, StructureType type)
    {        
        StructureBase structure = null;

        switch (type)
        {
            case StructureType.Player:
                structure = _playerStructureFacotry.Create(ownerTile.WorldPosition);
                break;
            case StructureType.Enemy:
                structure = _enemyStructureFacotry.Create(ownerTile.WorldPosition);
                break;
        }

        if (structure != null)
        {
            ownerTile.SetHasStructure(true);
            structure.SetOwnerTile(ownerTile);
            ownerChunk.AddStrucuture(structure);

            structure.transform.SetParent(_structureContainer, false);
        }
    }
}

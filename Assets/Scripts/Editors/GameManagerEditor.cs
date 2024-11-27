using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gameManager = (GameManager)target;

        if(GUILayout.Button("Regenerate Map"))
        {
            gameManager.GenerateMap();
        }

        if(GUILayout.Button("Spawn New Wave"))
        {
            gameManager.SpawnNewWave();
        }

        if(GUILayout.Button("Kill All Enemies"))
        {
            gameManager.DestroyAllEnemies();
        }

       
    }
}

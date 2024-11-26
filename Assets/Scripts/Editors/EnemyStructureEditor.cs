using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(EnemyStructure))]
public class EnemyStructureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyStructure structure = (EnemyStructure)target;
        if(GUILayout.Button("Spawn Enemy"))
        {
            structure.SpawnEnemy();
        }
    }
}

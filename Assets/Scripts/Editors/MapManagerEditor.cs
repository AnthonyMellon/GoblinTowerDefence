using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapManager mapManager = (MapManager)target;
        if(GUILayout.Button("Regenerate Map"))
        {
            mapManager.GenerateMap();
            mapManager.DrawMap();
        }
    }
}

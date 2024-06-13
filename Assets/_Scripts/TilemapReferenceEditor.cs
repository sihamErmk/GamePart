using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TilemapReference))]
public class TilemapReferenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Cast the target to TilemapReference
        TilemapReference tilemapRef = (TilemapReference)target;

        // Allow dragging a GameObject into the field
        tilemapRef.floorTilemap = (Tilemap)EditorGUILayout.ObjectField("Floor Tilemap", tilemapRef.floorTilemap, typeof(Tilemap), true);
        tilemapRef.wallTilemap = (Tilemap)EditorGUILayout.ObjectField("Wall Tilemap", tilemapRef.wallTilemap, typeof(Tilemap), true);
    }
}

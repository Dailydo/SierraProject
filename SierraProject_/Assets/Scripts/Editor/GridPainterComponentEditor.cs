using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridPainterComponent))]
[CanEditMultipleObjects]
public class GridPainterComponentEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();

        if(GUILayout.Button("Paint !")) 
        {
            GridPainterComponent gridPainter = (GridPainterComponent)(target);
            if (gridPainter)
            {
                gridPainter.ProcessPaintRequests();
            }
        }
    }
}
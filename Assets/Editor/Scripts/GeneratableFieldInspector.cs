using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RainyDay.GeneratableField))]
public class GeneratableFieldInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var field = target as RainyDay.GeneratableField;
        if (GUILayout.Button("Prebake Field"))
        {
            field.GenerateField();
        }
    }
}

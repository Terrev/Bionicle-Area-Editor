using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Positions))]
public class PositionsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Positions positions = (Positions)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		if (GUILayout.Button("Snap all pickups"))
		{
			positions.SnapAllPickups();
		}
		GUILayout.Space(10);
		if (GUILayout.Button("Revert all pickups"))
		{
			positions.RevertAllPickups();
		}
	}
}

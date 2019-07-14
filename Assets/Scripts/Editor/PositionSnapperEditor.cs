using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PositionSnapper))]
public class PositionSnapperEditor : Editor
{
	public override void OnInspectorGUI()
	{
		PositionSnapper positionSnapper = (PositionSnapper)target;
		DrawDefaultInspector();
		GUILayout.Space(10);
		if (GUILayout.Button("Snap pickup (floats 1.5 units from ground)"))
		{
			positionSnapper.SnapDownwards();
		}
	}
}

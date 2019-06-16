using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Movelists))]
public class MovelistsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GUILayout.Space(10);
		
		Movelists movelists = (Movelists)target;
		
		if (GUILayout.Button("Movelist SLB to XML"))
		{
			movelists.SlbToXml();
		}
	}
}
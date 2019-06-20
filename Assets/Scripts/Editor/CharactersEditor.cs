using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Characters))]
public class CharactersEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GUILayout.Space(10);
		
		Characters characters = (Characters)target;
		
		if (GUILayout.Button("Movelist SLB to XML"))
		{
			characters.SlbToXml();
		}
		if (GUILayout.Button("Movelist XML to SLB"))
		{
			characters.XmlToSlb();
		}
	}
}
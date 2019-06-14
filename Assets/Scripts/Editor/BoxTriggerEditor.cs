using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BoxTrigger))]
public class BoxTriggerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		BoxTrigger boxTrigger = (BoxTrigger)target;
		if (GUILayout.Button("Apply transformation"))
		{
			boxTrigger.ApplyTransformation();
		}
		if (GUILayout.Button("Turn into cube"))
		{
			boxTrigger.TurnIntoCube();
		}
	}
}
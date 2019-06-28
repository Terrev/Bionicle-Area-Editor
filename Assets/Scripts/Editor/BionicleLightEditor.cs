using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BionicleLight))]
public class BionicleLightEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		BionicleLight bionicleLight = (BionicleLight)target;
		if (bionicleLight.lightType == LightType.Directional)
		{
			if (GUILayout.Button("Normalize"))
			{
				bionicleLight.Normalize();
			}
		}
	}
}
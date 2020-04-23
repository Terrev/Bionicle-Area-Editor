using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TestCinCamLewaFixer))]
public class TestCinCamLewaFixerEditor : Editor // sick class name again
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TestCinCamLewaFixer testCinCamLewaFixer = (TestCinCamLewaFixer)target;
		if (GUILayout.Button("Save"))
		{
			testCinCamLewaFixer.Save();
		}
	}
}
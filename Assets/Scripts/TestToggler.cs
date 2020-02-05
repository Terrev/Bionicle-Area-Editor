using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestToggler : MonoBehaviour
{
	// flickers between two gameobjects every time gizmos are drawn (such as every frame while the camera is moving in the editor)
	// was using to check for visual differences between models
	
	public GameObject gameObject1;
	public GameObject gameObject2;
	
	public static bool blah;
	
	//[MenuItem("Editor/Toggle Object _F1")]
	public static void ToggleObject()
	{
		blah = !blah;
	}
	
	void OnDrawGizmos()
	{
		if (gameObject1 != null && gameObject2 != null)
		{
			gameObject1.SetActive(blah);
			gameObject2.SetActive(!blah);
		}
	}
}

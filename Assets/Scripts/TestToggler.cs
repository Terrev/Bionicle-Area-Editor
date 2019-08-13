using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToggler : MonoBehaviour
{
	// flickers between two gameobjects every time gizmos are drawn (such as every frame while the camera is moving in the editor)
	// was using to check for visual differences between models
	
	public GameObject gameObject1;
	public GameObject gameObject2;
	
	bool blah;
	
	void OnDrawGizmos()
	{
		if (gameObject1 != null && gameObject2 != null)
		{
			gameObject1.SetActive(blah);
			gameObject2.SetActive(!blah);
			blah = !blah;
		}
	}
}

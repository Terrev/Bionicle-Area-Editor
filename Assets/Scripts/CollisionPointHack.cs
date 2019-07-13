using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollisionPointHack : MonoBehaviour
{
	void LateUpdate()
	{
		// don't move
		transform.rotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;
	}
}

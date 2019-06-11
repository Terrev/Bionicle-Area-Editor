using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollisionPointHack : MonoBehaviour
{
	// COUNTERACT ROTATION SO WE CAN SEE COLLISION ACTIVATION BOXES PROPERLY
	void Update()
	{
		if (transform.parent.rotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.Euler(-transform.parent.rotation.eulerAngles.x, -transform.parent.rotation.eulerAngles.y, -transform.parent.rotation.eulerAngles.z);
		}
		transform.localPosition = Vector3.zero; // and don't let this move
	}
}

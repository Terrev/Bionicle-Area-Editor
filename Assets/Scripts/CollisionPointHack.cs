using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollisionPointHack : MonoBehaviour
{
	void Update()
	{
		// COUNTERACT ROTATION SO WE CAN SEE COLLISION ACTIVATION BOXES PROPERLY
		if (transform.parent.rotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.Euler(-transform.parent.rotation.eulerAngles.x, -transform.parent.rotation.eulerAngles.y, -transform.parent.rotation.eulerAngles.z);
		}
		// and don't let this move
		if (transform.localPosition != Vector3.zero)
		{
			transform.localPosition = Vector3.zero;
		}
	}
}

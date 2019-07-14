using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSnapper : MonoBehaviour
{
	public void SnapDownwards()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 30.0f))
		{
			transform.localPosition = transform.localPosition + (Vector3.down * hit.distance) + new Vector3(0.0f, 1.5f, 0.0f);
			Debug.Log("Snapped " + gameObject.name);
		}
		else
		{
			Debug.LogWarning("Could not snap " + gameObject.name + ", does the model beneath it have Generate Colliders enabled in its import settings?");
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSnapper : MonoBehaviour
{
	public float snapDistance = 30.0f;
	
	[System.NonSerialized]
	public Vector3 loadedPosition = Vector3.zero;
	
	public void SnapDownwards(float distanceToSnap, bool warnIfNotPickup)
	{
		if
		(
			!gameObject.name.StartsWith("am") &&  // ammo
			!gameObject.name.StartsWith("ar") &&  // arrow
			!gameObject.name.StartsWith("et") &&  // token
			!gameObject.name.StartsWith("pa") &&  // pickup air
			!gameObject.name.StartsWith("pe") &&  // pickup energy
			!gameObject.name.StartsWith("ph")     // pickup health
		)
		{
			if (warnIfNotPickup)
			{
				Debug.LogWarning(gameObject.name + " doesn't appear to be a pickup; not snapping it");
			}
			return;
		}
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToSnap))
		{
			Vector3 newPosition = transform.localPosition + (Vector3.down * hit.distance) + new Vector3(0.0f, 1.5f, 0.0f);
			float distanceSnapped = transform.localPosition.y - newPosition.y;
			transform.localPosition = newPosition;
			Debug.Log("Snapped " + gameObject.name + ", distance moved: " + distanceSnapped);
		}
		else
		{
			Debug.LogWarning("Nothing found below " + gameObject.name + " (Does the model beneath it have Generate Colliders enabled in its import settings?)");
		}
	}
	
	public void RevertToLoadedPosition()
	{
		transform.localPosition = loadedPosition;
	}
}

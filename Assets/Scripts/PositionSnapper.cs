using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSnapper : MonoBehaviour
{
	public float snapDistance = 30.0f;
	
	[System.NonSerialized]
	public Vector3 loadedPosition = Vector3.zero;
	
	// original game raycasts from 2.5 above pickup height to 30 below pickup height, then sets pickup height to 1.5 above point raycast hits
	public void SnapDownwards(float distanceToSnap, bool warnIfNotPickup)
	{
		if
		(
			!gameObject.name.StartsWith("am") &&  // ammo
			//!gameObject.name.StartsWith("ar") &&  // arrow - these snap in-game, but they probably shouldn't
			!gameObject.name.StartsWith("et") &&  // token
			//!gameObject.name.StartsWith("pa") &&  // pickup air - actually, these *don't* snap in-game
			!gameObject.name.StartsWith("pe") &&  // pickup energy
			!gameObject.name.StartsWith("ph")     // pickup health
		)
		{
			if (warnIfNotPickup)
			{
				Debug.LogWarning("Not snapping " + gameObject.name + " because it's either not a pickup type we want to snap or not a pickup at all");
			}
			return;
		}
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position + new Vector3(0.0f, 2.5f, 0.0f), Vector3.down, out hit, distanceToSnap + 2.5f))
		{
			Vector3 newPosition = transform.localPosition + new Vector3(0.0f, 4.0f - hit.distance, 0.0f); // 4.0f = 2.5f to counteract the shenanigans earlier + 1.5f for the actual boost upwards
			float distanceSnapped = newPosition.y - transform.localPosition.y;
			transform.localPosition = newPosition;
			Debug.Log("Snapped " + gameObject.name + ", distance moved: " + distanceSnapped);
		}
		else
		{
			Debug.LogWarning("Nothing found below " + gameObject.name + " (Does the model beneath it have Generate Colliders enabled in its import settings?)");
		}
	}
	
	public void RevertToLoadedPosition(bool warnIfNotPickup)
	{
		if
		(
			!gameObject.name.StartsWith("am") &&  // ammo
			//!gameObject.name.StartsWith("ar") &&  // arrow - these snap in-game, but they probably shouldn't
			!gameObject.name.StartsWith("et") &&  // token
			//!gameObject.name.StartsWith("pa") &&  // pickup air - actually, these *don't* snap in-game
			!gameObject.name.StartsWith("pe") &&  // pickup energy
			!gameObject.name.StartsWith("ph")     // pickup health
		)
		{
			if (warnIfNotPickup)
			{
				Debug.LogWarning("Not reverting " + gameObject.name + " because it's not something that could have been snapped");
			}
			return;
		}
		if (loadedPosition == Vector3.zero)
		{
			Debug.LogWarning("Loaded position for " + gameObject.name + " is 0/0/0, it probably got reset somehow (is it new/pasted, or did scripts recompile?), not reverting");
			return;
		}
		transform.localPosition = loadedPosition;
		Debug.Log("Reverted " + gameObject.name);
	}
}

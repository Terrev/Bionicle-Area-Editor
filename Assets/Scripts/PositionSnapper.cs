﻿using System.Collections;
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
		if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToSnap))
		{
			Vector3 newPosition = transform.localPosition + (Vector3.down * hit.distance) + new Vector3(0.0f, 1.5f, 0.0f);
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
	}
}

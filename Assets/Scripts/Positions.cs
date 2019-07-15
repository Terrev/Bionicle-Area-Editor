using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positions : MonoBehaviour
{
	public float snapDistance = 30.0f;
	
	public void SnapAllPickups()
	{
		Debug.Log("================= SNAPPING ALL PICKUPS =================");
		foreach (Transform position in transform)
		{
			position.gameObject.GetComponent<PositionSnapper>().SnapDownwards(snapDistance, false);
		}
		Debug.Log("================= DONE SNAPPING ALL PICKUPS =================");
	}
	
	public void RevertAllPickups()
	{
		foreach (Transform position in transform)
		{
			position.gameObject.GetComponent<PositionSnapper>().RevertToLoadedPosition(false);
		}
	}
}

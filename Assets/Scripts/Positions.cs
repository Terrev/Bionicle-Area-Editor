using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positions : MonoBehaviour
{
	public float snapDistance = 30.0f;
	
	public void SnapAllPickups()
	{
		foreach (Transform position in transform)
		{
			position.gameObject.GetComponent<PositionSnapper>().SnapDownwards(snapDistance, false);
		}
	}
	
	public void RevertAllPickups()
	{
		foreach (Transform position in transform)
		{
			position.gameObject.GetComponent<PositionSnapper>().RevertToLoadedPosition();
		}
	}
}

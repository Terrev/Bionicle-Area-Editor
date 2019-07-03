using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightHandle : MonoBehaviour
{
	BionicleSpotlight parentSpotlight;
	
	void OnDrawGizmos()
	{
		if (parentSpotlight == null)
		{
			parentSpotlight = transform.parent.gameObject.GetComponent<BionicleSpotlight>();
		}
		if (parentSpotlight == null)
		{
			return;
		}
		Gizmos.color = parentSpotlight.colorWithoutAlpha;
		Gizmos.DrawWireSphere(transform.position, 2.0f);
	}
}

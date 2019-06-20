using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CinCharacter : MonoBehaviour
{
	public string characterName;
	public string animBaked;
	public float maskSwitchTime1;
	public float maskSwitchTime2;
	
	void Update()
	{
		// don't move
		if (transform.localPosition != Vector3.zero)
		{
			transform.localPosition = Vector3.zero;
		}
		if (transform.localRotation != Quaternion.identity)
		{
			transform.localRotation = Quaternion.identity;
		}
	}
}

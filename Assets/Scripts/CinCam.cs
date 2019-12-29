using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CinCamFrame
{
	public float time;
	public Vector3 position;
	public Vector3 target;
}

public class CinCam : MonoBehaviour
{
	public float viewAngle;
	public float spinMask1;
	public float spinMask2;
	public float spinMask3;
	public CinCamFrame[] frames;
	
	bool colorToggle = false;
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		for (int i = 0; i < frames.Length; i++)
		{
			if (i != frames.Length - 1)
			{
				Gizmos.DrawLine(frames[i].position, frames[i + 1].position);
				//ToggleColor();
			}
		}
		
		/*
		Gizmos.color = Color.yellow;
		for (int i = 0; i < frames.Length; i++)
		{
			if (i != frames.Length - 1)
			{
				Gizmos.DrawLine(frames[i].target, frames[i + 1].target);
			}
		}
		*/
	}
	
	void ToggleColor()
	{
		if (colorToggle)
		{
			Gizmos.color = Color.cyan;
		}
		else
		{
			Gizmos.color = Color.yellow;
		}
		colorToggle = !colorToggle;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCinCamPlayer : MonoBehaviour
{
	public CinCam cinCam;
	public float time = 0.0f;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			time = 0.0f;
		}
		else if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			time = 6.5f;
		}
		for (int i = 0; i < cinCam.frames.Length; i++)
		{
			if (cinCam.frames[i].time >= time)
			{
				transform.position = cinCam.frames[i].position;
				transform.LookAt(cinCam.frames[i].target);
				break;
			}
		}
		time += Time.deltaTime;
	}
}

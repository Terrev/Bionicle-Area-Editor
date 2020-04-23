using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class TestCinCamLewaFixer : MonoBehaviour
{
	int firstFrameToTweak = 216;
	int lastFrameToTweak = 285;

	public void Save()
	{
		CinCam cinCam = gameObject.GetComponent<CinCam>();
		if (cinCam == null)
		{
			Debug.LogError("CinCam component not found");
		}

		string path = EditorUtility.SaveFilePanel("Save SLB", "", "cin1_CAM.slb", "slb");
		if (path.Length == 0)
		{
			return;
		}

		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);

		binaryWriter.Write(cinCam.viewAngle);
		binaryWriter.Write(cinCam.spinMask1);
		binaryWriter.Write(cinCam.spinMask2);
		binaryWriter.Write(cinCam.spinMask3);
		binaryWriter.Write(cinCam.frames.Length); // entry count
		binaryWriter.Write(24); // table offset

		Vector3 offset = new Vector3(0.0f, 0.0f, -2.0f);

		for (int i = 0; i < cinCam.frames.Length; i++)
		{
			CinCamFrame currentFrame = cinCam.frames[i];

			if (i >= firstFrameToTweak && i <= lastFrameToTweak)
			{
				// tweaked frame
				binaryWriter.Write(currentFrame.time);
				binaryWriter.Write(-(currentFrame.position.x + offset.x));
				binaryWriter.Write(currentFrame.position.y + offset.y);
				binaryWriter.Write(currentFrame.position.z + offset.z);
				binaryWriter.Write(-(currentFrame.target.x + offset.x));
				binaryWriter.Write(currentFrame.target.y + offset.y);
				binaryWriter.Write(currentFrame.target.z + offset.z);
			}
			else
			{
				// unchanged frame
				binaryWriter.Write(currentFrame.time);
				binaryWriter.Write(-currentFrame.position.x);
				binaryWriter.Write(currentFrame.position.y);
				binaryWriter.Write(currentFrame.position.z);
				binaryWriter.Write(-currentFrame.target.x);
				binaryWriter.Write(currentFrame.target.y);
				binaryWriter.Write(currentFrame.target.z);
			}
		}

		binaryWriter.Write(20); // offset
		binaryWriter.Write(1); // offset count
		binaryWriter.Write(0xC0FFEE); // footer

		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
	}
}

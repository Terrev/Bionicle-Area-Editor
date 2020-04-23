using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class TestCinCamInterpolator : MonoBehaviour
{
	public void Save()
	{
		CinCam cinCam = gameObject.GetComponent<CinCam>();
		if (cinCam == null)
		{
			Debug.LogError("CinCam component not found");
		}

		string path = EditorUtility.SaveFilePanel("Save SLB", "", cinCam.gameObject.name, "slb");
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
		long rememberMe = fileStream.Position;
		binaryWriter.Write(0); // temp entry count
		binaryWriter.Write(24); // table offset

		int framesWritten = 0;
		for (int i = 0; i < cinCam.frames.Length; i++)
		{
			CinCamFrame currentFrame = cinCam.frames[i];
			binaryWriter.Write(currentFrame.time);
			binaryWriter.Write(-currentFrame.position.x);
			binaryWriter.Write(currentFrame.position.y);
			binaryWriter.Write(currentFrame.position.z);
			binaryWriter.Write(-currentFrame.target.x);
			binaryWriter.Write(currentFrame.target.y);
			binaryWriter.Write(currentFrame.target.z);
			framesWritten++;

			if (i != cinCam.frames.Length - 1)
			{
				CinCamFrame nextFrame = cinCam.frames[i + 1];
				float time = Mathf.Lerp(currentFrame.time, nextFrame.time, 0.5f);
				Vector3 position = Vector3.Lerp(currentFrame.position, nextFrame.position, 0.5f);
				Vector3 target = Vector3.Lerp(currentFrame.target, nextFrame.target, 0.5f);

				binaryWriter.Write(time);
				binaryWriter.Write(-position.x);
				binaryWriter.Write(position.y);
				binaryWriter.Write(position.z);
				binaryWriter.Write(-target.x);
				binaryWriter.Write(target.y);
				binaryWriter.Write(target.z);
				framesWritten++;
			}
		}

		binaryWriter.Write(20); // offset
		binaryWriter.Write(1); // offset count
		binaryWriter.Write(0xC0FFEE); // footer

		fileStream.Seek(rememberMe, SeekOrigin.Begin);
		binaryWriter.Write(framesWritten);

		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
	}
}

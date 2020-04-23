using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TestCinCamExport : MonoBehaviour
{
	public CinCam cinCam;
	public int frameToStopAfter;
	public string animName;
	public float time = 0.0f;

	Animation anim;

	void Start()
	{
		anim = GetComponent<Animation>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			time = 0.0f;
		}
		else if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			time = 5.0f;
		}
		anim[animName].time = time / 2.0f; // I like the first attempt better, it's closer to the original, though it's at 60 FPS so this division by 2 is to account for that
		anim[animName].speed = 0.0f;
		anim.Play();
		time += Time.deltaTime;
	}

	public void Export()
	{
		anim = GetComponent<Animation>();

		string path = EditorUtility.SaveFilePanel("Save frames", "", animName + ".frames", "frames");
		if (path.Length == 0)
		{
			return;
		}

		FileStream fileStream = new FileStream(path, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);

		for (int i = 0; i < frameToStopAfter + 1; i++)
		{
			// Multiply/divide/whatever relevant things by 2 when exporting the 60 FPS one

			// Unity pls
			anim.Stop();
			anim.Play();
			anim[animName].time = cinCam.frames[i].time;
			anim[animName].speed = 0.0f;
			anim.Sample();

			binaryWriter.Write(cinCam.frames[i].time);

			binaryWriter.Write(-transform.position.x);
			binaryWriter.Write(transform.position.y);
			binaryWriter.Write(transform.position.z);

			binaryWriter.Write(215.383f);
			binaryWriter.Write(-399.995f);
			binaryWriter.Write(1899.4f);
		}

		binaryWriter.Close();
		fileStream.Close();
		Debug.Log("Saved " + path);
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Tester : MonoBehaviour
{
	// lazy test stuff goes here
	
	void Awake()
	{
		PosTest();
	}
	
	void ObjTest()
	{
		List<string> stringsToWrite = new List<string>();
		
		string[] paths = Directory.GetFiles(Application.dataPath + "/Resources", "*_OBJ.slb", SearchOption.AllDirectories);
		/*
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i]);
		}
		*/
		
		foreach (string path in paths)
		{
			string fileName = Path.GetFileName(path);
			if (fileName.StartsWith("cin"))
			{
				continue;
			}
			
			// lol
			//stringsToWrite.Add("\n=====================================================================\n" + path.Substring(40) + "\n=====================================================================");
			
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			
			// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
			binaryReader.BaseStream.Position = 8;
			
			// READ BASIC INFO
			UInt32 entryCount = binaryReader.ReadUInt32();
			UInt32 tableOffset = binaryReader.ReadUInt32();
			
			// GO TO BEGINNING OF TABLE
			fileStream.Seek(tableOffset, SeekOrigin.Begin);
			
			// LOOP THROUGH ENTRIES
			for (int i = 0; i < entryCount; i++)
			{
				// IDENTIFIER
				// read the characters then turn them into a string we can use more easily
				char[] charArray = new char[4];
				charArray = binaryReader.ReadChars(4);
				Array.Reverse(charArray);
				string identifier = new string(charArray);
				
				// LOCATION
				Vector3 location = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				
				// ORIENTATION
				Vector3 orientation = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				
				// UNKNOWN
				float unknown = binaryReader.ReadSingle();
				
				// COLLISION POINT 1
				Vector3 collisionPoint1 = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				
				// COLLISION POINT 2
				Vector3 collisionPoint2 = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				
				// FLAGS
				UInt32 flags = binaryReader.ReadUInt32();
				
				// comparing entire orientation vector3s missed lev4 maz2 main.x
				if (orientation.x != 0.0f || orientation.y != 0.0f || orientation.z != 0.0f)
				{
					// dumb
					string x;
					string y;
					string z;
					
					if (orientation.x == 0.0f)
					{
						x = " ";
					}
					else
					{
						x = "x";
					}
					
					if (orientation.y == 0.0f)
					{
						y = " ";
					}
					else
					{
						y = "y";
					}
					
					if (orientation.z == 0.0f)
					{
						z = " ";
					}
					else
					{
						z = "z";
					}
					
					// whatever
					string blah = path.Substring(40, 9) + "	" + identifier + "	" + orientation.x + "				" + orientation.y + "				" + orientation.z + "				" + x + " " + y + " " + z;
					Debug.Log(blah);
					stringsToWrite.Add(blah);
				}
				
				// lol
				//string lol = "\nNAME: " + identifier + "\nLOCATION: " + location + "\nORIENTATION: " + orientation + "\nUNKNOWN: " + unknown + "\nCOL 1: " + collisionPoint1 + "\nCOL 2: " + collisionPoint2 + "\nFLAGS: " + flags;
				//stringsToWrite.Add(lol);
			}
			// SHRUG
			binaryReader.Close();
			fileStream.Close();
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
		
		File.WriteAllLines(Application.dataPath + "/Output.txt", stringsToWrite.ToArray());
	}
	
	void PosTest()
	{
		List<string> stringsToWrite = new List<string>();
		
		string[] paths = Directory.GetFiles(Application.dataPath + "/Resources", "*_POS.slb", SearchOption.AllDirectories);
		
		foreach (string path in paths)
		{
			string fileName = Path.GetFileName(path);
			/*
			if (fileName.StartsWith("cin"))
			{
				continue;
			}
			*/
			
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			
			// SKIP UNNEEDED/UNUSED STUFF AT START OF FILE
			binaryReader.BaseStream.Position = 4;
			
			// READ BASIC INFO
			UInt32 entryCount = binaryReader.ReadUInt32();
			UInt32 tableOffset = binaryReader.ReadUInt32();
			
			// GO TO BEGINNING OF TABLE
			fileStream.Seek(tableOffset, SeekOrigin.Begin);
			
			// LOOP THROUGH ENTRIES
			for (int i = 0; i < entryCount; i++)
			{
				// IDENTIFIER
				// read the characters then turn them into a string we can use more easily
				char[] charArray = new char[4];
				charArray = binaryReader.ReadChars(4);
				Array.Reverse(charArray);
				string identifier = new string(charArray);
				
				// POSITION
				Vector3 position = new Vector3(-binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
				
				// FLAGS
				UInt32 flags = binaryReader.ReadUInt32();
				
				// looking for unusual position names, could still theoretically miss stuff if it's, idk, "arse" or something
				if
				(
					!identifier.StartsWith("str") && // start
					!identifier.StartsWith("lok") && // look
					!identifier.StartsWith("hs") &&  // hive start
					!identifier.StartsWith("et") &&  // token
					!identifier.StartsWith("ph") &&  // pickup health
					!identifier.StartsWith("pe") &&  // pickup energy
					!identifier.StartsWith("pa") &&  // pickup air
					!identifier.StartsWith("am") &&  // ammo
					!identifier.StartsWith("ar")     // arrow
				)
				{
					stringsToWrite.Add(path.Substring(40, 9) + " " + identifier + " " + flags);
				}
			}
			// SHRUG
			binaryReader.Close();
			fileStream.Close();
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}
		
		File.WriteAllLines(Application.dataPath + "/Output.txt", stringsToWrite.ToArray());
	}
}

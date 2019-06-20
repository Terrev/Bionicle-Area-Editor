using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
	public static string CharArrayToString(char[] charArray)
	{
		Array.Reverse(charArray);
		string identifier = new string(charArray);
		return identifier;
	}
	
	public static char[] StringToCharArray(string identifier)
	{
		char[] charArray = identifier.ToCharArray(0, 4);
		Array.Reverse(charArray);
		return charArray;
	}
	
	// get rid of -0s unity may introduce
	public static float DumbCheck(float input)
	{
		if (input == -0.0f)
		{
			return 0.0f;
		}
		else
		{
			return input;
		}
	}
	
	// clamp rotation values to avoid weird numbers - highest object rotation value in the vanilla game is 170, lowest is -90, unity sometimes gives values like -302.5 instead of the original 57.5
	public static float ClampRotation(float input)
	{
		if (input > 360.0f || input < -360.0f)
		{
			Debug.LogWarning("lol wtf is this rotation, lemme know if you see this");
			return input;
		}
		if (input > 180.0f)
		{
			return input - 360.0f;
		}
		if (input < -180.0f)
		{
			return input + 360.0f;
		}
		return input;
	}
}

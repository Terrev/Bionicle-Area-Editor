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
}

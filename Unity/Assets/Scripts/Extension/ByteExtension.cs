using UnityEngine;
using System.Collections;

public static class ByteExtension
{
	#region STATIC_METHODS
	
	public static byte Add(params byte[] aNumbers)
	{
		int result = 0;
		
		for(int i = 0;i<aNumbers.Length;i++)
		{
			result += aNumbers[i];
		}
		
		//
		return (byte)(Mathf.Clamp (result,0,255));
	}
	
	#endregion
	
	#region EXTENSION_METHODS
	
	public static byte Substract(this byte nb,params byte[] aNumbers)
	{
		int result = nb;
		
		for(int i = 0;i<aNumbers.Length;i++)
		{
			result -= aNumbers[i];
		}
		
		//
		return (byte)(Mathf.Clamp (result,0,255));
	}
	
	#endregion
}

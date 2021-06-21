using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions {
	
	public static List<Vector2SerializationInfo> Serialize(this List<Vector2> aList)
	{
		List<Vector2SerializationInfo> returnValue = null;
		
		if (aList != null)
		{
			returnValue = new List<Vector2SerializationInfo>(aList.Count);
			
			foreach(Vector2 v in aList)
			{
				returnValue.Add(new Vector2SerializationInfo(v));
			}
		}
		
		return returnValue;
	}
	
	public static List<Vector3SerializationInfo> Serialize(this List<Vector3> aList)
	{
		List<Vector3SerializationInfo> returnValue = null;
		
		if (aList != null)
		{
			returnValue = new List<Vector3SerializationInfo>(aList.Count);
			
			foreach(Vector3 v in aList)
			{
				returnValue.Add(new Vector3SerializationInfo(v));
			}
		}
		
		return returnValue;
	}
	
	public static List<Vector2> Deserialize(this List<Vector2SerializationInfo> aList)
	{
		List<Vector2> returnValue = null;
		
		if (aList != null)
		{
			returnValue = new List<Vector2>(aList.Count);
			
			foreach(Vector2SerializationInfo v in aList)
			{
				returnValue.Add(new Vector2(v.x,v.y));
			}
		}
		
		return returnValue;
	}
	
	public static List<Vector3> Deserialize(this List<Vector3SerializationInfo> aList)
	{
		List<Vector3> returnValue = null;
		
		if (aList != null)
		{
			returnValue = new List<Vector3>(aList.Count);
			
			foreach(Vector3SerializationInfo v in aList)
			{
				returnValue.Add(new Vector3(v.x,v.y,v.z));
			}
		}
		
		return returnValue;
	}
	
}

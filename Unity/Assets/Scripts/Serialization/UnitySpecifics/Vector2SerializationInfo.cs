using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector2SerializationInfo 
{
	#region PRIVATE_MEMBERS
	
	//	
	public float x;
	public float y;
	
	#endregion
	
	public Vector2SerializationInfo(float x,float y)
	{
		this.x = x;
		this.y = y;
	}
	
	public Vector2SerializationInfo(Vector2 aVector)
	{
		this.x = aVector.x;
		this.y = aVector.y;
	}
}

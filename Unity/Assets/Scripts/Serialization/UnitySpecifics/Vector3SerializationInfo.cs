using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector3SerializationInfo 
{
	#region PRIVATE_MEMBERS
	
	//	
	public float x;
	public float y;
	public float z;
	
	#endregion
	
	public Vector3SerializationInfo(float x,float y,float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public Vector3SerializationInfo(Vector3 aVector)
	{
		this.x = aVector.x;
		this.y = aVector.y;
		this.z = aVector.z;
	}
}

using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class ResearchLabSerializationInfo : WorkingRoomSerializationInfo {
	
	#region PUBLIC_MEMBERS
	
	public float mSearchTime;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public ResearchLabSerializationInfo(ResearchLab aRoom) :base(aRoom)
	{
		mSearchTime = aRoom.SearchTime;
	}
	
	#endregion
}

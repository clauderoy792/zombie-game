using UnityEngine;
using System.Collections;
using HCUtils;

[System.Serializable]
public class ConstructionSerializationInfo : RoomSerializationInfo {

	public Timer mProgressTimer; 
	public ERoomType mRoomToConstruct;

	public ConstructionSerializationInfo(Construction aConstruction) : base(aConstruction)
	{
		mProgressTimer 		= aConstruction.ProgressTimer;
		mRoomToConstruct 	= aConstruction.RoomToConstruct;
	}
}

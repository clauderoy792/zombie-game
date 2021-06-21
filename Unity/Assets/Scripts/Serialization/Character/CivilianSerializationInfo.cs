using UnityEngine;
using System.Collections;
using HCUtils;

[System.Serializable]
public class CivilianSerializationInfo : HumanSerializationInfo {

	#region PUBLIC_MEMBERS
	
	public bool mIsWaitingInReception;
	public bool mInitiallyMovedTowardReception;
	public Vector2SerializationInfo mWaitingPosition;
	public RandomTimer mGoingHomeTimer;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public CivilianSerializationInfo(Civilian aCivilian) : base(aCivilian)
	{
		mGoingHomeTimer 		= aCivilian.GoingHomeTimer;
		
		//
		mInitiallyMovedTowardReception = aCivilian.InitiallyMovedTowardReception;
		
		//
		mIsWaitingInReception 	= aCivilian.IsWaitingAtReception;
		
		//
		mWaitingPosition 		= new Vector2SerializationInfo(aCivilian.WaitingPosition);
	}
	#endregion
}

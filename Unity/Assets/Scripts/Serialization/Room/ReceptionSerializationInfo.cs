using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class ReceptionSerializationInfo : RoomSerializationInfo  {
	
	#region PUBLIC_MEMBERS
	
	public float mRelativeStartPositionX;
	public float mWaitingPositionX;
	public int mMaxNbWaitingCivilian;
	public int mCurrentNbCivilian;
	public int  mNbSpawnedCivilian;
	public List<Vector2SerializationInfo> mWaitingPositions;
	public List<int> mGoingHomeCharacters;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public ReceptionSerializationInfo(Reception aRoom) : base(aRoom)
	{
		//TODO Add chars in line
		
		mWaitingPositionX 		= aRoom.WaitingPositionX;
		mMaxNbWaitingCivilian 	= aRoom.MaxNbWaitingCivilian;
		mCurrentNbCivilian		= aRoom.CurrentNbWaitingCivilian;
		mRelativeStartPositionX = aRoom.RelativeStartPositionX;
		mNbSpawnedCivilian		= aRoom.NbSpawnedCivilian;
		mWaitingPositions		= aRoom.WaitingPositions.Serialize();
		
		if (aRoom.GoingHomeCharacters != null && aRoom.GoingHomeCharacters.Count > 0)
		{
			mGoingHomeCharacters = new List<int>();
			
			foreach(Character c in aRoom.GoingHomeCharacters)
			{
				mGoingHomeCharacters.Add(c.ID);
			}
		}
		
	}
	
	#endregion
}

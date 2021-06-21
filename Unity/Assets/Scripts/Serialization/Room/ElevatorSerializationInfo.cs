using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class ElevatorSerializationInfo : RoomSerializationInfo {
	
	#region PUBLIC_MEMBERS
	
	public bool mIsPlayingOpenDoors;
	public bool mIsPlayingCloseDoors;
	public float mAnimTime;
	public Vector3SerializationInfo mLeftDoorPos;
	public Vector3SerializationInfo mRightDoorPos;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public ElevatorSerializationInfo(Elevator aRoom) : base(aRoom)
	{
		if(aRoom.Animation.IsPlaying(Elevator.CLOSE_DOORS_ANIM))
		{
			mIsPlayingCloseDoors = true;
			mAnimTime = aRoom.Animation[Elevator.CLOSE_DOORS_ANIM].time;
		}
		else if(aRoom.Animation.IsPlaying(Elevator.OPEN_DOORS_ANIM))
		{
			mIsPlayingOpenDoors = true;
			mAnimTime = aRoom.Animation[Elevator.OPEN_DOORS_ANIM].time;
		}
		else
		{
			mLeftDoorPos = new Vector3SerializationInfo(aRoom.mLeftDoor.localPosition);
			mRightDoorPos = new Vector3SerializationInfo(aRoom.mRightDoor.localPosition);
		}
	}
	
	#endregion
}

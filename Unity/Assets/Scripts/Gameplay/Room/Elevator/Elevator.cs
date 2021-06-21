using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Elevator : Room 
{
	//
	Animation mAnim;
	public Transform mLeftDoor;
	public Transform mRightDoor;
	public SpriteRenderer mUpLight;
	public SpriteRenderer mDownLight;
	public Sprite lightOff;
	public Sprite  lightOn;
	public const string OPEN_DOORS_ANIM = "ElevatorOpenDoors";
	public const string CLOSE_DOORS_ANIM = "ElevatorCloseDoors";
	public const float DOOR_SIZE 		= 0.3125f;
	
	#region ACCESSORS
	
	public Animation Animation
	{
		get{return mAnim;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();
		mAnim = GetComponent<Animation>();
	}
	
	#endregion
	
	//
	public void SetElevatorState(EElevatorState aState)
	{
		//TODO: HG
		if(aState == EElevatorState.Idle)	
		{
			mUpLight.sprite = lightOff;
			mDownLight.sprite = lightOff;
		}
		else if(aState == EElevatorState.Going_Up)
		{
			mUpLight.sprite = lightOn;
			mDownLight.sprite = lightOff;
		}
		else
		{
			mUpLight.sprite = lightOff;
			mDownLight.sprite = lightOn;
		}
	}
	
	//
	public void OpenDoors()
	{
		mAnim.Play(OPEN_DOORS_ANIM);
	}
	
	//
	public void CloseDoors()
	{
		mAnim.Play(CLOSE_DOORS_ANIM);
	}
	
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new ElevatorSerializationInfo(this);
	}
	
	public override void Deserialize (RoomSerializationInfo aInfo)
	{
		base.Deserialize(aInfo);
		
		ElevatorSerializationInfo info = aInfo as ElevatorSerializationInfo;
		
		//
		if (info.mIsPlayingCloseDoors)
		{
			mAnim[CLOSE_DOORS_ANIM].time = info.mAnimTime;
			mAnim.Play(CLOSE_DOORS_ANIM);
		}
		else if(info.mIsPlayingOpenDoors)
		{
			mAnim[OPEN_DOORS_ANIM].time = info.mAnimTime;
			mAnim.Play(OPEN_DOORS_ANIM);
		}
		else
		{
			mLeftDoor.localPosition = new Vector3(info.mLeftDoorPos.x,info.mLeftDoorPos.y,info.mLeftDoorPos.z);
			mRightDoor.localPosition = new Vector3(info.mRightDoorPos.x,info.mRightDoorPos.y,info.mRightDoorPos.z);
		}
	}
	
	#endregion
}

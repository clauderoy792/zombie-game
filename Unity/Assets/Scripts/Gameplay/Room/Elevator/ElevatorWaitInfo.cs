using UnityEngine;
using System.Collections;

 [System.Serializable]
public struct ElevatorWaitInfo
{
	#region PUBLIC_MEMBERS
	
	public bool mIsGoingUp;
	public int mAtFloor;
	public int mGoingToFloor;
	public int mCharacterId;

	#endregion
	
	#region CONSTRUCTOR
	
	public ElevatorWaitInfo(int aCharId,int aAtFloor,int aGoingToFloor)
	{
		//
		mCharacterId = aCharId;
		mAtFloor = aAtFloor;
		mGoingToFloor = aGoingToFloor;
		mIsGoingUp = mGoingToFloor > mAtFloor;
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public void UpdateGoingToFloor(int aGoingToFloor)
	{
		mGoingToFloor = aGoingToFloor;
		mIsGoingUp = mGoingToFloor > mAtFloor;
	}

	public override string ToString ()
	{
		return string.Format("Char id : {0}, at floor : {1}, going to floor : {2}",mCharacterId,mAtFloor,mGoingToFloor);
	}

	#endregion
}

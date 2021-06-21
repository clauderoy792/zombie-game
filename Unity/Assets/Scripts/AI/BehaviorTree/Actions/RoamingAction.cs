using UnityEngine;
using System.Collections;

public class RoamingAction : Behavior
{
	Room mTargetRoom;
	Vector3 mGoingToPosition;
	
	public RoamingAction(Character aCharacter) : base (aCharacter)
	{
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{	
		if(mStatus == BH_Status.RUNNING)
		{
			if(mCharacter.LocalTransformPosition.x == mGoingToPosition.x && mCharacter.LocalTransformPosition.y == mGoingToPosition.y)
			{
				return BH_Status.SUCCESS;
			}
			return BH_Status.RUNNING;
		}
		else
		{
			//Prevent Zombies from going on other floors.
			if (mCharacter is Zombie)
			{
				mTargetRoom = RoomManager.Instance.GetRandomRoomOnSameFloor(mCharacter);
			}
			else
			{
				mTargetRoom = RoomManager.Instance.GetRandomRoom(mCharacter);
			}
			
			if(mTargetRoom != null)
			{
				mCharacter.RandomMovementInRoom();
				return BH_Status.RUNNING;
			}
		}
		
		return BH_Status.FAILURE;
	}
}

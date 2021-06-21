using UnityEngine;
using System.Collections;

public class MoveToToiletDoor : Behavior 
{
	Bathroom mBathroom = null;
	int mToiletID = 0;
	Vector2 mToiletPosition = Vector2.zero;
	
	public Bathroom Toilet {
		get {
			return this.mBathroom;
		}
	}	
	
	public MoveToToiletDoor(Human aCharacter) : base (aCharacter)
	{
		
	}
	
	public override void OnInitialize ()
	{
		mBathroom = null;
		mToiletID = 0;
		mToiletPosition = Vector2.zero;
	}
	
	public override BH_Status Update ()
	{
		if(mStatus == BH_Status.RUNNING)
		{
			if(mCharacter.Transform.position.x == mToiletPosition.x)
			{
				//
				mBathroom.ChangeToiletDoorState(mToiletID, false);
				
				//
				mCharacter.Hide();
				
				//
				return BH_Status.SUCCESS;
			}
			else
			{
				return BH_Status.RUNNING;
			}
		}
		
		if(mCharacter.CurrentRoom.Type == ERoomType.Bathroom)
		{
			if(mCharacter.Behavior is HumanBehavior)
			{
				mBathroom = (mCharacter.Behavior as HumanBehavior).GoToToiletAction.Toilet;
				mToiletID = (mCharacter.Behavior as HumanBehavior).WaitForAvailableToiletAction.ToiletID;
			}
			else
			{
				mBathroom = (mCharacter.Behavior as RoamingBehavior).GoToToiletAction.Toilet;
				mToiletID = (mCharacter.Behavior as RoamingBehavior).WaitForAvailableToiletAction.ToiletID;
			}
			
			//
			mToiletPosition = mBathroom.GetToiletPosition(mToiletID);
			
			mCharacter.MoveToLocalPoint(mToiletPosition);
			
			return BH_Status.RUNNING;
		}

		return BH_Status.FAILURE;
	}
}
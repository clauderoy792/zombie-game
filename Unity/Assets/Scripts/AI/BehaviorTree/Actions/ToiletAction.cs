using UnityEngine;
using System.Collections;

public class ToiletAction : Behavior 
{
	Bathroom mBathroom;
	int mToiletID;
	
	public ToiletAction(Human aCharacter) : base (aCharacter)
	{
	}
	
	public override void OnInitialize ()
	{
		mBathroom = null;
		mToiletID = 0;
	}
	
	public override BH_Status Update ()
	{
		if(mStatus == BH_Status.RUNNING)
		{
			(mCharacter as Human).Stats.Toilet += 2;
			
			if((mCharacter as Human).Stats.Toilet < 100)
			{
				return BH_Status.RUNNING;
			}
			else
			{
				mCharacter.Unhide();
				mBathroom.ChangeToiletDoorState(mToiletID, true);
				mBathroom.ReleaseToiletReservation(mToiletID);
				mBathroom.RemoveCharacter(mCharacter);
				return BH_Status.SUCCESS;
			}
		}
		
		if((mCharacter as Human).Stats.Toilet < 10)
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
			
			return BH_Status.RUNNING;
		}
		
		// Show character and remove from list if this task is failing.
		mCharacter.Unhide();
		mBathroom.RemoveCharacter(mCharacter);
		
		return BH_Status.FAILURE;
	}
}

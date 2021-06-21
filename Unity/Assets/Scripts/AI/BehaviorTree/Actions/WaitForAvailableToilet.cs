using UnityEngine;
using System.Collections;

public class WaitForAvailableToilet : Behavior 
{
	Bathroom mBathroom = null;
	int mToiletID = 0;
	
	
	public WaitForAvailableToilet(Human aCharacter) : base (aCharacter)
	{
	}

	public Bathroom Bathroom {
		get {
			return this.mBathroom;
		}
	}

	public int ToiletID {
		get {
			return this.mToiletID;
		}
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
			if(mBathroom.HasAvailableToilet())
			{
				mToiletID = mBathroom.GetAvailableToiletID();
				mBathroom.ReserveToilet(mToiletID);
				return BH_Status.SUCCESS;
			}
			
			return BH_Status.RUNNING;
		}
		
		if(mCharacter.CurrentRoom.Type == ERoomType.Bathroom)
		{
			if(mCharacter.Behavior is HumanBehavior)
			{
				mBathroom = (mCharacter.Behavior as HumanBehavior).GoToToiletAction.Toilet;
			}
			else
			{
				mBathroom = (mCharacter.Behavior as RoamingBehavior).GoToToiletAction.Toilet;
			}
			
			if(!mBathroom.HasAvailableToilet())
			{
				mCharacter.RandomMovementInRoom();
			}
			
			//
			if(!mBathroom.CharactersUsingRoom.Contains(mCharacter))
			{
				mBathroom.AddCharacter(mCharacter);
			}
			
			return BH_Status.RUNNING;
		}
		
		return BH_Status.FAILURE;
	}
}
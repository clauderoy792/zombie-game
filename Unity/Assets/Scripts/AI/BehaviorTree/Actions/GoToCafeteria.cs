using UnityEngine;
using System.Collections;

public class GoToCafeteria : Behavior 
{
	Room mCafeteria = null;
	
	public GoToCafeteria(Human aCharacter) : base (aCharacter)
	{
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{	
		if(mCharacter.CurrentRoom == mCafeteria)
		{
			if(mStatus == BH_Status.RUNNING)
			{
				mCharacter.RandomMovementInRoom();
			}
			return BH_Status.SUCCESS;
		}
		
		if(mStatus == BH_Status.RUNNING)
		{
			return BH_Status.RUNNING;
		}
		else
		{
			if((mCharacter as Human).Stats.Hungriness < 20f)
			{
				mCafeteria = mCharacter.MoveToRoom(ERoomType.Cafeteria);
				
				if(mCafeteria != null)
				{
					return BH_Status.RUNNING;
				}
			}
		}

		return BH_Status.FAILURE;
	}
}
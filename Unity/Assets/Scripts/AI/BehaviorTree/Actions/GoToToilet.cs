using UnityEngine;
using System.Collections;

public class GoToToilet : Behavior 
{
	Bathroom mToilet = null;

	public Bathroom Toilet {
		get {
			return this.mToilet;
		}
	}	
	
	
	public GoToToilet(Human aCharacter) : base (aCharacter)
	{
	}
	
	public override void OnInitialize ()
	{
		mToilet = null;
	}
	
	public override BH_Status Update ()
	{	
		if(mToilet != null && mCharacter.CurrentRoom == mToilet)
		{
			return BH_Status.SUCCESS;
		}
		
		if(mStatus == BH_Status.RUNNING)
		{
			return BH_Status.RUNNING;
		}
		else
		{
			if((mCharacter as Human).Stats.Toilet < 10f)
			{
				mToilet = mCharacter.MoveToRoom(ERoomType.Bathroom) as Bathroom;
				
				if(mToilet != null)
				{
					return BH_Status.RUNNING;
				}
			}
		}

		return BH_Status.FAILURE;
	}
}
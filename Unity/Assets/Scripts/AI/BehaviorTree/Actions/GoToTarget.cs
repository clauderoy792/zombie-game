using UnityEngine;
using System.Collections;

public class GoToTarget : Behavior 
{
	IRoaming mRoamingCharacter;

	public GoToTarget(Character aCharacter) : base (aCharacter)
	{
		mRoamingCharacter = (mCharacter as IRoaming);
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{	
		if(mStatus == BH_Status.RUNNING)
		{
			if(mCharacter is Zombie)
			{
				// Get the nearest 
				if(mRoamingCharacter.HasTarget())
				{
					mRoamingCharacter.GoToTarget();
				}
				else
				{
					return BH_Status.FAILURE;
				}
			}

			if(mRoamingCharacter.IsAtTarget())
			{
				mRoamingCharacter.DoTargetAction();
				return BH_Status.SUCCESS;
			}
			return BH_Status.RUNNING;
		}
		else
		{
			if(mRoamingCharacter.HasTarget())
			{
				mRoamingCharacter.GoToTarget();
				return BH_Status.RUNNING;
			}
		}
		
		return BH_Status.FAILURE;
	}
}
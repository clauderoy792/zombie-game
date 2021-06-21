using UnityEngine;
using System.Collections;

public class EatAction : Behavior 
{
	Human mHuman = null;

	public EatAction(Human aCharacter) : base (aCharacter)
	{
		mHuman = aCharacter as Human;
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{	
		if(mStatus == BH_Status.RUNNING)
		{
			mHuman.Stats.Hungriness += 1f;
		
			if(mHuman.Stats.Hungriness < 100)
			{
				return BH_Status.RUNNING;
			}
			else
			{
				return BH_Status.SUCCESS;
			}
		}
		
		if(mHuman.Stats.Hungriness < 20f)
		{
			return BH_Status.RUNNING;
		}
		
		return BH_Status.FAILURE;
	}
}

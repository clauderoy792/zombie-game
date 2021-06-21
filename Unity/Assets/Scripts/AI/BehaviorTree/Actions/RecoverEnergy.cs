using UnityEngine;
using System.Collections;

public class RecoverEnergy : Behavior 
{
	Human mOwner;
	
	public RecoverEnergy(Human aCharacter) : base (aCharacter)
	{
		mOwner = aCharacter;
	}
	
	public override void OnInitialize ()
	{

	}
	
	public override BH_Status Update ()
	{	
		if(mOwner.Stats.Exhaustion == 100 && mOwner.Stats.Hungriness == 100 && mOwner.Stats.Toilet == 100)
		{
			mOwner.Unhide();
			return BH_Status.SUCCESS;
		}
		else
		{
			mOwner.Stats.Exhaustion += 1;
			mOwner.Stats.Hungriness += 2;
			mOwner.Stats.Toilet += 5;
			
			return BH_Status.RUNNING;
		}
		
		//return BH_Status.FAILURE;
	}
}

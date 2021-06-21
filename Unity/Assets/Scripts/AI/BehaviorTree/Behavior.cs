using UnityEngine;
using System.Collections;

public enum BH_Status
{
	READY,
	SUCCESS,
	FAILURE,
	RUNNING,
}

public abstract class Behavior 
{
	protected BH_Status mStatus;
	protected Character mCharacter;
	
	public Behavior(Character aCharacter)
	{
		mCharacter = aCharacter;
	}

	public abstract BH_Status Update();
	public abstract void OnInitialize();
	public virtual void OnTerminate(BH_Status aStatus) 
	{ 
		mStatus = aStatus; 
	}
	
	public BH_Status Tick()
	{
		if(mStatus == BH_Status.READY)
		{
			OnInitialize();
		}
		
		BH_Status result = mStatus = Update();
		
		if(mStatus != BH_Status.RUNNING)
		{
			OnTerminate(BH_Status.READY);
		}
		
		return result;
	}
}

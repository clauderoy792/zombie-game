using UnityEngine;
using System.Collections;

public class GoToWork : Behavior 
{
	Human character = null;

	public GoToWork(Human aCharacter) : base (aCharacter)
	{
		character = aCharacter as Human;
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{	
		if(!character.IsWorking && !character.IsStartingWorkOnNextMovement && character.JobStats.WorkStation != null && character.JobStats.WorkStation == mCharacter.CurrentRoom && HCMath.AlmostEqual(character.LocalTransformPosition.x,character.CurrentRoom.LocalTransformPosition.x+Room.UNIT_CELL_WIDTH/2.0f))
		{
			character.SetReadyToWork();
			character.RandomMovementInRoom();
		
			return BH_Status.SUCCESS;
		}

		if(mStatus == BH_Status.RUNNING)
		{
			return BH_Status.RUNNING;
		}
		else if (!character.IsWorking && !character.IsStartingWorkOnNextMovement && character.JobStats.WorkStation != null)
		{
			character.MoveToRoom(character.JobStats.WorkStation);
			return BH_Status.RUNNING;
		}
		
		return BH_Status.FAILURE;
	}
}
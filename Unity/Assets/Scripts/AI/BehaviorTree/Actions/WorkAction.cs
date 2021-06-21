using UnityEngine;
using System.Collections;

public class WorkAction : Behavior 
{
	public WorkAction(Human aCharacter) : base (aCharacter)
	{
	}
	
	public override void OnInitialize ()
	{
		
	}
	
	public override BH_Status Update ()
	{
		return BH_Status.SUCCESS;
	}
}

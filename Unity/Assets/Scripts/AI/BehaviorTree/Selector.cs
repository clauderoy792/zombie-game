using UnityEngine;
using System.Collections;

public class Selector : ParentBehavior
{
	public Behavior mCurrentChild;
	
	public Selector(Character aCharacter) : base(aCharacter)
	{
		
	}
	
	public override void OnInitialize ()
	{
		mCurrentChild = mChildren[0];
	}
	
	public override BH_Status Update ()
	{
		int index = mChildren.IndexOf(mCurrentChild);
		
		while(true)
		{
			BH_Status s = mCurrentChild.Tick();
			
			// if the child succeeds, or keep running, do the same
			if(s != BH_Status.FAILURE)
			{
				return s;
			}
			
			// Hit the end of the array
			if(index == mChildren.Count -1)
			{
				return BH_Status.FAILURE;
			}
			
			index++;
			mCurrentChild = mChildren[index];
		}
	}
}

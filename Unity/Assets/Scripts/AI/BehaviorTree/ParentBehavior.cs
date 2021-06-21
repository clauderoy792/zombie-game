using UnityEngine;
using System.Collections.Generic;

public abstract class ParentBehavior : Behavior
{
	protected List<Behavior> mChildren = new List<Behavior>();
	
	public ParentBehavior(Character aCharacter) : base(aCharacter)
	{
		
	}
	
	//
	public void AddChild(Behavior aBehavior)	
	{
		mChildren.Add(aBehavior);
	}
}

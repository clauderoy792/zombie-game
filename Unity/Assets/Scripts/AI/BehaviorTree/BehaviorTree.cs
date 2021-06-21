using UnityEngine;
using System.Collections;

public abstract class BehaviorTree 
{
	protected Selector mRoot = null;
	protected Character mCharacter = null;
	
	public BehaviorTree(Character aCharacter)
	{
		mCharacter = aCharacter;
		mRoot = new Selector(aCharacter);
	}
	
	protected abstract void InitializeTree();
	
	public void Tick()
	{
		if(mRoot != null)
		{
			mRoot.Tick();
		}
	}
}

using UnityEngine;
using System.Collections;

public class Janitor : Human, IRoaming
{
	#region PRIVATE_MEMBERS
	
	Transform mTarget = null;
	
	#endregion
	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new RoamingBehavior(this);
		mDropJunkChance = 0;
	}
	
	#endregion
	
	#region INTERFACES_METHODS
	
	public bool HasTarget ()
	{
		//If we don't already have a target
		if (mTarget == null)
		{
			mTarget = TargetManager.Instance.GetNearestJunk(this);
		}
		return (mTarget != null);
	}
	
	public bool IsAtTarget ()
	{
		return (mTarget != null && new Vector2(mTarget.position.x, mTarget.position.y) == new Vector2(Transform.position.x, transform.position.y));
	}
	
	public void GoToTarget ()
	{
		MoveToLocalPoint( new Vector2( mTarget.position.x, mTarget.position.y+Room.UNIT_CELL_HEIGHT) );
	}
	
	public void DoTargetAction ()
	{
		if (CurrentRoom != null)
		{
			mCurrentRoom.RemoveJunk();
		}
		Destroy(mTarget.gameObject);
		mTarget = null;
	}
	
	#endregion
	
	#region ACCESSORS
	
	public Transform Junk
	{
		get{return mTarget;}
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public override CharacterSerializationInfo Serialize ()
	{
		return new JanitorSerializationInfo(this);
	}
	
	public override void Deserialize (CharacterSerializationInfo aInfo)
	{
		base.Deserialize (aInfo);
		
		JanitorSerializationInfo info = aInfo as JanitorSerializationInfo;
		
		if (info.mCurrentJunkX != float.MinValue && info.mCurrentJunkY != float.MinValue &&
			info.mCurrentJunkZ != float.MinValue)
		{
			TargetManager.Instance.CreateJunk(info.mCurrentJunkX,info.mCurrentJunkY,info.mCurrentJunkZ);
		}
	}
	
	#endregion
}

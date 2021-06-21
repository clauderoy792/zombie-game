using UnityEngine;
using System.Collections;

public class SecurityGuard : Human, IRoaming
{
	Transform mTarget = null;
	
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new RoamingBehavior(this);
	}
	
	public bool HasTarget ()
	{
		mTarget = TargetManager.Instance.GetNearestZombie(this);
		return (mTarget != null);
	}
	
	public bool IsAtTarget ()
	{
		return (mTarget != null && mTarget.position == Transform.position);
	}
	
	public void GoToTarget ()
	{
		
	}
	
	public void DoTargetAction ()
	{
		
	}
}

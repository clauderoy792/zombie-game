using UnityEngine;
using System.Collections;

public class Worker : Human
{
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new HumanBehavior(this);
	}
	
	#region WORKING_MANAGEMENT
	
	public override void StartWorking ()
	{
		base.StartWorking ();
	}
	 
	#endregion
}

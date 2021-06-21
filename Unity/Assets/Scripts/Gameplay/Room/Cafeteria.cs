using UnityEngine;
using System.Collections;

public class Cafeteria : Room {

	
	#region OVERRIDEN_METHODS
	
	protected override void Start ()
	{
		base.Start ();
		
		mDirtinessMultiplier = 2;
	}
	
	protected override void Update ()
	{
		base.Update();
		
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new CafeteriaSerializationInfo(this);
	}
	
	#endregion
}

using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class AssemblyLineSerializationInfo : WorkingRoomSerializationInfo  {
	
	#region PUBLIC_MEMBERS
	
	public float mProducingRoomX;
	public float mProducingRoomY;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public AssemblyLineSerializationInfo(AssemblyLine aRoom) : base(aRoom)
	{
		if (aRoom.ProductionRoom != null)
		{
			mProducingRoomX = aRoom.ProductionRoom.GridPosition.x;
			mProducingRoomY = aRoom.ProductionRoom.GridPosition.y;
		}
		else
		{
			//We don't have a production room
			mProducingRoomX = -1;
			mProducingRoomY = -1;
		}
	}
	
	#endregion
}

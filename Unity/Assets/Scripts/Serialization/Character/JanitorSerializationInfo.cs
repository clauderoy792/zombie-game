using UnityEngine;
using System.Collections;

[System.Serializable]
public class JanitorSerializationInfo : HumanSerializationInfo {

	#region PUBLIC_MEMBERS
	
	public float mCurrentJunkX;
	public float mCurrentJunkY;
	public float mCurrentJunkZ;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public JanitorSerializationInfo(Janitor aJanitor) : base(aJanitor)
	{
		//
		mCurrentJunkX = float.MinValue;
		mCurrentJunkY = float.MinValue;
		mCurrentJunkZ = float.MinValue;

		if (aJanitor.Junk != null)
		{
			//
			mCurrentJunkX = aJanitor.Junk.position.x;
			mCurrentJunkY = aJanitor.Junk.position.y;
			mCurrentJunkZ = aJanitor.Junk.position.z;
		}
	}
	
	#endregion
}

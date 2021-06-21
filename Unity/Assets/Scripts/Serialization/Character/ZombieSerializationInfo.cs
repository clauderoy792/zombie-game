using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ZombieSerializationInfo : CharacterSerializationInfo {

	#region MEMBERS				
	
	public ZombieStats mStats;
	
	#endregion
	
	#region CONSTRUCTORS
	
	public ZombieSerializationInfo(Zombie aZombie) : base(aZombie)
	{
		mStats = aZombie.Stats;
	}
	
	#endregion
}

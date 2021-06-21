using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieWarehouse : Room {
	
	#region PRIVATE_MEMBERS
	
	private int mNbZombies;
	private int mMaxNbZombies = 50;
	
	#endregion
	
	#region ACCESSORS
	
	public int CurrentNbZombies
	{
		get {return mNbZombies;}
	}
	
	public int MaxNbZombies
	{
		get{return mMaxNbZombies;}
	}
	
	#endregion
	
	
	#region ZOMBIE_PRODUCTION
	
	public void SpawnZombie()
	{
		GameObject zombie = GraphicsManager.Instance.GetRandomZombie();
		
		Zombie z = zombie.GetComponent<Zombie>();
		
		z.SetRoom(this);
		
		z.RandomMovementInRoom();
	}
	
	public void AddZombies(int aNbZombies)
	{
		if (aNbZombies > 0)
		{
			mNbZombies += aNbZombies;
		}
	}
	
	#endregion
	
	#region OVERRIDEN_METHODS
	
	public override bool IsRoomFull ()
	{
		return mNbZombies >= mMaxNbZombies;	
	}
	
	#endregion
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new ZombieWarehouseSerializationInfo(this);
	}
	
	#endregion
}

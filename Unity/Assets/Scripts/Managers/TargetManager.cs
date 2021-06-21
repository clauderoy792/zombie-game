using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TargetManager
{
	#region STATIC_MEMBERS

	private static TargetManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS
	
	List<Transform> mJunkQueue = null;
	List<Transform> mZombieQueue = null;
	
	ReadOnlyCollection<Transform> mJunkQueueReadOnly;
	ReadOnlyCollection<Transform> mZombieQueueReadOnly;
	
	#endregion

	#region ACCESSORS

	public static TargetManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new TargetManager();
			}

			return sInstance;
		}
	}
	
	public ReadOnlyCollection<Transform> JunkQueue
	{
		get{return mJunkQueueReadOnly;}
	}
	
	public ReadOnlyCollection<Transform> ZombieQueue
	{
		get{return mZombieQueueReadOnly;}
	}
	#endregion

	#region CONSTRUCTORS

	private TargetManager ()
	{
		mJunkQueue = new List<Transform>();
		mZombieQueue = new List<Transform>();
		
		//
		mJunkQueueReadOnly = mJunkQueue.AsReadOnly();
		mZombieQueueReadOnly = mZombieQueue.AsReadOnly();
	}

	#endregion
	
	#region PUBLIC METHODS
	
	/// <summary>
	/// Create and adds the junk to the junk queue.
	/// </summary>
	/// <param name='aJunkTransform'>
	/// A junk transform.
	/// </param>
	public void AddJunk(Transform aJunkTransform)
	{
		mJunkQueue.Add(aJunkTransform);
	}
	
	/// <summary>
	/// Create and adds the junk to the junk queue.
	/// </summary>
	/// <param name='aX'>
	/// A x.
	/// </param>
	/// <param name='aY'>
	/// A y.
	/// </param>
	/// <param name='aZ'>
	/// A z.
	/// </param>
	public void AddJunk(float aX,float aY,float aZ)
	{
		Transform junk = GraphicsManager.Instance.GetJunk().transform;
		
		//
		junk.position = new Vector3(aX,aY,aZ);

		//
		mJunkQueue.Add(junk);
	}
	
	/// <summary>
	/// Creates a junk without adding it to the junk queue.
	/// </summary>
	/// <param name='aX'>
	/// A x.
	/// </param>
	/// <param name='aY'>
	/// A y.
	/// </param>
	/// <param name='aZ'>
	/// A z.
	/// </param>
	public void CreateJunk(float aX,float aY,float aZ)
	{
		Transform junk = GraphicsManager.Instance.GetJunk().transform;
		
		//
		junk.position = new Vector3(aX,aY,aZ);
	}
	
	//
	public void AddZombie(Transform aZombieTransform)
	{
		mZombieQueue.Add(aZombieTransform);
	}
	
	//
	public Transform GetNearestJunk(Character aCharacter)
	{
		return GetNearestTarget(mJunkQueue, aCharacter);
	}
	
	//
	public Transform GetNearestZombie(Character aCharacter)
	{
		return GetNearestTarget(mZombieQueue, aCharacter);
	}
	
	#endregion
	
	#region PRIVATE METHODS
	
	private Transform GetNearestTarget(List<Transform> aList, Character aCharacter)
	{
		Transform result = null;
		
		if(aList.Count > 0)
		{
			List<Transform> sameFloorTarget = new List<Transform>();
			
			foreach(Transform t in aList)
			{
				if(aCharacter.IsOnSameFloor(t.position))
				{
					sameFloorTarget.Add(t);
				}
			}
			
			if(sameFloorTarget.Count > 0)
			{
				sameFloorTarget.Sort(delegate(Transform t1, Transform t2)
				{
					return Vector2.Distance(aCharacter.Transform.position, t1.position).CompareTo(Vector2.Distance(aCharacter.Transform.position, t2.position));
				});
				
				result = sameFloorTarget[0];
				aList.Remove(result);
			}
			else
			{
				result = aList[0];
				aList.Remove(result);
			}
		}
		
		return result;
	}
	
	#endregion
}

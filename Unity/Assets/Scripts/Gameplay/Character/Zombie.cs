using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Zombie : Character, IRoaming, IProductable
{
	#region CONSTANTS

	private const float ATTACK_THRESHOLD = 0.2f;

	#endregion

	ZombieStats mStats;
	Human mTarget;
	GameObject mFightCloudFX;
	bool mIsFighting;

	#region MONO_METHODS
	
	protected override void Awake()
	{
		base.Awake();
		
		//
		Behavior = new ZombieBehavior(this);

		//
		mStats = new ZombieStats(0,0,0,0);
		mType = ECharacterType.Zombie;
	}
	
	#endregion
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets the stats.
	/// </summary>
	/// <value>
	/// The stats.
	/// </value>
	public ZombieStats Stats
	{
		get{return mStats;}
	}
	
	#endregion

	#region OVERRIDEN_METHODS
	
	protected override void UpdateAI ()
	{
		if(!mIsFighting && mBehavior != null)
		{
			mBehavior.Tick();
		}
		else
		{
			//Debug.Log("Character has no Behavior");
		}
	}
	public override void SetCharacterType (ECharacterType aCharacter)
	{
		throw new System.NotImplementedException ();
	}

	#endregion

	#region SERIALIZATION
	
	public override CharacterSerializationInfo Serialize ()
	{
		return new ZombieSerializationInfo(this);
	}
	
	public override void Deserialize (CharacterSerializationInfo aInfo)
	{
		base.Deserialize(aInfo);
		
		//
		ZombieSerializationInfo info = (ZombieSerializationInfo)aInfo;
		
		//
		mStats = info.mStats;
	}
	
	#endregion

	#region IRoaming implementation
	
	public bool HasTarget ()
	{
		mTarget = GetNearestHumanOnSameFloor();

		return (mTarget != null);
	}
	
	public bool IsAtTarget ()
	{
		return Mathf.Abs(mTarget.TransformPosition.x - mTransform.position.x) <= ATTACK_THRESHOLD;
	}
	
	public void GoToTarget ()
	{
		if (mTarget != null)
		{
			MoveToDynmanicObject(mTarget.Transform);
		}
	}
	
	public void DoTargetAction ()
	{
		mIsFighting = true;
		StopMovement();
		StartCoroutine("Fight");
		mTarget.Die();
	}

	
	#endregion

	#region INTERFACES

	public float GetProductCost()
	{
		//TODO Implement -CR

		return 1000;
	}

	#endregion

	Human GetNearestHumanOnSameFloor()
	{
		List<Character> characters = CharacterManager.Instance.GetCharactersOnFloor( (int)GridPosition.y );

		if(characters.Count > 0)
		{
			Human nearestCharacter = null;

			for(int i = 0; i < characters.Count; i++)
			{
				if(characters[i] != null && characters[i] is Human && !characters[i].IsInElevator)
				{
					//Prevent from targeting a dead character.
					if(!characters[i].IsDead && (nearestCharacter == null || Mathf.Abs(characters[i].Transform.position.x - Transform.position.x) < Mathf.Abs(nearestCharacter.Transform.position.x - Transform.position.x)))
					{
						nearestCharacter = characters[i] as Human;
					}
				}
			}

			return nearestCharacter as Human;
		}
		else
		{
			return null;
		}
	}

	#region COROUTINE

	IEnumerator Fight()
	{
		if(mTarget != null)
		{
			//Get the target near the zombie.
			mTarget.TransformPosition = new Vector3(TransformPosition.x,mTarget.TransformPosition.y,mTarget.TransformPosition.z);

			mFightCloudFX = FxManager.Instance.PlayFxAtPoint(EFxType.FightCloud, new Vector2(Transform.position.x, Transform.position.y + 0.25f));

			yield return new WaitForSeconds(3);

			CharacterManager.Instance.RemoveCharacter(mTarget);
			AutomaticPoolSystem.Instance.DestroyObject(mFightCloudFX);
			mIsFighting = false;
		}
	}

	#endregion
}

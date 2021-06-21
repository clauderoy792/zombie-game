using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HCUtils;

public class Human : Character
{
	#region CONSTANTS
	
	const float STATS_DECREASE_RATE = 0.2f;
	const float EXHAUSTION_DECREASE_VALUE = 0.1f;
	const float HUNGRINESS_DECREASE_VALUE = 0.3f;
	const float TOILET_DECREASE_VALUE = 0.75f;
	const float HUNGRINESS_TRESHOLD = 15;
	const float TOILET_TRESHOLD = 15;
	const float EXHAUSTION_TRESHOLD = 10;
	
	#endregion

	#region STATIC_MEMBERS

	#endregion

	#region PUBLIC_MEMBERS

	#endregion

	#region PROTECTED_MEMBERS

	protected CharacterVisual mVisual;
	protected SpriteRenderer mBloodSprite;
	protected Vector2 mLeftHomePosition;
	protected Vector2 mRightHomePosition;
	protected ERace mRace;
	protected EGender mGender;
	protected HumanStats mStats;
	protected JobStats mJobStats;
	protected int mSalary = 0;
	protected float mDropJunkChance = 0.2f;
	protected float mJunkDropRate = 5;
	protected bool mIsWorking = false;
	
	#endregion

	#region PRIVATE_MEMBERS

	private bool mStartWorkingOnNextMovement = false;

	#endregion

	#region ACCESSORS

	/// <summary>
	/// Gets the stats.
	/// </summary>
	/// <value>
	/// The stats.
	/// </value>
	public HumanStats Stats
	{
		get{return mStats;}
	}
	
	/// <summary>
	/// Gets or sets the work station.
	/// </summary>
	/// <value>
	/// The work station.
	/// </value>
	public JobStats JobStats
	{
		get{return mJobStats;}
		set{mJobStats = value;}
	}

	public ERace Race
	{
		get{return mRace;}
	}

	public EGender Gender
	{
		get{return mGender;}
	}

	public int Salary
	{
		get{return mSalary;}
		set{mSalary = Mathf.Clamp(value,0,int.MaxValue);}
	}

	//
	public bool IsWorking
	{
		get{return mIsWorking;}
	}

	//
	public bool IsStartingWorkOnNextMovement
	{
		get{return mStartWorkingOnNextMovement;}
	}

	//
	public bool IsHungry
	{
		get{return mStats.Hungriness < HUNGRINESS_TRESHOLD;}
	}
	
	//
	public bool IsTired
	{
		get{return mStats.Exhaustion < EXHAUSTION_TRESHOLD;}
	}
	
	//
	public bool HasToPee
	{
		get{return mStats.Toilet < TOILET_TRESHOLD;}
	}

	/// <summary>
	/// Gets the clothing, hair and accessories.
	/// </summary>
	/// <value>The accessories.</value>
	public CharacterVisual Visual
	{
		get{return mVisual;}
	}

	#endregion

	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();

		//
		mVisual = GetComponent<CharacterVisual>();
		mBloodSprite = mTransform.GetChild(1).GetComponentInChildren<SpriteRenderer>();

		mStats = HumanStatsGenerator.Instance.GenerateRandomStats();
		mJobStats = new JobStats();

		//Events
		mOnEndMove += OnEndMovement;
	}

	public override void Start()
	{
		base.Start();
		InvokeRepeating("DecreaseStats", STATS_DECREASE_RATE, STATS_DECREASE_RATE); 
		InvokeRepeating("DumpJunkIfNeeded", mJunkDropRate, mJunkDropRate);
	}
	
	public override void OnDestroy ()
	{
		base.OnDestroy ();

		mOnEndMove -= OnEndMovement;
	}
	
	#endregion

	#region DISPLAY_METHODS

	public override void Hide ()
	{
		base.Hide ();

		mVisual.clothing.enabled = false;
	}

	public override void Unhide ()
	{
		base.Unhide ();

		mVisual.clothing.enabled = true;
	}

	public override void Die ()
	{
		base.Die ();

		//Show blood.
		mBloodSprite.enabled = true;
	}

	#endregion

	#region RACE_GENDER_MANAGEMENT

	public void SetRace(ERace aRace)
	{
		mRace = aRace;

		if(mVisual == null || mVisual.skin == null)
		{
			return;
		}

		//We use color range instead of only one color for the skin tones. 
		switch(aRace)
		{
		case ERace.White:
			mVisual.skin.color = Color.Lerp(new Color(0.996f,0.902f,0.722f,1f), new Color(0.871f,0.737f,0.592f,1f), Random.value);
			break;
		case ERace.Asian:
			mVisual.skin.color = Color.Lerp(new Color(0.812f,0.678f,0.502f,1f), new Color(0.847f,0.741f,0.416f,1f), Random.value);
			break;
		case ERace.Black:
			mVisual.skin.color = Color.Lerp(new Color(0.42f,0.263f,0.196f,1f), new Color(0.192f,0.133f,0.106f,1f), Random.value);
			break;
		}
	}

	public void SetGender(EGender aGender)
	{
		mGender = aGender;
	}

	public override void SetCharacterType (ECharacterType aCharacter)
	{
		mVisual.characterType = aCharacter;
	}

	#endregion
	
	#region PRIVATE METHOD
	
	//
	public virtual void DecreaseStats()
	{
		mStats.Exhaustion -= EXHAUSTION_DECREASE_VALUE;
		mStats.Hungriness -= HUNGRINESS_DECREASE_VALUE;
		mStats.Toilet -= TOILET_DECREASE_VALUE;
	}
	
	void DumpJunkIfNeeded()
	{
		if(mCurrentRoom == null || mCurrentRoom.Type == ERoomType.Elevator)
		{
			return;
		}
			
		float dumpChance = Random.value;
		
		if(dumpChance < mDropJunkChance)
		{
			Transform junk = GraphicsManager.Instance.GetJunk().transform;
			junk.localPosition = new Vector3(mTransform.localPosition.x, mCurrentRoom.GridPosition.y, mTransform.localPosition.z);
			TargetManager.Instance.AddJunk(junk);

			if (mCurrentRoom != null)
			{
				mCurrentRoom.AddJunk();
			}
		}
	}
	
	#endregion

	#region INITIALIZATION

	public override void Initialize ()
	{
		base.Initialize ();

		mName = CharacterNameGenerator.Instance.GenerateName(mGender,mRace);
	}

	#endregion
	
	#region MOVEMENT_MANAGEMENT
	
	public virtual void GoHome()
	{
		//Stop working
		if (mCurrentRoom == JobStats.WorkStation)
		{
			StopWorking();
		}

		float xLeft = Mathf.Clamp(BuildingManager.Instance.LeftBorder - Room.UNIT_CELL_WIDTH, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		float xRight = Mathf.Clamp(BuildingManager.Instance.RightBorder + Room.UNIT_CELL_WIDTH, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		mLeftHomePosition = new Vector2( xLeft, 0 );
		mRightHomePosition = new Vector2( xRight, 0 );
		
		//
		MoveToLocalPoint((GridPosition.x <= PathFinder.GRID_WIDTH) ? new Vector2(mLeftHomePosition.x, mLeftHomePosition.y) : new Vector2(mRightHomePosition.x, mRightHomePosition.y));
	}
	
	protected virtual void DeleteCharacter()
	{
		//TODO Quick fix for Character's OnEndMove called when whe stop the current movement
		if (mTransform.position.x == mLeftHomePosition.x && mTransform.position.y +1== mLeftHomePosition.y)
		{
			if (JobStats.WorkStation != null)
			{
				JobStats.WorkStation.RemoveCharacter(this);
			}
			
			Destroy(mGameObject);
			CharacterManager.Instance.RemoveCharacter(this);
		}
	}

	public override Room MoveToRoom (ERoomType aType)
	{
		Room moveToRoom = base.MoveToRoom (aType);

		//If you are currently working and asked to go 
		//in to your working room, dont stop working.
		if (mCurrentRoom == mJobStats.WorkStation && moveToRoom != mJobStats.WorkStation)
		{
			StopWorking();
		}
		
		return moveToRoom;
	}
	
	public override void MoveToRoom (Room aRoom)
	{
		base.MoveToRoom (aRoom);

		if (mCurrentRoom == mJobStats.WorkStation && aRoom != mJobStats.WorkStation)
		{
			StopWorking();
		}
	}

	#endregion
	
	#region WORK_MANAGEMENT

	public void SetReadyToWork()
	{
		mStartWorkingOnNextMovement = true;
	}

	public virtual void StartWorking()
	{
		if(mJobStats.WorkStation != null)
		{
			mIsWorking = true;
			mStartWorkingOnNextMovement = false;

			mJobStats.WorkStation.AddCurrentlyWorkingHuman(this);
		}
	}
	
	protected void StopWorking()
	{
		if (mJobStats.WorkStation != null && mCurrentRoom == mJobStats.WorkStation)
		{
			mIsWorking = false;
			mJobStats.WorkStation.RemoveCurrentlyWorkingHuman(this);
		}
	}
	
	public void RemoveCurrentJob()
	{
		if (mJobStats.WorkStation != null)
		{
			mJobStats.WorkStation.RemoveCurrentlyWorkingHuman(this);
			mJobStats.WorkStation.RemoveWorkingHuman(this);
			mJobStats.WorkStation = null;
		}
	}
	
	public void Fire()
	{
		if(mJobStats.WorkStation != null)
		{
			//
			RemoveCurrentJob();
			
			//
			GoHome();
		}
	}
	
	#endregion

	#region STATS_MANAGEMENT

	public int GetProductivityStats()
	{
		int returnValue = 0;
		
		KeyValuePair<EHumanStats,EHumanStats> statsPriorities = CharacterManager.Instance.GetStatsPriority(mType);

		returnValue = ParseStats(statsPriorities.Key).GetTotal() + ParseStats(statsPriorities.Value).GetTotal();

		return returnValue;
	}

	/// <summary>
	/// Gets the productivity stats divided. The key is the stats that the stats that the character had when he was hired and
	/// the value is the stats that he earned while on his job.
	/// </summary>
	/// <returns>The productivity stats divided. The key is the sum of the stats that the charcter already had
	/// and the value is the sum of the stats that the character acquired</returns>
	public KeyValuePair<int,int> GetProductivityStatsDivided()
	{
		KeyValuePair<int,int> returnValue;
		KeyValuePair<EHumanStats,EHumanStats> priorities = CharacterManager.Instance.GetStatsPriority(mType);
		int baseStats = 0;
		int statsAcquired = 0;

		Stats priority1 = new Stats();
		Stats priority2 = new Stats();

		//Priority 1
		if (priorities.Key != EHumanStats.None)
		{
			priority1 = ParseStats(priorities.Key);
			baseStats 		+= priority1.BaseStats;
			statsAcquired 	+= priority1.StatsGained;
		}

		//Priority 2
		if (priorities.Value != EHumanStats.None)
		{
			priority2 = ParseStats(priorities.Value);
			baseStats 		+= priority2.BaseStats;
			statsAcquired 	+= priority2.StatsGained;
		}

		returnValue = new KeyValuePair<int, int>(baseStats,statsAcquired);
		return returnValue;
	}

	Stats ParseStats(EHumanStats aStats)
	{
		Stats returnValue = new Stats();
		
		switch(aStats)
		{
			case EHumanStats.Awareness:
				returnValue = mStats.Awareness;
				break;
			case EHumanStats.Handiness:
				returnValue = mStats.Handiness;
				break;
			case EHumanStats.Intellect:
				returnValue = mStats.Intellect;
				break;
		}

		return returnValue;
	}

	#endregion

	#region EVENTS

	void OnEndMovement()
	{
		//If character was set to start working.
		if (mStartWorkingOnNextMovement && mCurrentRoom == JobStats.WorkStation)
		{
			StartWorking();
		}
	}

	#endregion
	
	#region SERIALIZATION
	
	public override CharacterSerializationInfo Serialize()
	{
		return new HumanSerializationInfo(this);
	}
	
	public override void Deserialize (CharacterSerializationInfo aInfo)
	{
		base.Deserialize(aInfo);
		
		//
		HumanSerializationInfo info = (HumanSerializationInfo)aInfo;
		
		//
		mJobStats = new JobStats(info.mJobStats);

		//
		mVisual.Deserialize(info.mVisual);

		//Start working
		if (mJobStats.WorkStation == mCurrentRoom)
		{
			StartWorking();
		}
		
		//
		mStats = info.mStats;
		mRace = info.mRace;
		mGender = info.mGender;
	}
	
	#endregion
}

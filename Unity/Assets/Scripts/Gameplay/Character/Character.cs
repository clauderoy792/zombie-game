using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : HCMonoBehavior
{
	#region STATIC_MEMBERS
	
	private static int sIdGenerator;
	
	#endregion

	#region CONSTANTS

	protected const float ROAMING_DELAY 										= 2f;
	protected const float ROAMING_DELAY_WHILE_WAITING_ELEVATOR					= 0.5f;
	protected const float RANDOM_MOVEMENT_MIN_DISTANCE 							= 0.15f;
	protected const float RANDOM_MOVEMENT_MIN_DISTANCE_WHILE_WAITING_ELEVATOR 	= 0.05f;
	protected const float RANDOM_MOVEMENT_MAX_DISTANCE_WHILE_WAITING_ELEVATOR 	= 0.4f;
	#endregion
	
	#region EVENTs
	
	protected event System.Action mOnEndMove;
	protected event System.Action mOnEndRandomMove;
	
	#endregion

	#region MEMBERS

	protected SpriteRenderer mBaseSprite;
	protected Animator mController;
	protected ECharacterType mType;
	protected BehaviorTree mBehavior;
	protected string mName = "";
	protected Room mCurrentRoom;
	protected Vector2 mGridPosition;
	protected bool mIsMovingRandomly;
	protected bool mIsMoving;
	protected bool mIsDead = false;
	protected Transform mDynamicTargetTransform;
	private Vector2 mCurrentGoingToPosition;
	protected int mID;
	
	//Elevator variable
	protected bool mIsInElevator;
	protected int mDestinationFloor;
	private int mFloorWhenCallingElevator;
	private float mInitZ = float.MinValue;
	protected ElevatorLine mCurrentElevator;
	
	private bool mShouldCallEndMove;
	
	/// <summary>
	/// The list of vectors that is used to perform movement in function MoveTowardPath.
	/// </summary>
	protected List<Vector2> mCurrentPath;
	
	//Time in seconds that it takes to move from 1 unit.
	protected float mMovementSpeed = 64f;
	
	//Current Index that we are in the path.
	private int mCurrentIndexInPath;

	#endregion
	
	#region ACCESSORS
	
	public static int IDGenerator
	{
		get{return sIdGenerator;}
	}
	
	public int ID
	{
		get{return mID;}
	}
	
	public Vector2 GridPosition
	{
		get{return mGridPosition;}
	}
	
	public float InitZ
	{
		get{return mInitZ;}
	}
	
	public string Name
	{
		get{return mName;}
		set{mName = value;}
	}
	
	public ECharacterType Type
	{
		get {return mType;}
		set {mType = value;}
	}
	
	public Room CurrentRoom
	{
		get {return mCurrentRoom;}
		set {mCurrentRoom = value;}
	}
	
	public float MovementSpeed
	{
		get {return mMovementSpeed;}
	}
	
	public bool IsMovingRandomly
	{
		get {return mIsMovingRandomly;}
	}
	
	public bool IsInElevator
	{
		get {return mIsInElevator;}
		set{mIsInElevator = value;}
	}
	
	public bool IsMoving
	{
		get{return mIsMoving;}
	}

	public bool IsDead
	{
		get{return mIsDead;}
	}

	public BehaviorTree Behavior
	{
		get{return mBehavior;}
		set{mBehavior = value;}
	}

	public int DestinationFloor
	{
		get{return mDestinationFloor;}
	}
	
	public int AtFloorWhenCallingElevator
	{
		get{return mFloorWhenCallingElevator;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected  override void Awake()
	{
		base.Awake();

		//
		mBaseSprite = mGameObject.GetComponent<SpriteRenderer>();
		mController = mGameObject.GetComponent<Animator>();
	}
	
	public virtual void Start()
	{
		//If init z hasnt been initialized in deserialization
		if (mInitZ == float.MinValue)
		{
			mInitZ = mTransform.position.z;
		}
		
		//
		InvokeRepeating("UpdateAI", 0, 0.1f);
	}
	
	void OnMouseDown()
	{
		InGame.Instance.ShowCharacter(this);
	}
	
	public virtual void OnDestroy()
	{
		//Used in subclass
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public virtual void Initialize()
	{
	}
	
	#endregion
	
	#region TRANSFORM/DISPLAY_METHODS
	
	public virtual void Hide()
	{
		if(mRenderer)
		{
			mRenderer.enabled = false;
		}
	}
	
	public virtual void Unhide()
	{
		if(mRenderer)
		{
			mRenderer.enabled = true;
		}
	}
	
	public void SetTransformDepth(float aZ)
	{
		mTransform.localPosition = new Vector3(mTransform.localPosition.x,mTransform.localPosition.y,aZ);
	}

	public abstract void SetCharacterType(ECharacterType aCharacter);

	//
	public virtual void Die()
	{
		mIsDead = true;
		StopMovement();

		//Reset anim at frame 1.
		mController.Play("Idle",0,0);

		mController.Update(0);
		StartCoroutine("DisableAnimator");

		//Character is rotated a bit
		//Transform.rotation = Quaternion.Euler(0,0,81.5f);
		//Transform.position = new Vector3(Transform.position.x + GraphicsManager.HUMAN_SIZE/2, Transform.position.y + 0.12f, Transform.position.z);

		//Set scale
		mTransform.localScale = Vector3.one;

		//Character is rotated 90 degree.
		Transform.rotation = Quaternion.Euler(0,0,90);
		LocalTransformPosition = new Vector3(Transform.localPosition.x + GraphicsManager.HUMAN_SIZE/2, Transform.localPosition.y + 0.12f, Transform.localPosition.z);
	}

	IEnumerator DisableAnimator()
	{
		yield return new WaitForEndOfFrame();

		mController.enabled = false;
	}

	#endregion
	
	#region ID_MANGEMENT
	
	public static void InitializeIDGenerator(int aId)
	{
		sIdGenerator = aId;
	}
	
	public void GenerateID()
	{
		mID = sIdGenerator++;
	}
	
	#endregion
	
	protected virtual void UpdateAI()
	{
		if(mBehavior != null && !mIsDead)
		{
			mBehavior.Tick();
		}
	}

	protected virtual void Update()
	{
		// Update Animation
		if(!mIsDead)
		{
			if(mIsMoving)
			{
				mController.SetBool("Moving", true);
			}
			else
			{
				mController.SetBool("Moving", false);
			}
		}
	}
	
	#region RANDOM_MOVEMENT
	
	/// <summary>
	/// Move to a random position.
	/// </summary>
	/// <returns>
	/// The randomly in room.
	/// </returns>
	protected virtual IEnumerator MoveRandomlyInRoom()
	{	
		bool validPosition = false;
		
		if (mCurrentRoom != null)
		{
			//
			mIsMovingRandomly = true;

			//Set the minimal and miaximum x position that the charcter can go
			float minX = mCurrentRoom.LocalTransformPosition.x+GraphicsManager.HUMAN_SIZE/2;
			float maxX = minX + Room.UNIT_CELL_WIDTH -GraphicsManager.HUMAN_SIZE;
			float x = 0;
			
			//Move to a random position until stopcoroutine is called.
			while(true)
			{
				while (!validPosition)
				{
					//Get a random pos depending the place we want to go
					x = Random.Range(minX,maxX);
					
					validPosition = Mathf.Abs(mTransform.localPosition.x - x) >= RANDOM_MOVEMENT_MIN_DISTANCE;
				}

				//
				mShouldCallEndMove = true;

				//Move to point
				yield return StartCoroutine("PerformHorizontalMovement",x);
				
				//
				validPosition = false;
				
				//
				yield return new WaitForSeconds(ROAMING_DELAY*Time.timeScale);

				if (mOnEndRandomMove != null)
				{
					mOnEndRandomMove();
				}
			}
		}
	}
	
	private IEnumerator MoveRandomlyInElevator()
	{
		//
		bool validPosition = false;
		while (mIsInElevator)
		{
			//
			mIsMovingRandomly = true;

			//Set the minimal and maximum x value between elevator's doors
			float minX = mCurrentRoom.GridPosition.x*Room.UNIT_CELL_WIDTH+Room.UNIT_CELL_WIDTH/2-Elevator.DOOR_SIZE+GraphicsManager.HUMAN_BOX_COLLIDER_SIZE/2;
			float maxX = mCurrentRoom.GridPosition.x*Room.UNIT_CELL_WIDTH+Room.UNIT_CELL_WIDTH/2 +Elevator.DOOR_SIZE-GraphicsManager.HUMAN_BOX_COLLIDER_SIZE/2;
			float x = 0;
			
			//Move to a random position until stopcoroutine is called.
			while(mIsInElevator)
			{
				while (!validPosition)
				{
					//Get a random pos depending the place we want to go
					x = Random.Range(minX,maxX);
					
					validPosition = Mathf.Abs(mTransform.position.x - x) >= RANDOM_MOVEMENT_MIN_DISTANCE_WHILE_WAITING_ELEVATOR;
				}

				//Move to point
				yield return StartCoroutine("PerformHorizontalMovement",x);
				
				//
				validPosition = false;
				
				//
				yield return new WaitForSeconds(ROAMING_DELAY_WHILE_WAITING_ELEVATOR*Time.timeScale);
			}
		}
	}
	
	#endregion
	
	#region PREVIEW_MANAGEMENT
	
	/// <summary>
	/// Gets the preview.
	/// </summary>
	/// <returns>
	/// The preview.
	/// </returns>
	public GameObject GetPreview()
	{
		//TODO return gameobject preview.
		return null;
	}
	
	#endregion
	
	#region MOVEMENT_COROUTINES
	IEnumerator MoveToDynamicObject()
	{
		while(mTransform.position != mDynamicTargetTransform.position)
		{
			//
			mCurrentPath = PathFinder.Instance.FindPath(mGridPosition,new Vector2(mDynamicTargetTransform.position.x/Room.UNIT_CELL_WIDTH,
			                                                                      mDynamicTargetTransform.position.y/Room.UNIT_CELL_HEIGHT));

			//If were on the same cell
			if (mCurrentPath[1].x/Room.UNIT_CELL_WIDTH == GridPosition.x && mCurrentPath[1].y == GridPosition.y)
			{
				yield return StartCoroutine("PerformHorizontalMovement",mDynamicTargetTransform.position.x);	
			}
			else if (mCurrentGoingToPosition.x != mTransform.localPosition.x)
			{
				yield return StartCoroutine("PerformHorizontalMovement",mCurrentPath[1].x);	
			}
			else if (mCurrentPath[1].y != mGridPosition.y)
			{
				yield return StartCoroutine("PerformVerticalMovement",mCurrentPath[1].y/Room.UNIT_CELL_HEIGHT);
			}
		}
	}
	
	/// <summary>
	/// Moves the toward path definied in member mCurrentPath.
	/// </summary>
	/// <returns>
	/// The toward path.
	/// </returns>
	/// <param name='aGoToExactPoint'>
	/// Use true if you want to go to the exact position and false to go in the middle of a room.
	/// </param>
	IEnumerator MoveTowardPath(bool aGoToExactPoint)
	{
		//
		if (mCurrentPath != null)
		{
			for(int i = 1;i<mCurrentPath.Count;i++)
			{
				float goingToCoordinate = 0;

				//Horizontal movement
				if (!HCMath.AlmostEqual(mCurrentPath[i].x + Room.UNIT_CELL_WIDTH/2,mTransform.localPosition.x)  && mCurrentPath[i].y/Room.UNIT_CELL_HEIGHT == mGridPosition.y)
				{
					if (i == mCurrentPath.Count -1 && aGoToExactPoint)
					{
						goingToCoordinate = mCurrentGoingToPosition.x;
					}
					else
					{
						goingToCoordinate = mCurrentPath[i].x+Room.UNIT_CELL_WIDTH/2;
					}

					//Move to the position.
					yield return StartCoroutine("PerformHorizontalMovement",goingToCoordinate);

					//
					SetCharacterRoomAndGridPosition();
				}
				//Call Elevator
				else  if (mCurrentPath[i].y != mGridPosition.y || mIsInElevator)
				{
					//Find Going to floor
					int goingToFloor = (int)(mCurrentPath[i].y/Room.UNIT_CELL_HEIGHT);
					bool goingUp = mCurrentPath[i].y > mGridPosition.y;
					
					//
					while(i+1 < mCurrentPath.Count && mCurrentPath[i].x == mCurrentPath[i+1].x)
					{
						goingToFloor = goingUp ? goingToFloor+1 :goingToFloor-1;
						i++;
					}

					//Move the transform horizontally to the position
					yield return StartCoroutine("PerformVerticalMovement",goingToFloor);
				}	
				else
				{
					Debug.Log("NO MOVEMENT COULD BE DONE");
				}
			}

			//EndMovement event
			if (mOnEndMove != null)
			{
				mOnEndMove();
			}
		}
	}
	
	#endregion
	
	#region MOVEMENT_METHODS
	
	/// <summary>
	/// Stops all movement reliated coroutines.
	/// </summary>
	protected void StopMovement()
	{
		//
		StopCoroutine("MoveRandomlyInRoom");
		StopCoroutine("MoveRandomlyInElevator");
		StopCoroutine("MoveTowardPath");
		StopCoroutine("MoveToDynamicObject");
		StopCoroutine("PerformHorizontalMovement");
		StopCoroutine("PerformVerticalMovement");
		StopCoroutine("WaitForElevator");
	
		//
		RemoveFromElevator();

		//
		mIsMoving = false;
		mIsMovingRandomly = false;
	}
	
	public void MoveToDynmanicObject(Transform aTransform)
	{
		//
		mDynamicTargetTransform = aTransform;
		
		//
		StopMovement();
		
		//
		StartCoroutine("MoveToDynamicObject");
	}
	
	/// <summary>
	/// Moves to a world vector.
	/// </summary>
	public void MoveToLocalPoint(Vector2 aPoint)
	{
		//If were not in the same position already.
		if (!HCMath.AlmostEqual(aPoint.x,mTransform.localPosition.x) || aPoint.y +Room.UNIT_CELL_HEIGHT != mGridPosition.y)
		{
			Vector2 pathfinderPos = new Vector2(aPoint.x/Room.UNIT_CELL_WIDTH,aPoint.y/Room.UNIT_CELL_HEIGHT);
			
			//
			StopMovement();

			mCurrentPath = PathFinder.Instance.FindPath(mGridPosition,pathfinderPos);
			
			//If were on the same room already
			if (mCurrentPath != null && mCurrentPath.Count == 2)
			{
				mShouldCallEndMove = true;

				//
				StartCoroutine("PerformHorizontalMovement",aPoint.x);
			}
			else
			{
				mCurrentGoingToPosition = aPoint;
				
				//
				StartCoroutine("MoveTowardPath",true);
			}
		}
	}
	
	/// <summary>
	/// Moves the randomly in room.
	/// </summary>
	public void RandomMovementInRoom()
	{
		StopMovement();
		StartCoroutine("MoveRandomlyInRoom");
	}
	
	/// <summary>
	/// Moves to room.
	/// </summary>
	/// <returns>
	/// The to room.
	/// </returns>
	/// <param name='aType'>
	/// A type.
	/// </param>
	public virtual Room MoveToRoom(ERoomType aType)
	{
		Room returnValue = null;

		List<Room> rooms = RoomManager.Instance.GetSameTypeRoom(aType);
		
		if (rooms.Count > 0)
		{
			//
			List<KeyValuePair<Room, List<Vector2>>> roomPath = new List<KeyValuePair<Room, List<Vector2>>>();
			
			//
			for(int i = 0;i<rooms.Count;i++)
			{
				//
				roomPath.Add(new KeyValuePair<Room, List<Vector2>>(rooms[i], PathFinder.Instance.FindPath(mGridPosition,rooms[i].GridPosition)));
			}
			
			//
			roomPath.Sort(CompareGetBestRoom);
			
			//
			returnValue = roomPath[0].Key;
			
			//Check if we% found a valid path
			if (roomPath[0].Value != null)
			{
				//
				StopMovement();

				mCurrentPath = new List<Vector2>();

				//Set so we go in the middle of the room.
				//TODO Remove -CR
			/*	for(int i = 0;i < roomPath[0].Value.Count;i++)
				{
					mCurrentPath.Add(new Vector2(roomPath[0].Value[i].x+Room.UNIT_CELL_WIDTH/2,roomPath[0].Value[i].y));
				}*/

				mCurrentPath = roomPath[0].Value;

				//
				StartCoroutine("MoveTowardPath",false);
			}
		}
		
		return returnValue;
	}
	
	//
	static int CompareGetBestRoom(KeyValuePair<Room, List<Vector2>> a, KeyValuePair<Room, List<Vector2>> b)
    {
		if(a.Value == null)
		{
			return 1;
		}
		
		if(b.Value == null)
		{
			return -1;
		}
		
		if(a.Value.Count < b.Value.Count)
		{
			if(a.Key.IsRoomFull() && !b.Key.IsRoomFull())
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		else if(a.Value.Count > b.Value.Count)
		{
			if(!a.Key.IsRoomFull() && b.Key.IsRoomFull())
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
		else if(a.Value.Count == b.Value.Count)
		{
			if(!a.Key.IsRoomFull() && b.Key.IsRoomFull())
			{
				return -1;
			}
			else if(a.Key.IsRoomFull() && !b.Key.IsRoomFull())
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	
		return 0;
    }
	
	public virtual void MoveToRoom(Room aRoom)
	{
		if (aRoom != null)
		{
			//
			StopMovement();
			
			//Set current path
			mCurrentPath = PathFinder.Instance.FindPath(mGridPosition,aRoom.GridPosition);
			
			if (mCurrentPath != null)
			{
				//
				StartCoroutine("MoveTowardPath",false);
			}
		}
		else
		{
			Debug.LogError("Cannot move to a null room.");
		}
	}
	
	public bool IsOnSameFloor(Vector2 aOther)
	{
		return (aOther.y >= mGridPosition.y && aOther.y <= mGridPosition.y+Room.UNIT_CELL_HEIGHT);
	}
	
	IEnumerator PerformHorizontalMovement(float aGoingToPos)
	{
		mIsMoving = true;
		//
		bool moveRight = (aGoingToPos > mTransform.localPosition.x);
		
		//Move while were not at the right position.
		while(!HCMath.AlmostEqual(mTransform.localPosition.x,aGoingToPos))
		{
			//calculate the x room position that were in right now.
			int xBefore = (int)mTransform.localPosition.x/Room.UNIT_CELL_WIDTH;
			
			//Move Right
			if (moveRight)
			{
				//
				FaceRight();
				
				//Make sure we dont go past the other room
				if (mTransform.localPosition.x+(mMovementSpeed*Time.deltaTime) > aGoingToPos)
				{
					mTransform.localPosition = new Vector3(aGoingToPos,mTransform.localPosition.y,mTransform.localPosition.z);
				}
				else
				{
					mTransform.localPosition = new Vector3(mTransform.localPosition.x+(mMovementSpeed*Time.deltaTime),mTransform.localPosition.y,mTransform.localPosition.z);
				}
			}
			//Move Left
			else
			{
				//
				FaceLeft();
				
				//Make sure we dont go past the other room
				if (mTransform.localPosition.x-(mMovementSpeed*Time.deltaTime) <= aGoingToPos)
				{
					mTransform.localPosition = new Vector3(aGoingToPos,mTransform.localPosition.y,mTransform.localPosition.z);
				}
				else
				{
					mTransform.localPosition = new Vector3(mTransform.localPosition.x-(mMovementSpeed*Time.deltaTime),mTransform.localPosition.y,mTransform.localPosition.z);	
				}
			}	
			
			//Recalculate x pos if changed room
			if ((int)mTransform.localPosition.x/Room.UNIT_CELL_WIDTH != xBefore)
			{
				SetCharacterRoomAndGridPosition();
			}

			//
			yield return new WaitForEndOfFrame();
		}

		mIsMoving = false;

		//Call end move event
		if (mShouldCallEndMove)
		{
			mShouldCallEndMove = false;
			
			//
			if (mOnEndMove != null)
			{
				mOnEndMove();
			}
		}
	}
	
	/// <summary>
	/// go up in an elevator.
	/// </summary>
	/// <returns>
	/// The vertical movement.
	/// </returns>
	/// <param name='aGoingToFloor'>
	/// A going to position.
	/// </param>
	IEnumerator PerformVerticalMovement(int aGoingToFloor)
	{
		//
		mCurrentElevator = ElevatorManager.Instance.FindElevatorForCharacter(this);
		
		if (mCurrentElevator != null)
		{
			//If were not in the elevator already.
			if (!mIsInElevator)
			{
				//
				mDestinationFloor = aGoingToFloor;
				mFloorWhenCallingElevator = (int)mGridPosition.y;
				
				//
				mCurrentElevator.CallElevator(mID,(int)mGridPosition.y,aGoingToFloor);

				do
				{
					//
					yield return StartCoroutine("WaitForElevator");
				}
				//If the character failed to go in elevator while he could go in (elevator got off while he was
				// going in it, wait for it.
				while (!mCurrentElevator.AddWaitingCharacterInElevator(this));
			}
			
			//Wait till in Elevator
			yield return StartCoroutine ("MoveRandomlyInElevator");
	
			//
			SetCharacterRoomAndGridPosition();
		}
	}

	#endregion
	
	#region CHARACTER_POSITION_METHODS
	
	public void SetCharacterRoomAndGridPosition()
	{
		//
		mGridPosition.Set(Mathf.FloorToInt(mTransform.localPosition.x/Room.UNIT_CELL_WIDTH),Mathf.RoundToInt(mTransform.localPosition.y/Room.UNIT_CELL_HEIGHT));

		//Update Character in room list.
		RoomManager.Instance.UpdateCharacterRoom(this);
	}
	
	public void SetRoom(Room aRoom)
	{
		mTransform.localPosition = new Vector3(aRoom.GridPosition.x*Room.UNIT_CELL_WIDTH,aRoom.GridPosition.y*Room.UNIT_CELL_HEIGHT,mTransform.localScale.z);

		SetCharacterRoomAndGridPosition();
	}

	#endregion

	#region SCALE_MANAGEMENT

	public void FaceLeft()
	{
		//
		if (mTransform.localScale != Vector3.one)
		{
			mTransform.localScale = Vector3.one;
		}
	}

	public void FaceRight()
	{
		Vector3 right = new Vector3(-1,1,1);

		//
		if (mTransform.localScale != right)
		{
			mTransform.localScale = right;
		}
	}

	#endregion
	
	#region DISPOSE_MANAGEMENT
	
	public virtual void Dispose()
	{
		if (mCurrentRoom != null)
		{
			CharacterManager.Instance.RemoveCharacter(this);
		}
		
		//
		Destroy(mGameObject);
	}
	
	#endregion
	
	#region ELEVATOR_MANAGEMENT
	
	IEnumerator WaitForElevator()
	{
		bool validPosition = false;
		
		//
		mIsMovingRandomly = true;

		//Set the minimal and miaximum x position that the charcter can go
		float minX = mCurrentRoom.GridPosition.x*Room.UNIT_CELL_WIDTH+GraphicsManager.HUMAN_SIZE/2 + Room.UNIT_CELL_WIDTH/2;
		float maxX = mCurrentRoom.GridPosition.x*Room.UNIT_CELL_WIDTH-GraphicsManager.HUMAN_SIZE/2 + Room.UNIT_CELL_WIDTH/2;
		float x = 0;
		int i = 0;
		//While the elevator isnt stopped at our room
		while(!mCurrentElevator.CanGoIn(this))
		{
			while (!validPosition && i < 100)
			{
				//Get a random pos depending the place we want to go
				x = Random.Range(minX,maxX);
				
				float distance = Mathf.Abs(mTransform.localPosition.x - x)/Room.UNIT_CELL_WIDTH;
				validPosition =  distance >= RANDOM_MOVEMENT_MIN_DISTANCE_WHILE_WAITING_ELEVATOR 
					&& (distance <= RANDOM_MOVEMENT_MAX_DISTANCE_WHILE_WAITING_ELEVATOR);
				i++;
			}

			
			//Move to point
			yield return StartCoroutine("PerformHorizontalMovement",x);
			
			//
			validPosition = false;
			
			//
			yield return new WaitForSeconds(ROAMING_DELAY_WHILE_WAITING_ELEVATOR*Time.timeScale);
		}
		
		//Walk to the elevator
		yield return StartCoroutine("PerformHorizontalMovement",mGridPosition.x*Room.UNIT_CELL_WIDTH+Room.UNIT_CELL_WIDTH/2);
	}
	
	#endregion
	
	#region ELEVATOR_MANAGEMENT

	void RemoveFromElevator()
	{
		if (mCurrentElevator != null)
		{
			mCurrentElevator.RemoveCharacter(this);
			mCurrentElevator = null;
		}
	}
	
	#endregion
	
	#region SERIALIZATION

	public virtual void PostSerialization()
	{
		if (mIsInElevator)
		{
			ElevatorManager.Instance.AddCharacterInElevator(this);
		}
	}
	
	public virtual CharacterSerializationInfo Serialize()
	{
		return new CharacterSerializationInfo(this);
	}
	
	public virtual void Deserialize(CharacterSerializationInfo aInfo)
	{
		//
		mID = aInfo.mID;
		
		//Set gameobject's name
		mGameObject.name = "Character"+ mID;
		
		//
		mTransform.localPosition = new Vector3(aInfo.mTransformX,aInfo.mTransformY,aInfo.mTransformZ);
	
		//
		mInitZ = aInfo.mInitZ;
		
		//
		mGridPosition = new Vector2(aInfo.mGridPositionX,aInfo.mGridPositionY);
		
		//Elevator
		mDestinationFloor = aInfo.mDestinationFloor;
		mFloorWhenCallingElevator = aInfo.mAtFloorWhenCallingElevator;
		mIsInElevator = aInfo.mIsInElevator;
		
		//
		mIsMovingRandomly = aInfo.mIsMovingRandomly;
		
		//
		mMovementSpeed = aInfo.mMovementSpeed;
		
		//
		mName = aInfo.mName;
		
		mType = aInfo.mType;
		
		//Add character in the room's character list
		RoomManager.Instance.UpdateCharacterRoom(this);
		
		//
		if (mIsMovingRandomly)
		{
			RandomMovementInRoom();
		}
	}
	
	#endregion
}

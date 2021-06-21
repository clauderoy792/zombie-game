using UnityEngine;
using HCUtils;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Room : HCMonoBehavior {
	
	#region STATIC_MEMBERS
	
	public const int UNIT_CELL_HEIGHT = 64;
	public const int UNIT_CELL_WIDTH = 128;
	
	#endregion
	
	#region MEMBERS

	//
	public ProgressBar mProgressBar;

	//
	protected Vector2 mGridPosition;
	
	//
	protected int mMaxHumansPerRoom;
	protected int mWidth;
	protected int mHeight;
	protected int mNumberOfJunks;

	//
	protected ERoomType mRoomType;

	//
	protected List<Character> mCharactersUsingRoom;
	protected ReadOnlyCollection<Character> mCharactersInRoomReadOnly;
	
	//
	protected float mDirtinessMultiplier = 1;
	
	private RoomSerializationInfo mRoomSerInfo;
	
	protected SpriteRenderer mSprite;
	
	#endregion
	
	#region ACCESSORS
	
	
	public ReadOnlyCollection<Character> CharactersUsingRoom
	{
		get {return mCharactersInRoomReadOnly;}
	}
	
	public ERoomType Type
	{
		get {return mRoomType;}
	}
	
	public int NumberOfJunks
	{
		get{return mNumberOfJunks;}
	}

	public Vector2 GridPosition
	{
		get {return mGridPosition;}
		set 
		{
			mGridPosition = value;
		}
	}
	
	public SpriteRenderer Sprite
	{
		get{return mSprite;}
	}

	public int Width
	{
		get {return mWidth;}
	}
	
	public int Height
	{
		get {return mHeight;}
	}
	
	public int MaxNumberOfHumans
	{
		get {return mMaxHumansPerRoom;}
		set {mMaxHumansPerRoom = value;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	protected override void Awake ()
	{
		base.Awake ();
		mSprite = GetComponent<SpriteRenderer>();
	}
	
	protected virtual void Start()
	{
	}
	
	protected virtual void Update()
	{
	}

	protected virtual void OnDestroy()
	{

	}
	
	void OnTouchUp2D()
	{
		//
		InGame.Instance.ShowRoom(this);
	}
	
	#endregion

	#region INITIALIZATION
	
	public virtual void Initialize(float aX, float aY, ERoomType aType)
	{
		//
		mWidth = Room.UNIT_CELL_WIDTH/Room.UNIT_CELL_HEIGHT;
		
		//
		mCharactersUsingRoom = new List<Character>();
		mCharactersInRoomReadOnly = mCharactersUsingRoom.AsReadOnly();

		//
		mMaxHumansPerRoom = 5;
		mRoomType = aType;
		mHeight = 1;
		
		if (aX < 0 || aX + mWidth >= PathFinder.GRID_WIDTH || aY < 0 
			|| aY + mHeight >= PathFinder.GRID_HEIGHT)
		{
			Debug.LogError("Room position is outside of the Pathfinder's array : "+aX+","+aY);
		}
		
		mGridPosition = new Vector2(aX,aY);
		
		//
		InitializeRoomInPathFinder();
	}
	
	public virtual void PostInitialization()
	{
	}

	void InitializeRoomInPathFinder()
	{
		//Set elevator
		if (mRoomType == ERoomType.Elevator)
		{
			PathFinder.Instance.SetRoomElevator(this,true);
		}
		
		//
		PathFinder.Instance.SetRoomOccupied(mGridPosition,true);
	}
	
	public void InitializeUsingCharacters()
	{
		foreach(int charId in mRoomSerInfo.mCharactersUsingRoom)
		{
			mCharactersUsingRoom.Add(CharacterManager.Instance.GetCharById(charId));
		}
	}
	
	#endregion
	
	#region ROOM_POSITION_MANAGEMENT
	
	public bool IsCharacterInRoom(Character aCharacter)
	{
		return ContainsRoom(aCharacter.GridPosition);
	}
	
	public bool Overlaps(Vector2 aOther,int aWidth,int aHeight)
	{
		return (mGridPosition.x < aOther.x+aWidth && 
			mGridPosition.x +mWidth > aOther.x &&
			mGridPosition.y < aOther.y +aHeight &&
			mGridPosition.y + mHeight > aOther.y);
	}
	
	public bool Overlaps(Room aOther)
	{
		return	(mGridPosition.x < aOther.GridPosition.x+aOther.Width && 
			mGridPosition.x +mWidth > aOther.GridPosition.x &&
			mGridPosition.y < aOther.GridPosition.y +aOther.Height &&
			mGridPosition.y + mHeight > aOther.GridPosition.y);
	}
	
	public bool Overlaps(Rect aOther)
	{
		return	(mGridPosition.x < aOther.x+aOther.width && 
			mGridPosition.x +mWidth > aOther.x &&
			mGridPosition.y < aOther.y +aOther.height &&
			mGridPosition.y + mHeight > aOther.y);
	}
	
	public bool ContainsRoom(Vector2 aPosition)
	{
		return	(mGridPosition.x <= aPosition.x&& 
			mGridPosition.x +mWidth > aPosition.x &&
			mGridPosition.y <= aPosition.y &&
			mGridPosition.y + mHeight > aPosition.y);
	}
	
	public bool ContainsCharacter(Vector2 aPosition)
	{
		return	(mGridPosition.x <= aPosition.x&& 
			mGridPosition.x +mWidth > aPosition.x &&
			mGridPosition.y <= aPosition.y &&
			mGridPosition.y + mHeight > aPosition.y);
	}
	
	#endregion
	
	#region CHARACTER_MANAGEMENT
	
	/// <summary>
	/// Determines whether this instance is room full. Only counts the humans.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is room full; otherwise, <c>false</c>.
	/// </returns>
	public virtual bool IsRoomFull()
	{
		return (mCharactersUsingRoom.Count >= mMaxHumansPerRoom);
	}
	
	public void AddCharacter(Character aCharacter)
	{
		mCharactersUsingRoom.Add(aCharacter);
	}

	public virtual bool RemoveCharacter(Character aCharacter)
	{
		return mCharactersUsingRoom.Remove(aCharacter);
	}
	
	public List<Zombie> GetZombies()
	{
		List<Zombie> zombies = null;
		
		var chars = mCharactersUsingRoom.FindAll(c => c.Type == ECharacterType.Zombie);
		
		if (chars != null && chars.Count > 0)
		{
			zombies = new List<Zombie>();
			
			foreach(var c in chars)
			{
				zombies.Add(c as Zombie);
			}
		}
		
		return zombies;
	}
	
	#endregion
	
	#region JUNKS_MANAGEMENT
	
	public void AddJunk()
	{
		mNumberOfJunks++;
	}
	
	public void RemoveJunk()
	{
		mNumberOfJunks--;
	}

	#endregion
	
	#region SERIALIZATION
	
	public virtual RoomSerializationInfo Serialize()
	{
		return new RoomSerializationInfo(this);
	}
	
	public virtual void Deserialize(RoomSerializationInfo aInfo)
	{
		//
		mRoomSerInfo = aInfo;
		
		//
		mCharactersUsingRoom = new List<Character>();
		mCharactersInRoomReadOnly = mCharactersUsingRoom.AsReadOnly();

		//
		mWidth = aInfo.mWidth;
		mMaxHumansPerRoom = aInfo.mMaxHumansPerRoom;
		mRoomType = aInfo.mRoomType;
		mHeight = aInfo.mHeight;
		
		//
		mNumberOfJunks = aInfo.mNbJunks;
		
		//
		mGridPosition = new Vector2(aInfo.mX,aInfo.mY);

		//
		InitializeRoomInPathFinder();

		//
		Initialize(aInfo.mX,aInfo.mY,aInfo.mRoomType);
	}
	
	#endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

public class RoomManager : MonoBehaviour {
	
	#region STATIC_MEMBERS
	
	private static RoomManager sInstance;
	
	#endregion

	#region CONSTANTS

	const int BASE_ROOM_COST = 500;
	const float ROOM_COST_INCREMENT_FACTOR = 1.85f;

	#endregion
	
	#region MEMBERS
	
	private List<Room> mRooms;
	private ReadOnlyCollection<Room> mRoomsReadOnly;
	private int mCurrentRoomPrice;
	private int mRoomId;
	private int mNbRoomCreated;

	#endregion
	
	#region ACCESSORS
	
	public static RoomManager Instance
	{
		get 
		{
			return sInstance;
		}
	}
	
	public ReadOnlyCollection<Room> Rooms
	{
		get {return mRoomsReadOnly;}
	}

	public int RoomPrice
	{
		get{return mCurrentRoomPrice;}
	}

	public int NbRoomCreated
	{
		get{return mNbRoomCreated;}
	}

	public int RoomId
	{
		get{return mRoomId;}
	}
	
	#endregion
	
	#region MONO_METHODS

	void Awake()
	{
		if (sInstance == null)
		{
			mCurrentRoomPrice = BASE_ROOM_COST;
			sInstance = this;
			mRooms = new List<Room>();
			mRoomsReadOnly = mRooms.AsReadOnly();
		}
		else
		{
			Debug.Log("There is already an instance of the RoomManger in the scene");
		}
	}
	
	#endregion

	#region ROOM_PRICE_MANAGEMENT

	public void IncrementRoomCreated()
	{
		mNbRoomCreated++;
		mCurrentRoomPrice = (int)(mCurrentRoomPrice * ROOM_COST_INCREMENT_FACTOR);
	}

	#endregion

	#region CHARACTER_MANAGEMENT
	
	/// <summary>
	/// Updates the character's current room and add it to the room's character list.
	/// </summary>
	/// <param name='aCharacter'>
	/// A character.
	/// </param>
	public void UpdateCharacterRoom(Character aCharacter)
	{
		if (aCharacter != null)
		{
			//Find room that contains the character
			Vector2 gridPos = aCharacter.LocalTransformPosition.WorldToGrid().GridToWorld().WorldToGrid();
			Room newRoom = mRooms.Find(r => r.GridPosition == gridPos);
			
			if (newRoom != null && newRoom != aCharacter.CurrentRoom)
			{
				//Remove Character from currentRoom
				if (aCharacter.CurrentRoom != null)
				{
					aCharacter.CurrentRoom.RemoveCharacter(aCharacter);
				}
				
				aCharacter.CurrentRoom = newRoom;
			}
		}
	}
	
	/// <summary>
	/// Initializes the characters in room.
	/// </summary>
	public void InitializeCharactersInRoom()
	{
		foreach(Room r in mRooms)
		{
			r.InitializeUsingCharacters();
		}
	}
	
	#endregion
	
	#region ROOM_ACCESSIBILITY
	/// <summary>
	/// Determines whether this instance can create room the specified aPos. If there is a overlapping corridor, it removes it
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance can create room the specified aPos; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='aPos'>
	/// If set to <c>true</c> a position.
	/// </param>
	public bool CanCreateRoom(Rect aPos,bool aRemoveContainingRoomIfNeeded = false)
	{
		bool returnValue = true;
		
		//Find if we are trying to create a room over another one.
		Room containingRoom = mRooms.Find(r => r.ContainsRoom(new Vector2(aPos.x,aPos.y)));
		
		//
		if (containingRoom == null || containingRoom.Type == ERoomType.Corridor)
		{
			//Can always create at first floor, if theres no room but a corridor
			if (aPos.y == 0)
			{
				returnValue = true;
			}
			else
			{
				for(int i = 0;i<aPos.width/Room.UNIT_CELL_WIDTH;i++)
				{
					if (!PathFinder.Instance.Grid[(int)aPos.x+i][(int)aPos.y].IsOccupied)
					{
						returnValue = false;
						break;
					}
				}
			}
		}
		else
		{
			returnValue = false;
		}
		
		//
		if (aRemoveContainingRoomIfNeeded)
		{
			//Removes overlapping corridor if needed.
			if (returnValue && containingRoom != null)
			{
				RemoveRoom(containingRoom);
			}
		}

		return returnValue;
	}

	public bool IsPositionValid(Vector2 aGridPos)
	{
		//
		if(aGridPos.x * Room.UNIT_CELL_WIDTH < BuildingManager.Instance.LeftBorder || aGridPos.x * Room.UNIT_CELL_WIDTH >= BuildingManager.Instance.RightBorder || aGridPos.y * Room.UNIT_CELL_HEIGHT >= BuildingManager.Instance.TopBorder )
		{
			return false;
		}

		//
		Room other = mRooms.Find(r => r.GridPosition == aGridPos);

		if(other == null && aGridPos.y == 0)
		{
			return true;
		}
		
		Room under = mRooms.Find(r => r.GridPosition == new Vector2(aGridPos.x, aGridPos.y-1));
		
		return other == null && under != null;
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public void ResetRooms()
	{
		foreach(Room room in mRooms)
		{
			Destroy(room.GameObject);
		}
		
		//
		mRooms = new List<Room>();
		mRoomsReadOnly = mRooms.AsReadOnly();
	}
	
	public void InitializeNewGame()
	{
		//Add a reception in the bottom center of the building.
		Reception r = AddRoom(new Vector2(PathFinder.GRID_WIDTH/2,0),ERoomType.Reception,true) as Reception;

		if (r != null)
		{
			int nbWaitingCivilian = 4;

			//Add civilians to waiting queue
			for(int i = 0;i < nbWaitingCivilian;i++)
			{
				Civilian c = CharacterManager.Instance.AddCharacter(ECharacterType.Civilian,r) as Civilian;

				if (c != null)
				{
					c.InitializeCharacterWithoutMoving();
					r.IncrementSpawnedCivilian();
				}
				else
				{
					Debug.LogError("Civilian is null when trying to add to reception in InitializeNewGame");
				}
			}
		}
		else
		{
			Debug.LogError("Reception is null when initializing a new game");
		}
	}
	
	#endregion
	
	#region PRODUCTION_ROOM_MANAGEMENT
	
	public ZombieWarehouse FindProductionRoom()
	{
		Room room = mRooms.Find(r => (r.Type == ERoomType.ZombieWarehouse && !(r as ZombieWarehouse).IsRoomFull()));
		
		return ( room != null ?  room as ZombieWarehouse : null); 
	}
	
	#endregion
	
	#region ROOM_CREATION

	/// <summary>
	/// Adds the room.
	/// </summary>
	/// <returns>The room.</returns>
	/// <param name="aPos">The grid position.</param>
	/// <param name="aType">A type.</param>
	/// <param name="aIsRoomFree">If set to <c>true</c> a is room free.</param>
	public Room AddRoom(Vector2 aGridPos,ERoomType aType, bool aIsRoomFree = false)
	{
		Room returnValue = null;
		int price = aIsRoomFree ? 0 : mCurrentRoomPrice;
		//TODO Remove -CR.
		price = 1;
		if (GameManager.Instance.UserStats.CanBuy(ECurrency.Gold,price))
		{
			//
			GameManager.Instance.UserStats.RemoveMoney( ECurrency.Gold,price);
			
			//
			RemoveRoom(aGridPos);

			//
			returnValue = CreateRoom(aGridPos,aType);
			
			//
			returnValue.Initialize(aGridPos.x,aGridPos.y,aType);
			
			//
			mRooms.Add(returnValue);
			
			//Add in ElevatorManager
			if (returnValue.Type == ERoomType.Elevator)
			{
				ElevatorManager.Instance.AddElevator(returnValue as Elevator);
			}
			
			//
			CheckForCorridors(aGridPos);

			//
			if (!aIsRoomFree)
			{
				IncrementRoomCreated();
			}
		}
		else
		{
			Debug.Log("Not enough money to create that room, price is : "+price);
		}

		return returnValue;
	}

	/// <summary>
	/// Adds the blueprint for room.
	/// </summary>
	/// <param name="aRoomType">A room type.</param>
	/// <param name="aGridPos">A grid position.</param>
	public void AddBlueprintForRoom(ERoomType aRoomType, Vector2 aGridPos)
	{
		Blueprint bp = GraphicsManager.Instance.GetBlueprint(aRoomType);

		Vector3 localPos = aGridPos.GridToWorld();
		localPos.z = -500;
		bp.CachedTransform.localPosition = localPos;
	}
	
	/// <summary>
	/// Adds the room.
	/// </summary>
	/// <param name='aRoom'>
	/// A room.
	/// </param>
	public void AddRoom(RoomSerializationInfo aRoom)
	{
		Room newRoom = null;

		//
		newRoom = CreateRoom(new Vector2(aRoom.mX,aRoom.mY),aRoom.mRoomType);

		//
		newRoom.Deserialize(aRoom);
		
		//
		mRooms.Add(newRoom);
	}
	
	private void AddRoomWithoutCheckingCorridors(Vector2 aPos,ERoomType aType)
	{
		//
		Room newRoom = CreateRoom(new Vector2((int)aPos.x*2,aPos.y),aType);
		
		//
		newRoom.Initialize(aPos.x,aPos.y,aType);
		
		//
		mRooms.Add(newRoom);
	}
	
	/// <summary>
	/// Removes the room.
	/// </summary>
	/// <returns>
	/// The room.
	/// </returns>
	/// <param name='aRoom'>
	/// If set to <c>true</c> a room.
	/// </param>
	public bool RemoveRoom(Room aRoom)
	{
		//
		Destroy(aRoom.GameObject);
		
		//
		PathFinder.Instance.SetRoomOccupied(aRoom.GridPosition,false);
		
		return mRooms.Remove(aRoom);
	}
	
	public bool RemoveRoom(Vector2 aPos)
	{
		Room room = mRooms.Find(r => r.GridPosition == aPos);
		
		if (room != null)
		{
			//
			Destroy(room.GameObject);
			
			//
			PathFinder.Instance.SetRoomOccupied(aPos,false);
			
			//
			return mRooms.Remove(room);
		}
		
		return false;
	}

	/// <summary>
	/// Replaces the room.
	/// </summary>
	/// <returns><c>true</c>, if room was replaced, <c>false</c> otherwise.</returns>
	/// <param name="aGridPos">A grid position.</param>
	/// <param name="aType">A type.</param>
	/// <param name="aIsFree">If set to <c>true</c> a is free.</param>
	public Room ReplaceRoom(Vector2 aGridPos,ERoomType aType,bool aIsFree = false)
	{
		Room returnValue = GetRoom(aGridPos);

		if (returnValue != null)
		{
			returnValue = ReplaceRoom(returnValue,aType,aIsFree);
		}

		return returnValue;
	}

	public Room ReplaceRoom(Room aRoom,ERoomType aType,bool aIsRoomFree = false)
	{
		Room returnValue = null;

		if (aRoom != null)
		{
			Vector2 gridPos = aRoom.GridPosition;

			//
			returnValue = AddRoom(gridPos,aType,aIsRoomFree);
		}

		return returnValue;
	}
	
	/// <summary>
	/// Creates the room at a given position.
	/// </summary>
	/// <param name='aPos'>
	/// A position.
	/// </param>
	/// <param name='aType'>
	/// A type.
	/// </param>
	/// <param name='aSize'>
	/// A size.
	/// </param>
	private Room CreateRoom(Vector2 aPos,ERoomType aType)
	{
		//
		GameObject go = GraphicsManager.Instance.GetRoom(aType);

		Room newRoom = go.GetComponent<Room>();

		//
		newRoom.LocalTransformPosition = aPos.GridToWorld();

		if (newRoom != null)
		{
			//
			newRoom.GridPosition = aPos;

			//Name room
			newRoom.name = aType.ToString()+mRoomId++;
		}
		else
		{
			Debug.LogError("Could not set grid position");
		}
	
		return newRoom;
	}
	
	/// <summary>
	/// Check if we need to create corridors on the room's position.
	/// </summary>
	/// <param name='aPos'>
	/// A position.
	/// </param>
	private void CheckForCorridors(Vector2 aPos)
	{
		//
		List<Room> rooms = GetRoomOnSameRow((int)aPos.y);
		
		//Sort rooms x pos
		rooms.Sort(delegate(Room r1, Room r2) {
			return (r1.GridPosition.x.CompareTo(r2.GridPosition.x));
		});
		
		//If it is the first or last room, and that the count is greater than 1.
		if (rooms.Count > 1)
		{
			if (rooms[0].GridPosition.x == (int)aPos.x)
			{
				//Set start position
				Vector3 startPos = new Vector3(rooms[0].GridPosition.x+1,rooms[0].GridPosition.y,0);
				
				//Put corridors to the right	
				while(startPos.x < rooms[1].GridPosition.x)
				{
					//Check if we are not overlapping a room
					if (CanCreateRoom(new Rect(startPos.x,startPos.y,Room.UNIT_CELL_WIDTH,Room.UNIT_CELL_HEIGHT)))
					{	
						//
						AddRoomWithoutCheckingCorridors(startPos,ERoomType.Corridor);
					}
					
					//Move to next room
					startPos.Set(startPos.x+1,startPos.y,startPos.z);
				}
			}
			else if (rooms[rooms.Count-1].GridPosition.x == (int)aPos.x)
			{
				//Set start position
				Vector3 startPos = new Vector3(rooms[rooms.Count-2].GridPosition.x+1,rooms[rooms.Count-2].GridPosition.y,0);
				
				//Put corridors to the left	
				while(startPos.x < aPos.x)
				{
					//Check if we are not overlapping a room
					if (CanCreateRoom(new Rect(startPos.x,startPos.y,Room.UNIT_CELL_WIDTH,Room.UNIT_CELL_HEIGHT)))
					{	
						//
						AddRoomWithoutCheckingCorridors(startPos,ERoomType.Corridor);
					}
					
					//Move to next room
					startPos.Set(startPos.x+1,startPos.y,startPos.z);
				}
			}
		}
	}
	
	#endregion

	#region ROOM_INFORMATIONS
	
	public List<Room> GetSameTypeRoom(ERoomType aType)
	{
		return mRooms.FindAll(r => r.Type == aType);
	}
	
	public List<Room> GetRoomOnSameRow(int aRow)
	{
		return mRooms.FindAll(r => r.GridPosition.y == aRow);
	}
	
	public List<Room> GetRoomOnSameColumn(int aColumn)
	{
		return mRooms.FindAll(r => r.GridPosition.x == aColumn);
	}
	
	/// <summary>
	/// Gets a accessible random room that is different from the current room.
	/// </summary>
	/// <returns>
	/// The random room.
	/// </returns>
	/// <param name='aCharacter'>
	/// A character.
	/// </param>
	public Room GetRandomRoom(Character aCharacter)
	{
		List<int> checkedRooms = new List<int>(mRooms.Count);
		
		//
		Room returnValue = aCharacter.CurrentRoom;
		
		//
		int nbRoomChecked = 0;
		int randomIndex = 0;
		
		//
		bool validIndex = false;
		
		while (nbRoomChecked < mRooms.Count)
		{
			validIndex = false;
			
			//Find a room that has never been checked before.
			while (!validIndex)
			{
				randomIndex = UnityEngine.Random.Range(0,mRooms.Count);
				
				if (!checkedRooms.Contains(randomIndex))
				{
					checkedRooms.Add(randomIndex);
					validIndex = true;
				}
			}
			
			//Check if we can go to the room and that
			// it is not the same room the current one.
			if (mRooms[randomIndex] != aCharacter.CurrentRoom)
			{
				//Check if room isnt full and that the path is smaller than the other one.
				if (PathFinder.Instance.FindPath(aCharacter.GridPosition,mRooms[randomIndex].GridPosition) != null)
				{
					returnValue = mRooms[randomIndex];
					break;
				}
			}
			
			nbRoomChecked++;
		}

		return returnValue;
	}

	public Room GetRandomRoomOnSameFloor(Character aCharacter)
	{
		List<int> checkedRooms = new List<int>(mRooms.Count);
		List<Room> rooms = GetRoomOnSameRow((int)aCharacter.GridPosition.y);

		//
		Room returnValue = aCharacter.CurrentRoom;

		//
		int nbRoomChecked = 0;
		int randomIndex = 0;
		
		//
		bool validIndex = false;
		if (rooms != null && rooms.Count > 0)
		{
			while (nbRoomChecked < rooms.Count)
			{
				validIndex = false;
				
				//Find a room that has never been checked before.
				while (!validIndex)
				{
					randomIndex = UnityEngine.Random.Range(0,rooms.Count);
					
					if (!checkedRooms.Contains(randomIndex))
					{
						checkedRooms.Add(randomIndex);
						validIndex = true;
					}
				}
				
				//Check if we can go to the room and that
				// it is not the same room the current one.
				if (rooms[randomIndex] != aCharacter.CurrentRoom)
				{
					//Check if that the path is smaller than the other one.
					if (PathFinder.Instance.FindPath(aCharacter.GridPosition,rooms[randomIndex].GridPosition) != null)
					{
						returnValue = rooms[randomIndex];
						break;
					}
				}
				
				nbRoomChecked++;
			}
		}
		
		return returnValue;
	}

	//TODO Implement - CR
	public Room GetAdjacentRoom(Rect aRect,EDirection aDirection)
	{
		/*switch(aDirection)
		{
			case EDirection.North:
				
				break;
			case EDirection.East:
				break;
			case EDirection.South:
				break;
			case EDirection.West:
				break;
		}*/
		return null;
	}
	
	/// <summary>
	/// Gets the room.
	/// </summary>
	/// <returns>
	/// The room.
	/// </returns>
	/// <param name='aPosition'>
	/// A Grid Position.
	/// </param>
	public Room GetRoom(Vector2 aPosition)
	{
		return mRooms.Find(r => r.GridPosition == aPosition);
	}
	
	/// <summary>
	/// Gets the room.
	/// </summary>
	/// <returns>
	/// The room.
	/// </returns>
	/// <param name='aX'>
	/// A x.
	/// </param>
	/// <param name='aY'>
	/// A y.
	/// </param>
	public Room GetRoom(float aX, float aY)
	{
		return GetRoom(new Vector2(aX,aY));
	}
	
	public bool IsRoomOverlapOthers(Rect aRoom)
	{
		bool returnValue = false;
		
		for(int i = 0;i<mRooms.Count;i++)
		{
			if (mRooms[i].Overlaps(aRoom))
			{
				returnValue = true;
				break;
			}
		}
		
		return returnValue;
	}
	
	public bool IsRoomOverlapOthers(Room aRoom)
	{
		bool returnValue = false;
		
		for(int i = 0;i<mRooms.Count;i++)
		{
			if (mRooms[i].Overlaps(aRoom))
			{
				returnValue = true;
				break;
			}
		}
		
		return returnValue;
	}
	
	#endregion
	
	#region WORKING_ROOM_MANAGEMENT
	
	public List<WorkingRoom> GetAvailableWorkingRooms()
	{
		List<WorkingRoom> returnValue = new List<WorkingRoom>();
		
		//
		for(int i =0 ;i < mRooms.Count;i++)
		{
			if (mRooms[i] is WorkingRoom)
			{
				//
				WorkingRoom r = mRooms[i] as WorkingRoom;
				
				//
				if (!r.IsFullOfWorkers())
				{
					returnValue.Add(r);
				}
			}
		}
		
		return returnValue;
	}
	
	#endregion

	#region SERIALIZATION

	public RoomManagerSerializationInfo Serialize()
	{
		return new RoomManagerSerializationInfo(this);
	}

	public void Deserialize(RoomManagerSerializationInfo aInfo)
	{
		mCurrentRoomPrice = aInfo.mCurrentRoomPrice;
		mRoomId = aInfo.mRoomId;
	}

	#endregion
}

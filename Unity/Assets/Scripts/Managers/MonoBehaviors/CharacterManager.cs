using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CharacterManager : MonoBehaviour
{
	//
	private static CharacterManager mInstance;
	private List<Character> mCharacters;
	private ReadOnlyCollection<Character> mCharactersReadOnly;
	private Dictionary<ECharacterType,KeyValuePair<EHumanStats,EHumanStats>> mHumanStatsPriorities;
	
	#region ACCESSORS
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static CharacterManager Instance
	{
		get
		{
			return mInstance;
		}
	}
	
	public ReadOnlyCollection<Character> Characters
	{
		get {return mCharactersReadOnly;}
	}

	#endregion
	
	#region MONO_METHODS
	
	void Awake()
	{
		if (mInstance == null)
		{

			mInstance = this;

			mCharacters = new List<Character>();
			mCharactersReadOnly = mCharacters.AsReadOnly();

			mHumanStatsPriorities = new Dictionary<ECharacterType,KeyValuePair<EHumanStats,EHumanStats>>();
			InitializeStatsPriorities();
		}
		else
		{
			Debug.Log("There is already an instance of the CharacterManager in the scene");
		}
	}
	
	#endregion
	
	#region CHARACTERS_MANAGEMENT
	
	public Character GetCharById(int aID)
	{
		Character returnValue = null;
		
		returnValue = mCharacters.Find(c => c.ID == aID);
		
		if (returnValue == null)
		{
			Debug.LogError("Could not find character with id : "+aID);
		}
		
		return returnValue;
	}
	
	#endregion

	#region HUMAN_STATS_PRIORITIES_MANAGEMENT

	public KeyValuePair<EHumanStats,EHumanStats> GetStatsPriority(ECharacterType aType)
	{
		KeyValuePair<EHumanStats,EHumanStats> returnValue = new KeyValuePair<EHumanStats, EHumanStats>();

		if (mHumanStatsPriorities.ContainsKey(aType))
		{
			returnValue = mHumanStatsPriorities[aType];
		}

		return returnValue;
	}

	void InitializeStatsPriorities()
	{
		//TODO Fetch from internet -CR
		mHumanStatsPriorities.Add(ECharacterType.Janitor,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
		mHumanStatsPriorities.Add(ECharacterType.Receptionist,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
		mHumanStatsPriorities.Add(ECharacterType.ResearchScientist,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
		mHumanStatsPriorities.Add(ECharacterType.SecurityGuard,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
		mHumanStatsPriorities.Add(ECharacterType.VirusScientist,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
		mHumanStatsPriorities.Add(ECharacterType.Worker,
		                          new KeyValuePair<EHumanStats,EHumanStats>(EHumanStats.Awareness,EHumanStats.Handiness));
	}

	#endregion
	
	#region RESET_INFORMATIONS
	
	public void ResetCharacters()
	{
		foreach(Character character in mCharacters)
		{
			Destroy(character.GameObject);
		}
		
		mCharacters.Clear();
	}
	
	#endregion

	/// <summary>
	/// Adds the character.
	/// </summary>
	/// <returns>
	/// The character.
	/// </returns>
	/// <param name='aType'>
	/// A type.
	/// </param>
	/// <param name='aRoom'>
	/// The working room of the character.
	/// </param>
	/// <param name='aRace'>
	/// The race of the character. Can be null if you want a random race.
	/// </param>
	public Human AddCharacter(ECharacterType aType,Room aRoom,ERace? aRace = null)
	{
		GameObject go = GraphicsManager.Instance.GetCharacter(aType,aRace);
		Human h = go.GetComponent<Human>();
		
		if (GameManager.Instance.UserStats.CanBuy(ECurrency.Gold,h.Salary))
		{
			GameManager.Instance.UserStats.RemoveMoney(ECurrency.Gold,h.Salary);

			h.Type = aType;
			h.JobStats.Job = aType;

			//
			WorkingRoom wr = aRoom as WorkingRoom;

			if (wr != null)
			{
				if (!wr.IsFullOfWorkers())
				{
					h.JobStats.WorkStation = wr;
				}
				else
				{
					Debug.LogError("Room is already full of workers");
				}
			}
			
			h.GenerateID();
			
			//Add ID of char debugging purposes
			h.GameObject.name = "Character"+ h.ID;

			//Initialize the character (will be called only one time)
			h.Initialize();
			
			//
			mCharacters.Add(h);
		}
		else
		{
			//TODO Find better way than creating/destroying the object.
			Destroy(go);
			h = null;
		}
		
		
		return h;
	}
	
	/// <summary>
	/// Adds the character.
	/// </summary>
	/// <param name='aInfo'>
	/// A info.
	/// </param>
	public void AddCharacter(CharacterSerializationInfo aInfo)
	{
		//
		Character character = GraphicsManager.Instance.GetCharacter(aInfo.mType).GetComponent<Character>();
		
		//
		character.Deserialize(aInfo);
		
		if(character != null)
		{
			mCharacters.Add(character);
		}
	}
	
	/// <summary>
	/// Removes the character.
	/// </summary>
	public void RemoveCharacter(Character aCharacter)
	{
		if(aCharacter != null)
		{
			mCharacters.Remove(aCharacter);
			
			//Remove from room;
			if (aCharacter.CurrentRoom != null)
			{
				aCharacter.CurrentRoom.RemoveCharacter(aCharacter);
			}
		}
	}
	
	#region ROOMS_MANAGEMENT
	
	public List<Character> GetCharactersInRoom(Room aRoom)
	{
		List<Character> returnValue = new List<Character>();
		
		//
		for(int i =0;i< mCharacters.Count;i++)
		{
			//
			if (mCharacters[i].CurrentRoom == aRoom)
			{
				returnValue.Add(mCharacters[i]);
			}
		}
		
		return returnValue;
	}

	public List<Character> GetCharactersOnFloor(int aFloor)
	{
		List<Character> returnValue = new List<Character>();
		
		//
		for(int i =0;i< mCharacters.Count;i++)
		{
			//
			if (mCharacters[i].GridPosition.y == aFloor)
			{
				returnValue.Add(mCharacters[i]);
			}
		}
		
		return returnValue;
	}
	
	#endregion
	
	#region JOB_MANAGEMENT
	
	public bool AssignJob(Human aHuman,ECharacterType aType,Room aRoom)
	{
		bool returnValue = false;
		
		//TODO prevent bad asignement : worker in virus lab -CR
		if (aType.IsWorkingCharacter())
		{
			WorkingRoom wr = aRoom as WorkingRoom;
			if ((wr == null )|| !(wr.IsFullOfWorkers()))
			{
				Vector3 humanPos = aHuman.LocalTransformPosition;
				
				Human human = AddCharacter(aType,aRoom,aHuman.Race) as Human;
				
				human.LocalTransformPosition = new Vector3(humanPos.x,humanPos.y,human.LocalTransformPosition.z);
				human.SetCharacterRoomAndGridPosition();
				wr.AddWorkingHuman(human);

				//
				CharacterManager.Instance.RemoveCharacter(aHuman);
				Destroy(aHuman.GameObject);

				//
				returnValue = true;
			}
			else
			{
				Debug.LogError("Cannot assign job there, Room is full of worker");
			}
		}
		else
		{
			Debug.LogError("Trying to assign a non working character to a working room " + aType.ToString());
		}
		
		return returnValue;
	}

	public bool AssignJob(Human aHuman,ECharacterType aType)
	{
		bool returnValue = false;
		
		//TODO prevent bad asignement : worker in virus lab -CR
		if (aType.IsWorkingCharacter())
		{
			Vector3 humanPos = aHuman.LocalTransformPosition;
			
			Human human = AddCharacter(aType,null,aHuman.Race) as Human;
			
			human.LocalTransformPosition = new Vector3(humanPos.x,humanPos.y,human.TransformPosition.z);
			human.SetCharacterRoomAndGridPosition();

			//
			CharacterManager.Instance.RemoveCharacter(aHuman);
			Destroy(aHuman.GameObject);

			returnValue = true;
		}
		else
		{
			Debug.LogError("Trying to assign a non working character to a working room " + aType.ToString());
		}
		
		return returnValue;
	}
	
	#endregion
}

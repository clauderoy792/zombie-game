using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphicsManager : MonoBehaviour {
	
	#region MEMBERS
	
	private static GraphicsManager sInstance;
	public GameObject mCharacterPrefab;
	public GameObject mZombiePrefab;
	public GameObject mJunkPrefab;
	public GameObject mBlueprint;
	
	public const float HUMAN_SIZE 				= 20f;
	public const float ZOMBIE_SIZE 				= 20f;
	public const float HUMAN_BOX_COLLIDER_SIZE 	= HUMAN_SIZE;
	
	public float mDepth = LAYER_TICKNESS;
	public const float LAYER_TICKNESS = 0.1f;

	private GameObject[] mRoomPrefabs;
	private Dictionary<ECharacterType, Dictionary<int, Sprite>> mCharacterSprites;
	private Dictionary<ECivilianClothingType, Dictionary<EClothingPart, Dictionary<int, Sprite>>> mCivilianClothingSprites;

	#endregion
	
	#region ACCESSORS
	
	public static GraphicsManager Instance
	{
		get {return sInstance;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
		
			//InitializeRoom prefabs
			mRoomPrefabs = new GameObject[(int)ERoomType.COUNT];
			for(int i = 0;i < mRoomPrefabs.Length;i++)
			{
				mRoomPrefabs[i] = Resources.Load("Prefabs/Rooms/"+((ERoomType)i).ToString()) as GameObject;
			}
		}
		else
		{
			Debug.LogError("There is already an instance of the GraphicsManager in the Scene.");
		}
	}

	void Start()
	{
		BuildCharactersSprites();
	}
	
	#endregion
	
	#region ROOMS_MANAGEMENT

	/// <summary>
	/// Gets the blueprint.
	/// </summary>
	/// <returns>The blueprint.</returns>
	/// <param name="aRoom">A room.</param>
	public Blueprint GetBlueprint(ERoomType aRoom)
	{
		GameObject result = Instantiate(mBlueprint) as GameObject;
		result.transform.parent = UIScreen2D.Game;
		result.transform.localScale = Vector3.one;

		//
		Blueprint bp = result.GetComponent<Blueprint>();
		bp.Initialize(aRoom);

		return bp;
	}

	/// <summary>
	/// Gets the GameObject of room for a certain type and size.
	/// </summary>
	/// <returns>
	/// The room.
	/// </returns>
	/// <param name='aType'>
	/// A type.
	/// </param>
	public GameObject GetRoom(ERoomType aType)
	{
		GameObject result = Instantiate(mRoomPrefabs[(int)aType]) as GameObject;

		result.transform.parent = UIScreen2D.Game;
		result.transform.localPosition = new Vector3(result.transform.localPosition.x, result.transform.localPosition.y, -mDepth);
		result.transform.localScale = Vector3.one;
		mDepth += LAYER_TICKNESS;
		
		return result;
	}
	
	
	/// <summary>
	/// Gets the GameObject of a random character.
	/// </summary>
	/// <returns>
	/// The random character.
	/// </returns>
	public GameObject GetRandomCharacter()
	{
		return GetCharacter((ECharacterType)Random.Range(0, (int)ECharacterType.COUNT));
	}
	
	
	/// <summary>
	/// Gets the character.
	/// </summary>
	/// <returns>
	/// The character.
	/// </returns>
	/// <param name='aCharacterType'>
	/// A character type.
	/// </param>
	/// <param name='aRace'>
	/// A race, can be null if you want a random race.
	/// </param>
	public GameObject GetCharacter(ECharacterType aCharacterType, ERace? aRace = null, EGender? aGender = null)
	{
		GameObject result = null;
		
		if (!aCharacterType.IsZombie())
		{
			result = GameObject.Instantiate(mCharacterPrefab) as GameObject;

			switch(aCharacterType)
			{
				case ECharacterType.Civilian:
					result.AddComponent<Civilian>();
					break;
				case ECharacterType.VirusScientist:
					result.AddComponent<VirusScientist>();
					break;
				case ECharacterType.ResearchScientist:
					result.AddComponent<ResearchScientist>();
					break;
				case ECharacterType.Janitor:
					result.AddComponent<Janitor>();
					break;
				case ECharacterType.SecurityGuard:
					result.AddComponent<SecurityGuard>();
					break;
				case ECharacterType.Worker:
					result.AddComponent<Worker>();
					break;
				case ECharacterType.Receptionist:
					result.AddComponent<Receptionist>();
					break;
			}
			
			//
			Human c = result.GetComponent<Human>();

			if (c != null)
			{
				//Race
				if (aRace != null)
				{
					c.SetRace(aRace.Value);
				}
				else
				{
					c.SetRace((ERace)Random.Range(0, (int)ERace.Count));
				}

				//Gender
				if (aGender != null)
				{
					c.SetGender(aGender.Value);
				}
				else
				{
					c.SetGender((EGender)Random.Range(0, (int)EGender.Count));
				}

				c.SetCharacterType(aCharacterType);
			}
			else
			{
				Debug.LogError("Failed to create human");
			}
		}
		else
		{
			result = GameObject.Instantiate(mZombiePrefab) as GameObject;
			//
			//TODO: HG
			//result.GetComponent<SpriteRenderer>().SetSprite(0);
			
			result.AddComponent<Zombie>();
		}

		//
		result.transform.parent = UIScreen2D.Game;
		result.transform.localPosition = new Vector3(result.transform.localPosition.x, result.transform.localPosition.y, -mDepth);
		result.transform.localScale = Vector3.one;
		mDepth += LAYER_TICKNESS;
		
		return result;
	}
	
	/// <summary>
	/// Gets the GameObject of a random zombie.
	/// </summary>
	/// <returns>
	/// The random zombie.
	/// </returns>
	public GameObject GetRandomZombie()
	{
		GameObject result = GameObject.Instantiate(mZombiePrefab) as GameObject;

		//
		//TODO: HG
		//result.GetComponent<SpriteRenderer>().SetSprite(0);
		
		result.AddComponent<Zombie>();

		//
		result.transform.parent = UIScreen2D.Game;
		result.transform.localPosition = new Vector3(result.transform.localPosition.x, result.transform.localPosition.y, -mDepth);
		result.transform.localScale = Vector3.one;
		mDepth += LAYER_TICKNESS;
		
		return result;
	}
	
	/// <summary>
	/// Gets the junk.
	/// </summary>
	/// <returns>
	/// The junk.
	/// </returns>
	public GameObject GetJunk()
	{
		GameObject result = GameObject.Instantiate(mJunkPrefab) as GameObject;

		//
		result.transform.parent = UIScreen2D.Game;
		result.transform.localPosition = new Vector3(result.transform.localPosition.x, result.transform.localPosition.y, -mDepth);
		result.transform.localScale = Vector3.one;
		mDepth += LAYER_TICKNESS;
		
		return result;
	}
	
	#endregion

	#region SPRITES_MANAGEMENT

	/// <summary>
	/// Gets the sprite for animation.
	/// </summary>
	/// <returns>The sprite for animation.</returns>
	/// <param name="aCharacterType">A character type.</param>
	/// <param name="aFrame">A frame. 1 = Idle, 2 = Walk, 3 = Flee. Second number is the frame number starting at 0. Ex: (24 = Get the fifth frame of walk animation)</param>
	public Sprite GetCostumeSpriteForAnimation(ECharacterType aCharacterType, int aFrame)
	{
		if(mCharacterSprites == null || !mCharacterSprites.ContainsKey(aCharacterType) || !mCharacterSprites[aCharacterType].ContainsKey(aFrame))
		{
			return null;
		}

		return mCharacterSprites[aCharacterType][aFrame];
	}

	public Sprite GetCivilianSprites(ECivilianClothingType aCivilianClothingType, EClothingPart aClothingPart, int aFrame)
	{
		return mCivilianClothingSprites[aCivilianClothingType][aClothingPart][aFrame];
	}

	// Initialization des sprites
	void BuildCharactersSprites()
	{
		Sprite[] sprites = Resources.LoadAll<Sprite>("Character/Human");

		#region JOB_COSTUMES

		//
		Dictionary<int, Sprite> virusScientistSprites = new Dictionary<int, Sprite>()
		{
			{10, sprites[6]},
			{11, sprites[7]},
			{20, sprites[8]},
			{21, sprites[9]},
			{30, sprites[10]},
			{31, sprites[11]},
		};

		//
		Dictionary<int, Sprite> securityGuardSprites = new Dictionary<int, Sprite>()
		{
			{10, sprites[12]},
			{11, sprites[13]},
			{20, sprites[14]},
			{21, sprites[15]},
			{30, sprites[16]},
			{31, sprites[17]},
		};

		//
		Dictionary<int, Sprite> janitorSprites = new Dictionary<int, Sprite>()
		{
			{10, sprites[18]},
			{11, sprites[19]},
			{20, sprites[20]},
			{21, sprites[21]},
			{30, sprites[22]},
			{31, sprites[23]},
		};

		//
		Dictionary<int, Sprite> workerSprites = new Dictionary<int, Sprite>()
		{
			{10, sprites[24]},
			{11, sprites[25]},
			{20, sprites[26]},
			{21, sprites[27]},
			{30, sprites[28]},
			{31, sprites[29]},
		};

		//
		Dictionary<int, Sprite> reasearchSprites = new Dictionary<int, Sprite>()
		{
			{10, sprites[30]},
			{11, sprites[31]},
			{20, sprites[32]},
			{21, sprites[33]},
			{30, sprites[34]},
			{31, sprites[35]},
		};
		
		//
		mCharacterSprites = new Dictionary<ECharacterType, Dictionary<int, Sprite>>()
		{
			{ECharacterType.VirusScientist, virusScientistSprites},
			{ECharacterType.SecurityGuard, securityGuardSprites},
			{ECharacterType.Janitor, janitorSprites},
			{ECharacterType.Worker, workerSprites},
			{ECharacterType.ResearchScientist, reasearchSprites},
		};

		#endregion

		#region CIVILIANS_CLOTHING
	
		//
		Dictionary<int, Sprite> casual01Shirts = new Dictionary<int, Sprite>()
		{
			//Shirts
			{10, sprites[37]},
			{11, sprites[38]},
			{20, sprites[41]},
			{21, sprites[42]},
			{30, sprites[45]},
			{31, sprites[46]},
		};

		Dictionary<int, Sprite> casual01Pants = new Dictionary<int, Sprite>()
		{
			{10, sprites[39]},
			{11, sprites[40]},
			{20, sprites[43]},
			{21, sprites[44]},
			{30, sprites[47]},
			{31, sprites[48]},
		};

		Dictionary<EClothingPart, Dictionary<int, Sprite>> casual01 = new Dictionary<EClothingPart, Dictionary<int, Sprite>>()
		{
			{EClothingPart.Shirt, casual01Shirts},
			{EClothingPart.Pants, casual01Pants},
		};

		mCivilianClothingSprites = new Dictionary<ECivilianClothingType, Dictionary<EClothingPart, Dictionary<int, Sprite>>>()
		{
			{ ECivilianClothingType.Casual_01, casual01 },
		};

		#endregion

	}

	#endregion
}

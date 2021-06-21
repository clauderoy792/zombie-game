using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InGame : MonoBehaviour {
	
	#region CONSTANTS
	
	private static Color UNABLE_TO_CREATE_ROOM_COLOR 	= RGBColor.FromRGB(255,144,144);
	private static Color ABLE_TO_CREATE_ROOM_COLOR 		= RGBColor.FromRGB(200,255,193);
	private static Color INITIAL_COLOR					= RGBColor.FromRGB(255,255,255);

	#endregion
	
	#region STATIC_MEMBERS
	
	private static InGame sInstance;
	
	#endregion
	
	#region PUBLIC_MEMBERS
	
	public Transform mCameraTransform;
	public GameObject mCharacter;
	public GameObject[] mRoomPrefabs;
	public GameObject mObstaclePrefab;
	public CameraMove mCameraMove;
	
	
	//TODO General stats -CR
	public int mNbRoomWidth = 5;
	public int mNbRoomHeight = 5;
	
	#endregion
	
	#region PRIVATE_MEMBERS
	
	private Dictionary<int,int> mIngredients;

	private int[] mVirusQuantities = new int[]{10,20,50,100};
	private int assignID = -1;
	private int mSelectedContractID = 0;
	private Ingredient[] mSelectedIngredients = new Ingredient[3];
	private string mResultVirus = "Result:";
	private Virus mCurrentVirus	= null;
	public Ingredient mCurrentIngredient = null;
	
	private Transform mCurrentTransform;
	private SpriteRenderer mCurrentRoomSprite;
	
	private float minX = 0;
    private float maxX = 99;
    private float minY = 0;
    private float maxY = 49;
	
	//Used for creating rooms
	private bool mIsShowingRoomMenu = false;
	private bool mIsCreatingRoom = false;
	private bool mIsShowingCharacterCreationMenu = false;
	private bool mIsShowingAvailableRooms = false;
	private bool mIsShowingSaveMenu = false;
	private bool mIsShowingExpandMenu = false;
	private bool mIsShowingSingleRoomMenu = false;
	private bool mIsShowingSearchForNewIngredients = false;
	private bool mIsShowingMakeKnownIngredients = false;
	private bool mIsShowingMakeVirus = false;
	private bool mIsShowingMakeKnownVirus = false;
	private bool mIsShowingCharacterMenu = false;
	private bool mIsShowingGameTime = false;
	private bool mIsSettingJob = false;
	private bool mIsContractMenuOpen = false;
	private bool mShowContractInfo = false;
	private bool mIsShowingVirusQuantities = false;
	private bool mIsShowingIngredientQuantities = false;

	//
	private ERoomType mCurrentType;
	private ECharacterType mDesiredCharacterType;
	private Room mCurrentRoom;
	private Character mCurrentCharacter;
	private int mRoomRectInitX;
	private GUIStyle blackStyle;
	
	//
	bool mIsRoomSelected;
	Vector2 mScrollPos;
	
	//
	Rect mRoomRect;
	
	#endregion
	
	#region ACCESSORS
	
	public static InGame Instance
	{
		get{return sInstance;}
	}
	
	#endregion
	
	#region MONO_METHODS
	
	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			blackStyle = new GUIStyle();
			blackStyle.normal.textColor = Color.black;
		}
		else
		{
			Debug.Log("There is already an instance of InGame in the scene.");
		}
	}
	
	// Use this for initialization
	void Start () {
		
		//
		InitializeBorders();
		
		mIsShowingSaveMenu = true;
		
		HCLocalization.Instance.InitializeData();
		
		mIngredients = GameManager.Instance.UserStats.Ingredients;
	}
	
	// Update is called once per frame
	void Update () 
	{
		ProcessRoomInputs();
	}
	
	void OnGUI()
	{
		//
		ShowUserStats();
		
		//
		if (mIsShowingSaveMenu)
		{
			//
			ShowSaveMenu();
		}
		else if (mIsShowingRoomMenu)
		{
			//
			ShowRoomMenu();
			
			//
			ShowActions();
		}
		else if(mIsShowingCharacterCreationMenu)
		{
			ShowCharacterCreationMenu();
			
			//
			ShowActions();
		}
		else if(mIsShowingAvailableRooms)
		{
			ShowAvailableRooms();
		}
		else if (mIsShowingExpandMenu)
		{
			ShowExpandMenu();
		}
		else if (mIsShowingSingleRoomMenu)
		{
			SingleRoomMenu();
		}
		else if (mIsShowingCharacterMenu)
		{
			ShowCharacterMenu();
		}
		else
		{
			//
			ShowMainMenu();
			
			//
			ShowActions();
		}
	}
	
	#endregion
	
	#region ROOM_MOVEMENT_MANAGEMENT
	
	/// <summary>
	/// Processes the room inputs.
	/// </summary>
	void ProcessRoomInputs()
	{
		if (mCurrentType != ERoomType.None && !mIsRoomSelected && InputManager.Instance.Inputs.IsTouchDown() && mRoomRect.Contains(InputManager.Instance.Inputs.GetWorldPosition()))
		{
			mCameraMove.canPan = false;
			mIsRoomSelected = true;
		}
		else if (InputManager.Instance.Inputs.IsTouchUp())
		{
			mCameraMove.canPan = true;
			mIsRoomSelected = false;
		}

		//
		if (mIsRoomSelected)
		{
			SetRoomPosition();	
		}
	}
	
	IEnumerator MoveToPath(List<Vector2> aPath,GameObject character)
	{
		foreach(Vector2 path in aPath)
		{	
			yield return new WaitForSeconds(0.5f);
			
			//offset path
			path.Set(path.x-0.5f,path.y);
			
			character.transform.position = path;
		}
	}
	
	#endregion
	
	#region PRIVATE_METHODS
	
	void SaveRoom()
	{
		//
		Rect realArrayPos = mRoomRect.ConvertToRoomPos();
		
		if (RoomManager.Instance.CanCreateRoom(realArrayPos,true))
		{
			//Set room z at 0
			mCurrentTransform.position = new Vector3(mCurrentTransform.position.x,mCurrentTransform.position.y,0);
			
			//
			RoomManager.Instance.AddRoom(new Vector2(realArrayPos.x,realArrayPos.y),mCurrentType);

			//
			mIsRoomSelected = false;
			mIsCreatingRoom = false;
			mCurrentType = ERoomType.None;
			
			//Delete current transform
			Destroy(mCurrentTransform.gameObject);
		}
		else
		{
			Debug.Log("CANT CREATE ROOM HERE !");
		}
	}
	
	#endregion
	
	#region ROOM_RECT_MANAGEMENT
	
	/// <summary>
	/// Updates the room rect to make it fit the transform position.
	/// </summary>
	void UpdateRoomRect()
	{
		//
		bool posChanged =(mRoomRect.x != mCurrentTransform.position.x - mRoomRect.width/2 || 
			mRoomRect.y != mCurrentTransform.position.y-mRoomRect.height/2);
		
		//Rect pos start by top left corner, so we have to do -
		mRoomRect.Set(mCurrentTransform.position.x,mCurrentTransform.position.y,mRoomRect.width,mRoomRect.height);

		//Check if we can create a room in the new pos
		if (posChanged)
		{
			//
			if (RoomManager.Instance.CanCreateRoom(mRoomRect.ConvertToRoomPos()))
			{
				mCurrentRoomSprite.color = ABLE_TO_CREATE_ROOM_COLOR;
			}
			//
			else
			{
				mCurrentRoomSprite.color = UNABLE_TO_CREATE_ROOM_COLOR;
			}
		}
	}
	
	/// <summary>
	/// Sets the room position so it cannot be out of the grid and that it respects
	/// minimum cell width.
	/// </summary>
	void SetRoomPosition()
	{
		/*Vector2 mousePos = InputManager.Instance.Inputs.GetWorldPosition();
		Debug.Log ("MOUSE POS : "+mousePos);
		//Clamp position, to permit only move by the minimum room size.
		Vector3 tempPos = new Vector3((int)mousePos.x,Mathf.CeilToInt(mousePos.y) ,mCurrentTransform.position.z);
		Vector3 transformPos = tempPos;
		
		//set position needed to compare with minimum, to prevent from going outside of the array.
		tempPos.Set(tempPos.x-mRoomRect.width/2,tempPos.y+mRoomRect.height/2,tempPos.z);
		
		//Check if position is in grid bounds.
		if (tempPos.x >= minX && tempPos.x +mRoomRect.width < maxX && tempPos.y >= minY && tempPos.y < maxY)
		{
			Debug.Log("SetRoomPosition  : "+transformPos);
			//
			mCurrentTransform.localPosition = transformPos;

			//
			UpdateRoomRect(); 
		}*/
	}
	
	void InitRoomRect()
	{
		mRoomRectInitX = (int)mCameraTransform.position.x;
		
		if (mRoomRectInitX % Room.UNIT_CELL_WIDTH != 0)
		{
			mRoomRectInitX -= Room.UNIT_CELL_WIDTH/2;
		}
		
		mRoomRect.Set(mRoomRectInitX,Mathf.RoundToInt(mCameraTransform.position.y),Room.UNIT_CELL_WIDTH,Room.UNIT_CELL_HEIGHT);	
	}
	
	void InitRoom()
	{
		//
		InitRoomRect();

		//Set room transform position
		mCurrentTransform.localPosition = new Vector3(mRoomRectInitX,Mathf.RoundToInt(mCameraTransform.position.y),-1);

		//
		UpdateRoomRect();

		//
		mIsShowingRoomMenu = false;
	}
	
	#endregion
	
	#region GUI_FUNCTIONS
	
	void ShowUserStats()
	{
		//Time
		if (mIsShowingGameTime)
		{
			GUI.Label(new Rect(Screen.width - 125,10,120,50)
				,String.Format("Y{0} M{1} W{2}",TimeManager.Instance.CurrentTime.Year,TimeManager.Instance.CurrentTime.Month,TimeManager.Instance.CurrentTime.Week)
				,blackStyle);

			if(GUI.Button(new Rect(Screen.width - 170, 30, 150, 50), "Contract"))
			{
				mIsContractMenuOpen = !mIsContractMenuOpen;

				if (mIsContractMenuOpen)
				{
					//Open menu
					GameManager.Instance.PauseGame(true);
					GameManager.Instance.UserStats.ContractTimer.Stop();
				}
				else
				{
					//Close menu
					GameManager.Instance.UserStats.ContractTimer.Start();
					GameManager.Instance.PauseGame(false);
				}
			}
		}

		//Display stats
		GUI.Label(new Rect(230,10,100,30), "Gold : "+ GameManager.Instance.UserStats.Gold,blackStyle);
		GUI.Label(new Rect(230,40,100,30), "Brainz : "+ GameManager.Instance.UserStats.Brainz,blackStyle);
		GUI.Label(new Rect(230,70,200,30), "Zombies Sold : " +GameManager.Instance.UserStats.ZombiesSold,blackStyle);

		//Display zombies
		int initY = 10;
		int y = initY;
		int zombiesPerRow = 5;
		int j = 0;
		int previousRow = 0;
		foreach(var pair in GameManager.Instance.UserStats.Zombies)
		{
			int row = 1+(j/zombiesPerRow);

			if (row != previousRow)
			{
				y = initY;
			}
			previousRow = row;
			GUI.Label(new Rect(350+(row-1)*220,y,100,30), "Zombie : "+pair.Key.ToString()+", Quantity : "+pair.Value,blackStyle);
			y+= 30;
			j++;
		}

		//
		int pos = 0;
		if(mIsShowingGameTime && mIsContractMenuOpen)
		{
			for(int i = 0; i < GameManager.Instance.UserStats.Contracts.Length; i++)
			{
				Contract c = GameManager.Instance.UserStats.Contracts[i];

				if (c != null)
				{

					GUI.BeginGroup(new Rect(Screen.width - 170, (pos*80) + 80, 150, 80));

					if(GUI.Button(new Rect(0,0,150,80), "", "Box"))
					{
						mShowContractInfo = true;
						mIsContractMenuOpen = false;
						mSelectedContractID = i;
					}

					GUI.Label(new Rect(5,5,100,20), c.CompanyName);
					GUI.Label(new Rect(5,55,100,20), string.Format("Pay: {0}", c.Pay));

					GUI.EndGroup();
					pos++;
				}
			}
		}

		if(mIsShowingGameTime && mShowContractInfo)
		{
			mCameraMove.canPan = false;

			float width = Screen.width;
			float height = Screen.height; 

			GUILayout.BeginArea(new Rect(width/4, height/4, width/2, height/2), "", "BOX");

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("X"))
			{
				mShowContractInfo = false;

				//Close menu
				GameManager.Instance.UserStats.ContractTimer.Start();
				GameManager.Instance.PauseGame(false);
			}
			GUILayout.EndHorizontal();

			mScrollPos = GUILayout.BeginScrollView(mScrollPos);
			GUILayout.Label(GameManager.Instance.UserStats.Contracts[mSelectedContractID].ToString());
			GUILayout.EndScrollView();

			//Buttons
			GUILayout.BeginHorizontal();

			string pinText = GameManager.Instance.UserStats.Contracts[mSelectedContractID].Keep ? "Unpin" : "Pin";

			if(GUILayout.Button(pinText))
			{
				GameManager.Instance.UserStats.Contracts[mSelectedContractID].Keep = !GameManager.Instance.UserStats.Contracts[mSelectedContractID].Keep;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Remove"))
			{
				GameManager.Instance.UserStats.RemoveContract(mSelectedContractID);

				//Close menu
				GameManager.Instance.UserStats.ContractTimer.Start();
				GameManager.Instance.PauseGame(false);
			}

			bool accepted = GameManager.Instance.UserStats.Contracts[mSelectedContractID].Status == EContractStatus.ACCEPTED;

			//If we can't complete contract.
			if (accepted && !GameManager.Instance.UserStats.Contracts[mSelectedContractID].CanCompleteContract())
			{
				GUI.enabled = false;
			}

			if(GUILayout.Button(accepted ? "Complete" : "Accept"))
			{
				if (accepted)
				{
					GameManager.Instance.UserStats.CompleteContract(mSelectedContractID);
				}
				else
				{
					GameManager.Instance.UserStats.Contracts[mSelectedContractID].AcceptContract();
				}

				mShowContractInfo = false;

				//Close menu
				GameManager.Instance.UserStats.ContractTimer.Start();
				GameManager.Instance.PauseGame(false);
			}

			GUI.enabled = true;

			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}
		else if (!mIsRoomSelected)
		{
			mCameraMove.canPan = true;
		}
	}
	
	void ShowMainMenu()
	{
		
		GUI.enabled = !mIsCreatingRoom;
		
		if (GUI.Button(new Rect(10,110,100,50),"Create Zombie"))
		{
			GraphicsManager.Instance.GetRandomZombie();
		}
		
		GUI.enabled = mIsCreatingRoom;
		
		if (GUI.Button(new Rect(10,160,100,50),"Cancel Room Creation"))
		{
			//
			mIsRoomSelected = false;
			mIsCreatingRoom = false;
			mCurrentType = ERoomType.None;
			
			//
			Destroy(mCurrentTransform.gameObject);
		}
		
		GUI.enabled = true;
		
		if (GUI.Button(new Rect(10,210,100,50),"Expand"))
		{
			mIsShowingExpandMenu = true;
		}
		
		if (GUI.Button(new Rect(10,260,100,50),"Save Game"))
		{
			SaveGameManager.SaveGame();
		}
		
	}
	
	void ShowRoomMenu()
	{
		//Rooms
		if (GUI.Button(new Rect(10,10,100,50),"Research Lab"))
		{
			//
			mCurrentType = ERoomType.ResearchLab;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,60,100,50),"Virus Lab"))
		{
			//
			mCurrentType = ERoomType.VirusLab;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,110,100,50),"Cafeteria"))
		{
			//
			mCurrentType = ERoomType.Cafeteria;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,160,100,50),"Assembly Line"))
		{
			//
			mCurrentType = ERoomType.AssemblyLine;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,210,100,50),"Reception"))
		{
			//
			mCurrentType = ERoomType.Reception;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,260,100,50),"Bathroom"))
		{
			//
			mCurrentType = ERoomType.Bathroom;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,310,100,50),"Zomie Warehouse"))
		{
			//
			mCurrentType = ERoomType.ZombieWarehouse;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,360,100,50),"Elevator"))
		{
			//
			mCurrentType = ERoomType.Elevator;
			
			//
			GameObject go = GraphicsManager.Instance.GetRoom(mCurrentType);
			mCurrentRoomSprite = go.GetComponent<SpriteRenderer>();
			mCurrentTransform = go.transform;
			
			//
			InitRoom();
		}
		else if (GUI.Button(new Rect(10,410,100,50),"Cancel"))
		{
			//
			mIsShowingRoomMenu = false;
			
			//
			mIsRoomSelected = false;
			mIsCreatingRoom = false;
			mCurrentType = ERoomType.None;
		}
	}
	
	//
	void ShowCharacterCreationMenu()
	{
		if (GUI.Button(new Rect(10,10,100,50),"Virus Scientist"))
		{
			mDesiredCharacterType = ECharacterType.VirusScientist;
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;
			mIsShowingAvailableRooms = true;
		}
		else if (GUI.Button(new Rect(10,60,100,50),"Research Scientist"))
		{
			mDesiredCharacterType = ECharacterType.ResearchScientist;
			mIsShowingAvailableRooms = true;
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;
		}
		else if (GUI.Button(new Rect(10,110,100,50),"Janitor"))
		{
			mDesiredCharacterType = ECharacterType.Janitor;
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;

			//Assign job
			if (CharacterManager.Instance.AssignJob(mCurrentCharacter as Human ,mDesiredCharacterType))
			{
				mIsSettingJob = false;
				mIsShowingAvailableRooms = false;
				
				Civilian civ = mCurrentCharacter as Civilian;
				if (civ != null)
				{
					civ.RemoveFromReception();
				}
			}
		}
		else if (GUI.Button(new Rect(10,160,100,50),"Security Guard"))
		{
			mDesiredCharacterType = ECharacterType.SecurityGuard;
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;

			//Assign job
			if (CharacterManager.Instance.AssignJob(mCurrentCharacter as Human ,mDesiredCharacterType))
			{
				mIsSettingJob = false;
				mIsShowingAvailableRooms = false;
				
				Civilian civ = mCurrentCharacter as Civilian;
				if (civ != null)
				{
					civ.RemoveFromReception();
				}
			}
		}
		else if (GUI.Button(new Rect(10,210,100,50),"Worker"))
		{
			mDesiredCharacterType = ECharacterType.Worker;
			mIsShowingAvailableRooms = true;
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;
		}
		else if (GUI.Button(new Rect(10,260,100,50),"CANCEL"))
		{
			mIsShowingRoomMenu = false;
			mIsShowingCharacterCreationMenu = false;
			mIsShowingAvailableRooms = false;
		}
	}
	
	#region CHARACTERS MENU
	
	void ShowCharacterMenu()
	{
		GUI.Label(new Rect(10,10,100,50),"Name :" +mCurrentCharacter.Name,blackStyle);
		GUI.Label(new Rect(10,40,100,50),"Type :" +mCurrentCharacter.Type,blackStyle);
		
		if (mCurrentCharacter is Civilian)
		{
			ShowCivilianMenu();
		}
		else if (mCurrentCharacter is Human)
		{
			HumanStats stats = (mCurrentCharacter as Human).Stats;
			GUI.Label(new Rect(10,70,150,50),"Stats",blackStyle);
			GUI.Label(new Rect(10,100,150,50),"Intellect :" +stats.Intellect,blackStyle);
			GUI.Label(new Rect(10,130,150,50),"Handiness :" +stats.Handiness,blackStyle);
			GUI.Label(new Rect(10,160,150,50),"Awareness :" +stats.Awareness,blackStyle);
			GUI.Label(new Rect(10,190,150,50),"Exhaustion :" +stats.Exhaustion,blackStyle);
			GUI.Label(new Rect(10,220,150,50),"Toilet :" +stats.Toilet,blackStyle);
			GUI.Label(new Rect(10,250,150,50),"Hungriness :" +stats.Hungriness,blackStyle);
			
			if (GUI.Button(new Rect(10,280,100,50),"Assign New Job"))
			{
				mIsShowingCharacterCreationMenu = true;
				mIsShowingCharacterMenu = false;
			}
			else if (GUI.Button(new Rect(10,330,100,50),"Fire"))
			{
				(mCurrentCharacter as Human).Fire();
				mIsShowingCharacterMenu = false;
			}
			else if (GUI.Button(new Rect(10,380,100,50),"Cancel"))
			{
				mIsShowingCharacterMenu = false;
			}
		}
		else
		{
			if (GUI.Button(new Rect(10,70,100,50),"Cancel"))
			{
				mIsShowingCharacterMenu = false;
			}
		}
		
	}
	
	void ShowCivilianMenu()
	{
		Civilian civilian = mCurrentCharacter as Civilian;
		HumanStats stats = civilian.Stats;
		GUI.Label(new Rect(10,100,150,50),"Intellect :" +stats.Intellect,blackStyle);
		GUI.Label(new Rect(10,130,150,50),"Handiness :" +stats.Handiness,blackStyle);
		GUI.Label(new Rect(10,160,150,50),"Awareness :" +stats.Awareness,blackStyle);
	
		
		if (civilian.IsWaitingAtReception)
		{
			civilian.SetCanLeave(false);
			if (GUI.Button(new Rect(10,190,100,50),"Assign Job"))
			{
				mIsShowingCharacterCreationMenu = true;
				mIsShowingCharacterMenu = false;
			}
			if (GUI.Button(new Rect(10,240,100,50),"Kick Out"))
			{
				(mCurrentCharacter as Civilian).GoHome();
				mIsShowingCharacterMenu = false;
			}
			if (GUI.Button(new Rect(10,290,100,50),"Cancel"))
			{
				mIsShowingCharacterMenu = false;
				//civilian.SetCanLeave(true);
			}
		}
		else
		{
			if (GUI.Button(new Rect(10,190,100,50),"Cancel"))
			{
				mIsShowingCharacterMenu = false;
			}
		}
	}
	
	#endregion
	
	//
	void ShowAvailableRooms()
	{
		mIsSettingJob = true;
		
		//
		SetRoomsColorForJob(mDesiredCharacterType);
		
		if (GUI.Button(new Rect(10,10,100,50),"Cancel"))
		{
			ResetRoomsColor();
			mIsSettingJob = false;
			mIsShowingAvailableRooms = false;
		}
	}
	
	//
	void ShowWorkingRooms()
	{
		float marginY = 30;
		float currentMargin = 0;
		List<WorkingRoom> availableRooms = RoomManager.Instance.GetAvailableWorkingRooms();
		
		
		if (availableRooms != null)
		{
			for(int i = 0; i < availableRooms.Count; i++)
			{
				WorkingRoom r = availableRooms[i];
				
				if (!r.IsFullOfWorkers())
				{
					if (GUI.Button(new Rect(10,10 + currentMargin,150,30), r.Type.ToString() + " (" + r.HumansWorkingInRoom.Count + "/" + r.MaxNumberOfWorkers + ")  - " + r.GridPosition))
					{
						//
						r.AddWorkingHuman(mCurrentCharacter as Human);
					}
					
					currentMargin += marginY;
				}
			}
		}
		
		if (GUI.Button(new Rect(10,10+currentMargin,100,50),"Cancel"))
		{
			mIsShowingAvailableRooms = false;
		}
	}
	
	void ShowSaveMenu()
	{
		//New Game
		if (GUI.Button(new Rect(10,10,100,50),"New Game"))
		{
			mIsShowingSaveMenu = false;

			//
			RoomManager.Instance.InitializeNewGame();
			
			TimeManager.Instance.StartUpdatingGameTime();
			
			mIsShowingGameTime = true;
		}
		
		//Load Game
		if (GUI.Button(new Rect(10,60,100,50),"Load Game"))
		{
			SaveGameManager.LoadGame();
			mIsShowingSaveMenu = false;
			mIsShowingGameTime = true;
		}
	}
	
	void ShowExpandMenu()
	{
		int rightExpandCost=  BuildingManager.Instance.GetExpandCost(BuildingManager.EExpandDirection.Right);
		int leftExpandCost=  BuildingManager.Instance.GetExpandCost(BuildingManager.EExpandDirection.Left);
		int topExpandCost=  BuildingManager.Instance.GetExpandCost(BuildingManager.EExpandDirection.Top);
		
		GUI.enabled = GameManager.Instance.UserStats.CanBuy( ECurrency.Gold,leftExpandCost);
		
		if (GUI.Button(new Rect(10,10,150,50),"Left : " + leftExpandCost))
		{
			BuildingManager.Instance.Expand(BuildingManager.EExpandDirection.Left);
		}
		
		GUI.enabled = GameManager.Instance.UserStats.CanBuy(ECurrency.Gold,topExpandCost);
		if (GUI.Button(new Rect(10,60,150,50),"Top : "+ topExpandCost))
		{
			BuildingManager.Instance.Expand(BuildingManager.EExpandDirection.Top);
		}
		GUI.enabled = GameManager.Instance.UserStats.CanBuy(ECurrency.Gold,rightExpandCost);
		if (GUI.Button(new Rect(10,110,150,50),"Right : "+ rightExpandCost))
		{
			BuildingManager.Instance.Expand(BuildingManager.EExpandDirection.Right);
		}
		
		GUI.enabled = true;
		if (GUI.Button(new Rect(10,160,150,50),"Back"))
		{
			mIsShowingExpandMenu = false;
		}
	}
	
	void ShowActions()
	{
		
	}
	
	#endregion
	
	#region SPECIFIC_ROOM_GUI
	
	void SingleRoomMenu()
	{
		switch(mCurrentRoom.Type)
		{
			case ERoomType.ResearchLab:
				ResearchLabUI();
				break;
			case ERoomType.VirusLab:
				VirusLabUI();
				break;
			case ERoomType.AssemblyLine:
				AssemblyLineUI();
				break;
			default:
				if (GUI.Button(new Rect(10,10,100,50),"Cancel"))
				{
					mIsShowingSingleRoomMenu = false;
				}
			break;
		}
	}

	//
	void ResearchLabUI()
	{
		if (mIsShowingIngredientQuantities)
		{
			IngredientQuantitiesUI();
		}
		else if (mIsShowingMakeKnownIngredients)
		{
			MakeKnownIngredientsUI();
		}
		else if (mIsShowingSearchForNewIngredients)
		{
			SearchForNewIngredientUI();
		}
		else
		{
			if (GUI.Button(new Rect(10,10,150,50),"Search for ingredients."))
			{
				ResearchLab lab = mCurrentRoom as ResearchLab;
				if (lab != null)
				{
					lab.IsSearchingForNewIngredient =true;
					lab.StartWork();
				}
				mIsShowingSingleRoomMenu = false;
			}
			
			if (GUI.Button(new Rect(10,60,150,50),"Make known ingredients."))
			{
				mIsShowingMakeKnownIngredients = true;
			}
			
			if (GUI.Button(new Rect(10,110,100,50),"Cancel"))
			{
				mIsShowingSingleRoomMenu = false;
			}
		}
	}

	void SearchForNewIngredientUI()
	{
		int y = 10;
		int intervalY = 30;
		
		int i = 0;
		foreach(var pair in GameManager.Instance.UserStats.Ingredients)
		{
			if (pair.Value > 0)
			{
				Ingredient ing = CraftingManager.Instance.GetIngredientForId(pair.Key);
				if(GUI.Button(new Rect(10,y+i*intervalY,150,intervalY),ing.Name+" : "+pair.Value))
				{
					GameManager.Instance.UserStats.AddIngredient(pair.Key);
				}
				i++;
			}
			
		}
		
		if(GUI.Button(new Rect(10,y+i*intervalY,150,intervalY),"Cancel"))
		{
			mIsShowingSearchForNewIngredients = false;
		}
	}
	
	void MakeKnownIngredientsUI()
	{
		int y = 10;
		int intervalY = 30;
		
		int i = 0;
		foreach(var pair in GameManager.Instance.UserStats.Ingredients)
		{
			if (pair.Value > 0)
			{
				Ingredient ing = CraftingManager.Instance.GetIngredientForId(pair.Key);
				if(GUI.Button(new Rect(10,y+i*intervalY,150,intervalY),ing.Name+" : "+pair.Value))
				{
					mCurrentIngredient = ing;
					mIsShowingIngredientQuantities = true;
				}
				i++;
			}
			
		}
		
		if(GUI.Button(new Rect(10,y+i*intervalY,150,intervalY),"Cancel"))
		{
			mIsShowingMakeKnownIngredients = false;
		}
	}

	void IngredientQuantitiesUI()
	{
		for(int i = 0;i<mVirusQuantities.Length;i++)
		{
			if (GUI.Button(new Rect(10,i*50+10,100,50),"x"+mVirusQuantities[i].ToString()))
			{
				ResearchLab lab = mCurrentRoom as ResearchLab;

				lab.SetCurrentIngredientAndQuantity(mCurrentIngredient,mVirusQuantities[i]);
				lab.StartWork();
				mIsShowingIngredientQuantities = false;
			}
		}
		
		//
		if (GUI.Button(new Rect(130,300,100,50),"Cancel"))
		{
			mIsShowingIngredientQuantities = false;
		}
	}

	void VirusLabUI()
	{
		if (mIsShowingVirusQuantities)
		{
			ShowVirusQuantitiesUI();
		}
		else if (mIsShowingMakeVirus)
		{
			MakeVirusUI();
		}
		else if (mIsShowingMakeKnownVirus)
		{
			MakeKnownVirusUI();
		}
		else
		{
			GUILayout.Space(110);

			if (GUI.Button(new Rect(10,10,150,50),"Make Known Virus."))
			{
				mIsShowingMakeKnownVirus = true;
			}

			if (GUI.Button(new Rect(10,60,150,50),"Craft New Virus."))
			{
				mIsShowingMakeVirus = true;
			}
			
			if (GUI.Button(new Rect(10,110,100,50),"Cancel"))
			{
				mIsShowingSingleRoomMenu = false;
			}
		}
	}

	void AssemblyLineUI()
	{
		int maxNbVirusPerLine = 10;
		int currentNbVirusPerLine = 0;

		float x = 10;
		float y = 10;
		foreach(var virus in GameManager.Instance.UserStats.Viruses)
		{
			Virus v = CraftingManager.Instance.GetVirusForId(virus.Key);

			if (GUI.Button(new Rect(x,y,150,50),v.Name+ " : "+virus.Value))
			{
				AssemblyLine line = mCurrentRoom as AssemblyLine;

				if (line != null)
				{
					line.CurrentVirus = v;
					line.StartWork();
				}

				mIsShowingSingleRoomMenu = false;
			}

			//
			currentNbVirusPerLine++;

			//
			if(currentNbVirusPerLine == maxNbVirusPerLine)
			{
				x+= 160;
				y = 0;
				currentNbVirusPerLine = 0;
			}
			else
			{
				y += 60;
			}
		}

		if (GUI.Button(new Rect(10,210,150,50),"Cancel"))
		{
			mIsShowingSingleRoomMenu = false;
		}
	}
	
	void MakeVirusUI()
	{
		//
		if(GUI.Button(new Rect(150, 145, 120, 30), "Assign 1"))
		{
			assignID = 0;
		}
		
		if(GUI.Button(new Rect(280, 145, 120, 30), "Assign 2"))
		{
			assignID = 1;
		}
		
		if(GUI.Button(new Rect(410, 145, 120, 30), "Assign 3"))
		{
			assignID = 2;
		}
		
		//
		GUI.Box(new Rect(150, 20, 120, 120), mSelectedIngredients[0] != null ? mSelectedIngredients[0].Name : "");
		GUI.Box(new Rect(280, 20, 120, 120), mSelectedIngredients[1] != null ? mSelectedIngredients[1].Name : "");
		GUI.Box(new Rect(410, 20, 120, 120), mSelectedIngredients[2] != null ? mSelectedIngredients[2].Name : "");
		GUI.Box(new Rect(540, 20, 120, 120), mResultVirus);
		
		//
		if(mSelectedIngredients[0] != null && mSelectedIngredients[1] != null && mSelectedIngredients[2] != null)
		{
			if(GUI.Button(new Rect(540, 145, 120, 30), "Mix"))
			{
				Virus result = CraftingManager.Instance.CreateVirus(mSelectedIngredients[0], mSelectedIngredients[1], mSelectedIngredients[2]);
				VirusLab lab = mCurrentRoom as VirusLab;
				if (result != null)
				{
					mResultVirus = string.Format("Result:\n\nName: {0}\nZombie Type: {1}", result.Name, result.ZombieType);

					if (lab != null)
					{
						lab.CurrentVirus = result;
						lab.StartWork();
					}
				}
				else
				{
					Debug.Log("Virus created is null");
				}
			}
		}
		
		//
		if(assignID >= 0 && assignID < 3)
		{
			foreach(var pair in mIngredients)
			{
				if (pair.Value > 0)
				{
					Ingredient ing = CraftingManager.Instance.GetIngredientForId(pair.Key);

					if (ing != null)
					{
						if(GUILayout.Button(ing.Name +" : "+GameManager.Instance.UserStats.Ingredients[ing.ID], GUILayout.Width(120), GUILayout.Height(30)))
						{
							if(assignID == 0)
							{
								mSelectedIngredients[0] = ing;
							}
							else if(assignID == 1)
							{
								mSelectedIngredients[1] = ing;
							}
							else if(assignID == 2)
							{
								mSelectedIngredients[2] = ing;
							}
							
							//
							assignID = -1;
						}
					}
				}
			}
		}
		
		if (GUI.Button(new Rect(130,300,100,50),"Cancel"))
		{
			mIsShowingMakeVirus = false;
		}
	}

	void MakeKnownVirusUI()
	{
		float x = 10;
		float y = 10;
		int maxNbPerRow = 10;
		int currentNbPerRow = 0;

		foreach(var virus in GameManager.Instance.UserStats.Viruses)
		{
			Virus v = CraftingManager.Instance.GetVirusForId(virus.Key);

			if (v != null)
			{
				if (GUI.Button(new Rect(x,y,150,50),v.Name + " : "+v.ZombieType))
				{
					mCurrentVirus = v;
					mIsShowingMakeKnownVirus = false;
					mIsShowingVirusQuantities = true;
				}
			}
			currentNbPerRow++;

			if (currentNbPerRow == maxNbPerRow)
			{
				currentNbPerRow = 0;
				x += 160;
				y = 0;
			}
			else
			{
				y+= 60;
			}
		}
		
		if (GUI.Button(new Rect(130,300,100,50),"Cancel"))
		{
			mIsShowingMakeKnownVirus = false;
		}
	}

	void ShowVirusQuantitiesUI()
	{
		for(int i = 0;i<mVirusQuantities.Length;i++)
		{
			if (GUI.Button(new Rect(10,i*50+10,100,50),"x"+mVirusQuantities[i].ToString()))
			{
				VirusLab lab = mCurrentRoom as VirusLab;
				
				if (lab != null && GameManager.Instance.UserStats.CanProduce(mCurrentVirus,mVirusQuantities[i]))
				{
					lab.SetCurrentVirusAndQuantity(mCurrentVirus,mVirusQuantities[i]);
					lab.StartWork();
					mIsShowingVirusQuantities = false;
				}
			}
		}

		//
		if (GUI.Button(new Rect(130,300,100,50),"Cancel"))
		{
			mIsShowingVirusQuantities = false;
		}
	}
	
	#endregion
	
	#region ROOM_CREATION_BOUDARIES_MANAGEMENT
	
	private void InitializeBorders()
	{
		minX = (PathFinder.GRID_WIDTH/2-mNbRoomWidth/2)*Room.UNIT_CELL_WIDTH;


		//Make sure to have a a position that fits the room width
		// not in the center of a room unit.
		if (minX % Room.UNIT_CELL_WIDTH != 0)
		{
			minX -= Room.UNIT_CELL_WIDTH/2;
		}
		
		//Prevents people from creating room outside the bounds.
		maxX = minX + mNbRoomWidth*Room.UNIT_CELL_WIDTH + Room.UNIT_CELL_WIDTH;
		minY = 0;
		maxY = mNbRoomHeight;
		
		mCameraTransform.position = new Vector3(PathFinder.GRID_WIDTH*Room.UNIT_CELL_WIDTH/2,minY,mCameraTransform.position.z);
		
		//
		mCameraMove.minX = minX+3;
		mCameraMove.maxX = maxX-5;
		mCameraMove.maxY = maxY-2;

		BuildingManager.Instance.SetBuildingDimensions((int)minX,mNbRoomWidth,mNbRoomHeight);
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public void ShowRoom(Room aRoom)
	{
		if (mIsSettingJob && aRoom.Sprite.color == ABLE_TO_CREATE_ROOM_COLOR)
		{
			if (mCurrentCharacter is Human)
			{
				if (CharacterManager.Instance.AssignJob(mCurrentCharacter as Human ,mDesiredCharacterType,aRoom as WorkingRoom))
				{
					mIsSettingJob = false;
					mIsShowingAvailableRooms = false;
					
					Civilian civ = mCurrentCharacter as Civilian;
					if (civ != null)
					{
						civ.RemoveFromReception();
					}
					
					ResetRoomsColor();
				}
			}
			else
			{
				Debug.LogError("Cannot assign job to this type of character : "+mCurrentCharacter.GetType().ToString());
			}
		}
		else if (!mIsCreatingRoom)
		{
			mIsShowingCharacterMenu = false;
			mIsShowingSingleRoomMenu = true;
			mCurrentRoom = aRoom;
		}
	}
	
	public void ShowCharacter(Character aCharacter)
	{
		if (!mIsCreatingRoom)
		{
			mIsShowingSingleRoomMenu = false;
			mIsShowingCharacterMenu = true;
			mCurrentCharacter = aCharacter;
		}
	}
	
	public void UpdateRoomCreationBounds()
	{
		//Prevents people from creating room outside the bounds.
		minX = BuildingManager.Instance.LeftBorder;
		maxX = minX + BuildingManager.Instance.BuildingWidth*Room.UNIT_CELL_WIDTH + Room.UNIT_CELL_WIDTH;
		maxY = BuildingManager.Instance.BuildingHeight;
		
		//Prevents pan.
		mCameraMove.minX = minX+3;
		mCameraMove.maxX = maxX-5;
		mCameraMove.maxY = maxY-2;
	}
	
	#endregion
	
	#region ASSIGN_JOB_MANAGEMENT
	
	void SetRoomsColorForJob(ECharacterType aType)
	{
		foreach(Room r in RoomManager.Instance.Rooms)
		{
			WorkingRoom wr = r as WorkingRoom;
			
			if (wr != null && !wr.IsFullOfWorkers() && wr.Type.GetJob() == aType)
			{
				r.Sprite.color = ABLE_TO_CREATE_ROOM_COLOR;
			}
			else
			{
				r.Sprite.color = UNABLE_TO_CREATE_ROOM_COLOR;
			}
		}
	}
	
	void ResetRoomsColor()
	{
		foreach(Room r in RoomManager.Instance.Rooms)
		{
			r.Sprite.color = INITIAL_COLOR;
		}
	}
	
	#endregion
	
	List<ERoomType> GetAvailableRooms()
	{
		var availableRooms = new List<ERoomType>();
		
		switch(mDesiredCharacterType)
		{
			case ECharacterType.Janitor:
			case ECharacterType.SecurityGuard:
				availableRooms.Add(ERoomType.Corridor);
				availableRooms.Add(ERoomType.AssemblyLine);
				availableRooms.Add(ERoomType.Bathroom);
				availableRooms.Add(ERoomType.Cafeteria);
				availableRooms.Add(ERoomType.Corridor);
				availableRooms.Add(ERoomType.Elevator);
				availableRooms.Add(ERoomType.Reception);
				availableRooms.Add(ERoomType.ResearchLab);
				availableRooms.Add(ERoomType.VirusLab);
				availableRooms.Add(ERoomType.ZombieWarehouse);
				break;
			case ECharacterType.Worker:
				availableRooms.Add(ERoomType.AssemblyLine);
				break;
			case ECharacterType.ResearchScientist:
				availableRooms.Add(ERoomType.ResearchLab);
				break;
			case ECharacterType.VirusScientist:
				availableRooms.Add(ERoomType.VirusLab);
				break;
			case ECharacterType.Civilian:
				availableRooms.Add(ERoomType.Corridor);
				availableRooms.Add(ERoomType.AssemblyLine);
				availableRooms.Add(ERoomType.Bathroom);
				availableRooms.Add(ERoomType.Cafeteria);
				availableRooms.Add(ERoomType.Corridor);
				availableRooms.Add(ERoomType.Elevator);
				availableRooms.Add(ERoomType.Reception);
				availableRooms.Add(ERoomType.ResearchLab);
				availableRooms.Add(ERoomType.VirusLab);
				availableRooms.Add(ERoomType.ZombieWarehouse);
				availableRooms.Add(ERoomType.VirusLab);
				availableRooms.Add(ERoomType.ResearchLab);
				availableRooms.Add(ERoomType.AssemblyLine);
				break;
			}
		
		return availableRooms;
	}
}

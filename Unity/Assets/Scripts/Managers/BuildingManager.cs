using UnityEngine;
using System.Collections;

public class BuildingManager
{
	#region ENUM
	
	public enum EExpandDirection
	{
		Top,
		Left,
		Right
	}
	
	#endregion
	
	#region CONSTANTS
	
	private const int BASE_EXPAND_COST = 5000;
	private const int NB_ROOMS_HORIZONTAL_EXPAND = 5;
	private const int NB_ROOMS_VERTICAL_EXPAND = 2;
	private const float EXPAND_COST_FACTOR = 1.5f;
	
	#endregion
	
	#region STATIC_MEMBERS

	private static BuildingManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS
	
	//
	private int mBuildingWidth;
	private int mBuildingHeight;
	
	private int mLeftBorder;
	private int mRightBorder;
	private int mTopBorder;
	
	private int[] mExpandingCosts;

	#endregion

	#region ACCESSORS
	
	public static BuildingManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new BuildingManager();
			}

			return sInstance;
		}
	}
	
	public int BuildingWidth
	{
		get {return mBuildingWidth;}
	}
	
	public int BuildingHeight
	{
		get {return mBuildingHeight;}
	}
	
	public int LeftBorder
	{
		get{return mLeftBorder;}
	}
	
	public int RightBorder
	{
		get {return mRightBorder;}
	}

	public int TopBorder
	{
		get{return mTopBorder;}
	}
	
	#endregion

	#region CONSTRUCTORS

	private BuildingManager ()
	{
		//InitializeExpandCost
		mExpandingCosts = new int[] {BASE_EXPAND_COST,BASE_EXPAND_COST,BASE_EXPAND_COST};
	}

	#endregion

	#region PUBLIC_METHODS
	
	public int GetExpandCost(EExpandDirection aDirection)
	{
		return mExpandingCosts[(int)aDirection];
	}
	
	public bool Expand(EExpandDirection aDirection)
	{
		if (GameManager.Instance.UserStats.Gold >= mExpandingCosts[(int)aDirection])
		{
			//
			GameManager.Instance.UserStats.RemoveMoney( ECurrency.Gold,mExpandingCosts[(int)aDirection]);
			
			//
			mExpandingCosts[(int)aDirection] = (int)(mExpandingCosts[(int)aDirection]*EXPAND_COST_FACTOR);
			
			//Expand
			switch(aDirection)
			{
				case EExpandDirection.Left:
					SetBuildingDimensions(mLeftBorder-NB_ROOMS_HORIZONTAL_EXPAND*Room.UNIT_CELL_WIDTH,
					mBuildingWidth+NB_ROOMS_HORIZONTAL_EXPAND,mBuildingHeight);
					break;
				case EExpandDirection.Top:
					SetBuildingDimensions(mLeftBorder,mBuildingWidth,mBuildingHeight+NB_ROOMS_VERTICAL_EXPAND);
					break;
				case EExpandDirection.Right:
					SetBuildingDimensions(mLeftBorder,mBuildingWidth+NB_ROOMS_HORIZONTAL_EXPAND,mBuildingHeight);
					break;
			}
			
			//Permit to create rooms in the new expended area.
			InGame.Instance.UpdateRoomCreationBounds();
			
			return true;
		}
		
		return false;
	}
	
	public void SetBuildingDimensions(int aMinX,int aWidth,int aHeight)
	{
		//
		mLeftBorder = aMinX;
		mRightBorder = mLeftBorder + aWidth*Room.UNIT_CELL_WIDTH;
		mBuildingWidth = aWidth;
		mBuildingHeight =  aHeight;
		mTopBorder = mBuildingHeight*Room.UNIT_CELL_HEIGHT;

		//
		BorderManager.Instance.SetBorder(new Rect(mLeftBorder, 0, mBuildingWidth*Room.UNIT_CELL_WIDTH, mTopBorder));
	}

	public bool IsInBounds(Vector2 aGridPos)
	{
		if(aGridPos.x * Room.UNIT_CELL_WIDTH < mLeftBorder || aGridPos.x * Room.UNIT_CELL_WIDTH >= mRightBorder || aGridPos.y < 0 || aGridPos.y * Room.UNIT_CELL_HEIGHT >= mTopBorder )
		{
			return false;
		}

		return true;
	}

	#endregion

	#region PRIVATE_METHODS

	#endregion
}

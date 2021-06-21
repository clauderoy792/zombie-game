using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {
	
	#region STATIC_MEMBERS
	
	static 	PathFinder 	sInstance;
	
	//
	public const 	int 		GRID_WIDTH = 50;
	public const 	int 		GRID_HEIGHT = 50;
	
	#endregion

	#region MEMBERS
	
	//Grid of nodes.
	private PathFindNode[][] mGrid;
	
	//Closed List.
	private List<PathFindNode> mClosedList;
	
	//Open List.
	private List<PathFindNode> mOpenList;
	
	//Current node
	private PathFindNode mCurrentNode;
	
	//End position node.
	private PathFindNode mTargetNode;
	
	//
	private int mNbRow;
	private int mNbColumn;
	
	#endregion
	
	#region ACCESSORS
	
	public static PathFinder Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new PathFinder(GRID_HEIGHT,GRID_WIDTH);
			}
			
			return sInstance;
		}
	}
	
	public PathFindNode[][] Grid
	{
		get {return mGrid;}
	}
	
	#endregion
	
	#region CONSTRUCTOR
	
	public PathFinder(int aNbRow,int aNbColumn)
	{
		//
		mNbRow = aNbColumn;
		mNbColumn = aNbColumn;
		
		mClosedList = new List<PathFindNode>(20);
		mOpenList = new List<PathFindNode>(20);
		
		//
		InitGrid();
	}
	
	#endregion

	#region ROOMS_MANAGEMENT
	
	public void SetRoomOccupied(Vector2 aPos,bool aValue, int aWidth = 1)
	{
		for(int i = 0;i < aWidth;i++)
		{
			mGrid[Mathf.RoundToInt(aPos.x+i)][(int)aPos.y].IsOccupied = aValue;
		}
	}
	
	public PathFindNode[] GetCellsForPosition(Rect aRect)
	{
		PathFindNode[] returnValue = null;
		
		//Check if position are valids.
		if (aRect.x >= 0 && aRect.x+aRect.width/Room.UNIT_CELL_WIDTH < mNbColumn && aRect.y >= 0 && aRect.y < mNbRow)
		{
			returnValue = new PathFindNode[(int)(aRect.width/Room.UNIT_CELL_WIDTH)];
			
			for(int i = 0;i <aRect.width/Room.UNIT_CELL_WIDTH;i++)
			{
				returnValue[i] = mGrid[(int)aRect.x +i][(int)aRect.y];
			}
		}
		else
		{
			Debug.LogError("Cannot get a cell in that position on the grid, the grid size is ("
				+mNbColumn+","+mNbRow+") and your rect is : "+aRect);
		}
		
		return returnValue;
	}
	
	public void SetRoomElevator(Room aRoom,bool aValue)
	{
		if (aRoom.GridPosition.x >= 0 && aRoom.GridPosition.x+aRoom.Width < mNbColumn && aRoom.GridPosition.y >= 0 && aRoom.GridPosition.y < mNbRow)
		{
			mGrid[(int)aRoom.GridPosition.x][(int)aRoom.GridPosition.y].IsElevator = aValue;
		}
		else
		{
			Debug.LogError("Cannot get a cell in that rect on the grid, the grid size is ("
				+mNbColumn+","+mNbRow+") and your position is : "+aRoom.GridPosition + ", length : "+aRoom.Width+", height : "+aRoom.Height);
		}
	}
	
	#endregion
	
	#region PATH_FINDING_MANAGEMENT
	
	/// <summary>
	/// Inits the grid.
	/// </summary>
	void InitGrid()
	{
		mGrid = new PathFindNode[mNbColumn][];
		
		//Initialize all nodes.
		//Loop through each row
		for(int i = 0;i < mNbColumn;i++)
		{
			mGrid[i] = new PathFindNode[mNbRow];
			
			//Loop through each cell of the row
			for (int j = 0; j<mNbRow;j++)
			{
				mGrid[i][j] = new PathFindNode(i,j);
			}
		}
		
		SetAdjacentNodes();
	}
	
	/// <summary>
	/// Finds the path with a start and a end position.
	/// </summary>
	/// <returns>
	/// The path.
	/// </returns>
	/// <param name='aStartPos'>
	/// A start position.
	/// </param>
	/// <param name='aEndPos'>
	/// A end position.
	/// </param>
	public List<Vector2> FindPath(Vector2 aStartPos, Vector2 aEndPos)
	{
		return FindPath(mGrid[Mathf.RoundToInt(aStartPos.x)][(int)aStartPos.y],mGrid[Mathf.RoundToInt(aEndPos.x)][(int)aEndPos.y]);
	}
	
	public List<Vector2> FindPath(Room aRoom1,Room aRoom2)
	{
		if (aRoom1 == null || aRoom2 == null)
		{
			Debug.Log("Either the start room or the target room is null, cannot find path !");
			
			return null;
		}
		
		return FindPath(new Vector2(aRoom1.GridPosition.x,aRoom1.GridPosition.y)
			,new Vector2(aRoom2.GridPosition.x,aRoom2.GridPosition.y));
	}
	
	/// <summary>
	/// Finds the path. with a start node and a target node.
	/// </summary>
	/// <returns>
	/// The path.
	/// </returns>
	/// <param name='aStart'>
	/// A start.
	/// </param>
	/// <param name='aTarget'>
	/// A target.
	/// </param>
	public List<Vector2> FindPath(PathFindNode aStart, PathFindNode aTarget)
	{
		List<Vector2> path = null;

		//Check if we want to move in the same room
		if(aStart.Position == aTarget.Position)
		{
			Vector2 pos = new Vector2(aStart.Position.x*Room.UNIT_CELL_WIDTH,aStart.Position.y*Room.UNIT_CELL_HEIGHT);
			path  = new List<Vector2>() {pos,pos};
		}
		else
		{
			//Clear lists
			mClosedList.Clear();
			mOpenList.Clear();
			
			//
			mTargetNode = aTarget;
			mCurrentNode = aStart;

			CalculateHValues();
	
			//Check while we arent at the closest node that faces the position (not diagonal).
			while (mCurrentNode != null && (mCurrentNode.HValue != 1 || mCurrentNode.Position.y != aTarget.Position.y))
			{
				CalculateAdjacentNodes();
				
				FindNextNode();
			}
	
			//There was no path found.
			if (mCurrentNode == null)
			{
				Debug.LogWarning("COULD NOT FIND PATH FROM : "+aStart.Position +" , TO : "+aTarget.Position);
				return null;
			}
			
			path = new List<Vector2>(20);
			
			//Add current node and target node.
			path.Add( new Vector2(mCurrentNode.Position.x*Room.UNIT_CELL_WIDTH,mCurrentNode.Position.y*Room.UNIT_CELL_HEIGHT));
			path.Add(new Vector2 (aTarget.Position.x*Room.UNIT_CELL_WIDTH,aTarget.Position.y*Room.UNIT_CELL_HEIGHT));
			
			//Set the return list.
			while(mCurrentNode.Parent != null)
			{
				path.Insert(0,new Vector2( mCurrentNode.Parent.Position.x*Room.UNIT_CELL_WIDTH,mCurrentNode.Parent.Position.y*Room.UNIT_CELL_HEIGHT));
				mCurrentNode = mCurrentNode.Parent;
			}
		}

		return path;	
	}
	
	#endregion
	
	#region PRIVATE_METHODS
	
	/// <summary>
	/// Finds the next node, and set it as the current node that we are checking.
	/// </summary>
	private void FindNextNode()
	{
		int smallestGValue = int.MaxValue;
		PathFindNode nextNode = null;
		
		for(int i = 0; i< mOpenList.Count;i++)
		{
			if (!mClosedList.Contains(nextNode) && mOpenList[i].GValue < smallestGValue)
			{
				smallestGValue = mOpenList[i].GValue;
				nextNode = mOpenList[i];
			}
		}
		
		//
		mCurrentNode = nextNode;
		
		//
		AddCurrentNodeToClosedList();
	}

	/// <summary>
	/// Calculates all the H values for all nodes.
	/// </summary>
	private void CalculateHValues()
	{
		int targetNodeX = (int)mTargetNode.Position.x;
		int targetNodeY = (int)mTargetNode.Position.y;
		
		
		//Loop through each row
		for(int i = 0;i < mNbColumn;i++)
		{
			//Loop through each cell of the row
			for (int j = 0; j<mNbRow;j++)
			{
				//Reset node.
				mGrid[i][j].Reset();

				//if we are past the target on x and y
				if ((j > mTargetNode.Position.y) && (i > mTargetNode.Position.x))
				{
					mGrid[i][j].HValue = i - targetNodeX+ j - targetNodeY;
				}
				//if we are past the target x
				else if (i > mTargetNode.Position.x)
				{
					mGrid[i][j].HValue = i - targetNodeX + targetNodeY - j;
				}
				//if we are past the target y
				else if (j > mTargetNode.Position.y)
				{
					mGrid[i][j].HValue = j - targetNodeX + targetNodeY - i;
				}
				else
				{
					mGrid[i][j].HValue = targetNodeX - i + targetNodeY - j;
				}
			}
		}
	}
	
	/// <summary>
	/// Calculates the adjacent nodes.
	/// </summary>
	private void CalculateAdjacentNodes()
	{
		foreach(PathFindNode node in mCurrentNode.AdjacentNodes)
		{
			//If the adjacent node is not contained in the closed list and the open list,
			//parent it to the current node and add it to the open list And calculate movement cost.
			//Check also if the current node is in the same y pos than the node or the node is an elevator.
			if (node != null && !mClosedList.Contains(node) 
				&& (mCurrentNode.Position.y == node.Position.y || (mCurrentNode.IsElevator && node.IsElevator)))
			{
				
				
				//Special check if node is in open list.
				if (mOpenList.Contains(node))
				{
					//If the node is contained in the open list and that the distance from the current node to this node,
					//is less than the node GValue, we dont do anything.
					if ((mCurrentNode.GValue + PathFindNode.LINEAR_MOVEMENT_COST) < node.GValue)
					{
						continue;
					}
					else
					{
						//We parent the node, to the current node.
						node.Parent = mCurrentNode;
						continue;
					}
				}
				
				//Calculate G value.
				node.GValue += PathFindNode.LINEAR_MOVEMENT_COST;
				
				//Parent the node.
				node.Parent = mCurrentNode;

				//Calculate F Value;
				node.FValue = node.GValue + node.HValue;
				
				//Add to open list.
				mOpenList.Add(node);
			}
		}
		//Add current node to closed list.
		AddCurrentNodeToClosedList();
	}
	
	/// <summary>
	/// Adds the current node to closed list and remove it from the open list.
	/// </summary>
	private void AddCurrentNodeToClosedList()
	{
		mOpenList.Remove(mCurrentNode);
		mClosedList.Add(mCurrentNode);
	}
	
	/// <summary>
	/// Sets the adjacent nodes for the grid.
	/// </summary>
	private void SetAdjacentNodes()
	{
		//Loop through each row
		for(int i = 0;i < mNbColumn;i++)
		{
			//Loop through each cell of the row
			for (int j = 0; j<mNbRow;j++)
			{
				//Set West Node
				if (i - 1 >= 0)
				{
					mGrid[i][j].AdjacentNodes[(int)EDirection.West] = mGrid[i-1][j];
				}
				
				//Set Eaest Node
				if (i +1 < mNbColumn)
				{
					mGrid[i][j].AdjacentNodes[(int)EDirection.East] = mGrid[i+1][j];
				}
				
				//Set South Node
				if (j +1 < mNbRow)
				{
					mGrid[i][j].AdjacentNodes[(int)EDirection.South] = mGrid[i][j+1];
				}
				
				//Set North Node
				if (j -1 >= 0)
				{
					mGrid[i][j].AdjacentNodes[(int)EDirection.North] = mGrid[i][j-1];
				}
			}
		}
	}
	
	#endregion
}

using UnityEngine;
using System.Collections;

public class PathFindNode {
	
	#region STATIC_VARIABLES
	
	//Linear movement cost.
	public static int LINEAR_MOVEMENT_COST = 10;
	
	//Diagonal movement cost.
	public static int DIAGONAL_MOVEMENT_COST = 14;
	
	#endregion
	
	#region MEMBERS
	
	//To determine if we can access this node.
	private bool mIsOccupied;
	private bool mAccessible;
	private bool mIsElevator;
	
	//position in the grid
	private Vector2 mPosition;
	
	//Parent node
	private PathFindNode mParent;
	
	//Adjacent nodes
	private PathFindNode[] mAdjacentNodes;
	
	//Node values
	private int mHeuristicValue;
	private int mMovementCost;
	private int mHeuristicValuePlusMovementCost;
	
	#endregion
	
	#region ACCESSORS
	
	public Vector2 Position
	{
		get {return mPosition;}
		set {mPosition = value;}
	}
	
	public bool IsOccupied
	{
		get {return mIsOccupied;}
		set {mIsOccupied = value;}
	}
	
	public bool IsElevator
	{
		get{return mIsElevator;}
		set {mIsElevator = value;}
	}
	
	public PathFindNode Parent
	{
		get {return mParent;}
		set {mParent = value;}
	}
	
	public PathFindNode[] AdjacentNodes
	{
		get {return mAdjacentNodes;}
	}
	
	public int HValue
	{
		get {return mHeuristicValue;}
		set {mHeuristicValue = value;}
	}
	
	public int GValue
	{
		get {return mMovementCost;}
		set {mMovementCost = value;}
	}
	
	public int FValue
	{
		get {return mHeuristicValuePlusMovementCost;}
		set {mHeuristicValuePlusMovementCost = value;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public PathFindNode(float x,float y)
	{
		mPosition = new Vector2(x,y);
		mAdjacentNodes = new PathFindNode[(int)EDirection.Count];
	}
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public void Reset()
	{
		mParent = null;
	}
	
	#endregion
}

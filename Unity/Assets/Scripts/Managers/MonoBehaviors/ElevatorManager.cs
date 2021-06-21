using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorManager : MonoBehaviour
{
	#region CONSTANTS

	#endregion

	#region STATIC_MEMBERS

	private static ElevatorManager sInstance;

	#endregion

	#region PRIVATE_MEMBERS
	
	List<ElevatorLine> mElevators;
	
	#endregion

	#region ACCESSORS

	public static ElevatorManager Instance
	{
		get {return sInstance;}
	}
	
	public List<ElevatorLine> Elevators
	{
		get{return mElevators;}
	}

	#endregion

	#region MONO_METHODS

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			
			mElevators = new List<ElevatorLine>();
		}
		else
		{
			Debug.Log("There is already an instance of the ElevatorManager in the scene.");
		}
	}

	#endregion

	#region PUBLIC_METHODS
	
	public void AddElevator(Elevator aElevator)
	{
		ElevatorLine addedLine = null;
		
		foreach(ElevatorLine line in mElevators)
		{
			//If it it possible to join the elevator to existing lines.
			if (line.CanAdd(aElevator))
			{
				if (addedLine != null)
				{
					//need to merge 2 lines and quit function.
					MergeElevatorsLine(line,addedLine,aElevator);
					return;
				}
				else
				{
					//Set the added line.
					addedLine = line;
				}
			}
		}
		
		
		//Could not add elevator to an existing line
		if (addedLine == null)
		{
			addedLine = new ElevatorLine(aElevator);
			mElevators.Add(addedLine);
		}
		else
		{
			addedLine.AddElevator(aElevator);
		}
	}
	
	public void AddCharacterInElevator(Character aCharacter)
	{
		ElevatorLine e = FindElevatorForCharacter(aCharacter);
		
		if (e != null)
		{
			e.InitializeCharacter(aCharacter);
		}
	}
	
	public ElevatorLine FindElevatorForCharacter(Character aCharacter)
	{
		ElevatorLine returnValue = null;
		
		foreach(ElevatorLine line in mElevators)
		{
			if (line.IsAppropriateForCharacter(aCharacter))
			{
				returnValue = line;
				break;
			}
		}
		
		if (returnValue == null)
		{
			Debug.LogError("Could not find elevator for character");
		}
		
		return returnValue;
	}

	public void RemoveElevator(ElevatorLine aLine)
	{
		mElevators.Remove(aLine);
		aLine.Dispose();
	}
	
	#endregion
	
	#region INITIALIZATION
	
	public void Initialize(List<ElevatorLine> aElevators)
	{
		mElevators = aElevators;
	}
	
	#endregion

	#region PRIVATE_METHODS
				
	void MergeElevatorsLine(ElevatorLine aLine1,ElevatorLine aLine2, Elevator aElevator)
	{
		//Line 1 is below line 2
		if (aLine1.Root.GridPosition.y < aLine2.Root.GridPosition.y)
		{
			//
			aLine1.AddElevator(aElevator);
			aLine1.AddLine(aLine2);
			
			//
			mElevators.Remove(aLine2);
		}
		else
		{
			//
			aLine2.AddElevator(aElevator);
			aLine2.AddLine(aLine1);
			
			//
			mElevators.Remove(aLine1);
		}
	}
	
	#endregion
}

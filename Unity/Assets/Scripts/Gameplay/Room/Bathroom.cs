using UnityEngine;
using System.Collections;

public class Bathroom : Room 
{
	public SpriteRenderer[] doors;
	public Vector2[] toiletPosition;
	private bool[] mDoorsState;
	public Sprite mOpenDoor;
	public Sprite mClosedDoor;
	
	#region MONO METHODS
	
	protected override void Start ()
	{
		base.Start ();

		mDoorsState = new bool[doors.Length];
		mDirtinessMultiplier = 3;
		
		for(int i=0; i<doors.Length; i++)
		{
			//TODO: HG
		//	doors[i].sprite = DOOR_OPEN;
			mDoorsState[i] = true;
		}
	}
	
	public bool HasAvailableToilet()
	{
		for(int i=0; i<mDoorsState.Length; i++)
		{
			if(mDoorsState[i])
			{
				return true;
			}
		}
				
		return false;
	}
	
	public int GetAvailableToiletID()
	{
		for(int i=0; i<mDoorsState.Length; i++)
		{
			if(mDoorsState[i])
			{
				return i;
			}
		}
		
		// THE USER PROBABLY DIDNT CHECK IF THERE IS A TOILET AVAIBLE BEFORE USING THIS
		return -1;
	}
	
	public void ReserveToilet(int aToiletID)
	{
		mDoorsState[aToiletID] = false;
	}
	
	public Vector2 GetToiletPosition(int aToiletID)
	{
		Vector2 roomPos = new Vector2(GridPosition.x*UNIT_CELL_WIDTH, GridPosition.y-Room.UNIT_CELL_HEIGHT);
		return (roomPos + toiletPosition[aToiletID]);
	}
	
	public void ReleaseToiletReservation(int aToiletID)
	{
		mDoorsState[aToiletID] = true;
	}
	
	public void ChangeToiletDoorState(int aToiletID, bool aDoorOpen)
	{
		doors[aToiletID].sprite = aDoorOpen ? mOpenDoor : mClosedDoor;
	}
	
	#endregion
	
	
	#region SERIALIZATION
	
	public override RoomSerializationInfo Serialize ()
	{
		return new BathroomSerializationInfo(this);
	}
	
	#endregion
}

using UnityEngine;
using System.Collections;

public class GoHome : Behavior 
{
	Human mOwner;
	Vector2 mLeftHomePosition = new Vector2(-1,-1);
	Vector2 mRightHomePosition = new Vector2(-1,-1);
	
	
	/// <summary>
	/// Initializes a new instance of the <see cref="GoHome"/> class.
	/// </summary>
	/// <param name='aCharacter'>
	/// A character.
	/// </param>
	public GoHome(Human aCharacter) : base (aCharacter)
	{
		mOwner = aCharacter;
	}
	
	
	/// <summary>
	/// Raises the initialize event.
	/// </summary>
	public override void OnInitialize ()
	{
		float xLeft = Mathf.Clamp(BuildingManager.Instance.LeftBorder - 2, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		float xRight = Mathf.Clamp(BuildingManager.Instance.RightBorder + 2, 0, PathFinder.GRID_WIDTH * Room.UNIT_CELL_WIDTH);
		mLeftHomePosition = new Vector2( xLeft, 0 );
		mRightHomePosition = new Vector2( xRight, 0 );
	}
	
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public override BH_Status Update ()
	{	
		if(mStatus == BH_Status.RUNNING)
		{
			if(new Vector2(mOwner.Transform.position.x, mOwner.GridPosition.y)  == mLeftHomePosition || 
				new Vector2(mOwner.Transform.position.x, mOwner.Transform.position.y) == mRightHomePosition)
			{
				mOwner.Hide();
				return BH_Status.SUCCESS;
			}
			else
			{
				return BH_Status.RUNNING;
			}
		}
		else
		{
			if( mOwner.IsTired || 
				(RoomManager.Instance.GetSameTypeRoom(ERoomType.Bathroom).Count == 0 && mOwner.HasToPee) ||
				(RoomManager.Instance.GetSameTypeRoom(ERoomType.Cafeteria).Count == 0 && mOwner.IsHungry))
			{
				//
				mOwner.GoHome();
				
				return BH_Status.RUNNING;
			}
		}
		
		//
		return BH_Status.FAILURE;
	}
}

	
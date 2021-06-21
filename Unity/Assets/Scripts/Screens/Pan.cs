using UnityEngine;
using System.Collections;

public class Pan : MonoBehaviour 
{
	public static bool enabled = true;
	private Vector2 mLastPosition = Vector2.zero;

	//
	void Start () 
	{
		// Init pan to center of game
		Vector3 newPos = UIScreen2D.Game.localPosition;
		newPos.Set(-(PathFinder.GRID_WIDTH/2*Room.UNIT_CELL_WIDTH) - Room.UNIT_CELL_WIDTH/2 + 0.01f, -UIScreen2D.BaseHeight/2 + 0.01f, newPos.z);
		UIScreen2D.Game.localPosition = newPos;
	}
	
	//
	void Update ()
	{
		if (enabled)
		{
			//
			if(InputManager.Instance.Inputs.IsTouch())
			{
				Vector2 currentPos = InputManager.Instance.Inputs.GetScreenPosition() / UIScreen2D.Factor;

				if(mLastPosition == Vector2.zero)
				{
					mLastPosition = currentPos;
				}
				else
				{
					Vector2 delta = currentPos - mLastPosition;

					//
					Vector3 gamePos = UIScreen2D.Game.localPosition;
					gamePos.x += delta.x;
					gamePos.y += delta.y;

					// Round position to snap to pixel pos and add little offset to fix pixel flickering.
					gamePos.x = Mathf.RoundToInt(gamePos.x) + 0.01f;
					gamePos.y = Mathf.RoundToInt(gamePos.y) + 0.01f;

					//
					UIScreen2D.Game.localPosition = gamePos;

					//
					mLastPosition = currentPos;
				}
			}

			//
			if(InputManager.Instance.Inputs.IsTouchUp())
		   	{
				mLastPosition = Vector2.zero;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class RoomPan : MonoBehaviour {

	#region MEMBERS

	//
	private Transform mTransform;
	private Vector2 mLastPos;
	private bool mIsDragging;

	#endregion

	#region MONO_METHODS

	void Awake()
	{
		mTransform = transform;
	}

	void OnTouchDown2D()
	{
		mIsDragging = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(InputManager.Instance.Inputs.IsTouchUp())
		{
			mIsDragging = false;
			mLastPos = Vector2.zero;
		}

		if (mIsDragging)
		{
			Vector2 currentPos = UIScreen2D.Game.InverseTransformPoint(InputManager.Instance.Inputs.GetWorldPosition());

			//
			if (currentPos != mLastPos)
			{
				currentPos = new Vector2(Mathf.Clamp(currentPos.x, BuildingManager.Instance.LeftBorder, BuildingManager.Instance.RightBorder - Room.UNIT_CELL_WIDTH), Mathf.Clamp(currentPos.y, 0, BuildingManager.Instance.TopBorder - Room.UNIT_CELL_HEIGHT));

				Vector3 newPos = currentPos.WorldToGrid().GridToWorld();
				newPos.z = mTransform.localPosition.z;
				mTransform.localPosition = newPos;

			}

			//
			mLastPos = currentPos;
		}
	}

	#endregion
}

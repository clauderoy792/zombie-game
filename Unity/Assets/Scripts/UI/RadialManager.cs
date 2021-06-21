using UnityEngine;
using System.Collections;

public class RadialManager : MonoBehaviour 
{
	Vector2 mTouchPos;
	private System.Action<int>[] mBuildRoomActions; 
	private Vector2 mRadialPos;
	const float MAX_CLICK_THRESHOLD = 5;


	void Awake()
	{
		mBuildRoomActions = new System.Action<int>[]
		{
			CreateRoomConfirmation,
			CreateRoomConfirmation,
			CreateRoomConfirmation,
			CreateRoomConfirmation,
			CreateRoomConfirmation,
			CreateRoomConfirmation,
			CreateRoomConfirmation
		};
	}

	void OnTouchDown2D()
	{
		mTouchPos = InputManager.Instance.Inputs.GetWorldPosition();
	}

	void OnClick2D()
	{
		Vector2 worldPos = InputManager.Instance.Inputs.GetWorldPosition();

		Vector2 guiLocalPos = UIScreen2D.Gui.InverseTransformPoint(worldPos);
		mRadialPos = UIScreen2D.Game.InverseTransformPoint(worldPos);

		//
		if(mRadialPos.y >= 0)
		{
			if((guiLocalPos - (Vector2)(UIScreen2D.Gui.InverseTransformPoint(mTouchPos))).sqrMagnitude <= MAX_CLICK_THRESHOLD)
			{
				if(BuildingManager.Instance.IsInBounds(mRadialPos.WorldToGrid()))
				{
					UIManager.Instance.ShowRadialMenu(guiLocalPos, UIRadialMenu.MenuType.BUILD, mBuildRoomActions);
				}
			}
		}
	}

	#region BUILD

	void CreateRoomConfirmation(int aRoomID)
	{
		ERoomType roomType = (ERoomType)aRoomID;
		RoomManager.Instance.AddBlueprintForRoom(roomType, mRadialPos.WorldToGrid());
	}

	#endregion
}

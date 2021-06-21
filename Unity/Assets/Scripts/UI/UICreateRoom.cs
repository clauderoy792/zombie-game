using UnityEngine;
using System.Collections;

public class UICreateRoom : MonoBehaviour 
{
	/*
	public ERoomType mRoomType = ERoomType.None;

	public void OnButtonClick()
	{
		if(mRoomType != ERoomType.None || mRoomType != ERoomType.COUNT)
		{
			Camera nCam = NGUITools.FindCameraForLayer(this.gameObject.layer);
			Vector2 screenPos = nCam.WorldToScreenPoint(transform.parent.position);
			Vector3 realPos = Camera.main.ScreenToWorldPoint(screenPos);

			//RoomManager.Instance.AddRoom(realPos.ConvertToCellIndex(), mRoomType, false);

			//
			float x1 = (int)realPos.x + Room.UNIT_CELL_WIDTH/2.0f;
			float y1 = Mathf.CeilToInt(realPos.y)- Room.UNIT_CELL_HEIGHT/2.0f;
			
			if(x1 % 2 == 0)
			{
				x1--;
			}
			
			Vector2 result = new Vector2(x1,y1);
			
			GameObject blueprint = GraphicsManager.Instance.GetBlueprint(mRoomType);
			blueprint.transform.position = result;
		}
	}
	*/
}

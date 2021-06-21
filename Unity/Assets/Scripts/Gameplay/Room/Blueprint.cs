using UnityEngine;
using System.Collections;

public class Blueprint : AdvancedMonoBehaviour 
{
	public static bool isVisible = false;
	public UILabel label;
	public UIButton2D doneBtn;
	private ERoomType mRoom = ERoomType.None;

	public void Initialize(ERoomType aRoomType)
	{
		label.Text = aRoomType.ToString();
		mRoom = aRoomType;
	}

	void OnEnable()
	{
		isVisible = true;
	}

	void OnDisable()
	{
		isVisible = false;
	}

	void Update()
	{
		doneBtn.IsEnabled = RoomManager.Instance.IsPositionValid( CachedTransform.localPosition.WorldToGrid() );
	}

	void OnDone()
	{
		Vector2 pos = CachedTransform.localPosition;
		Vector2 cellPos = pos.WorldToGrid();

		if(RoomManager.Instance.IsPositionValid(cellPos))
		{
			Construction c = RoomManager.Instance.AddRoom(cellPos, ERoomType.Construction, false) as Construction;
			
			if (c != null)
			{
				c.StartConstruction(mRoom);
			}
			else
			{
				Debug.LogError("Could not find construction;");
			}

			Destroy(this.gameObject);
		}
		else
		{
			UIManager.Instance.ShowPopup("Error", "You cannot place two rooms on the same location. Please choose another spot.", null);
		}
	}

	void OnCancel()
	{
		Destroy(this.gameObject);
	}
}

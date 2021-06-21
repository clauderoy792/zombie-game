using UnityEngine;
using System.Collections;

public static class RectExtension  {

	public static Rect ConvertToRoomPos(this Rect aRect)
	{
		Rect returnValue = new Rect(aRect.x/Room.UNIT_CELL_WIDTH,aRect.y/Room.UNIT_CELL_HEIGHT,aRect.width,aRect.height);

		return returnValue;
	}
}

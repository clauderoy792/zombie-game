using UnityEngine;
using System.Collections;

[System.Serializable]
public class RoomManagerSerializationInfo {

	public int mCurrentRoomPrice;
	public int mNbRoomCreated;
	public int mRoomId;

	public RoomManagerSerializationInfo(RoomManager aManager)
	{
		mCurrentRoomPrice 	= aManager.RoomPrice;
		mRoomId 			= aManager.RoomId;
		mNbRoomCreated 		= aManager.NbRoomCreated;
	}
}


public struct RoomInfo
{
	#region MEMBERS

	private ERoomType mRoomType;
	
	#endregion
	
	#region ACCESSORS
	
	public ERoomType RoomType
	{
		get {return mRoomType;}
	}
	
	#endregion
	
	#region CONSTRUCTORS
	
	public RoomInfo(ERoomType aType)
	{
		mRoomType = aType;
	}
	
	#endregion
}

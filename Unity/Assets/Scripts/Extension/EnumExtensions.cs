using UnityEngine;
using System.Collections;

public static class EnumExtensions 
{	
	#region EROOMTYPE
	
	public static string GetRoomStringType(this ERoomType aType)
	{
		string returnValue = "";
		
		if (aType == ERoomType.Corridor)
		{
			returnValue = "Room";
		}
		else
		{
			returnValue = aType.ToString();
		}
		
		return returnValue;
	}
	
	public static ECharacterType GetJob(this ERoomType aType)
	{
		ECharacterType returnValue = ECharacterType.None;
		
		switch(aType)
		{
			case ERoomType.ResearchLab:
				returnValue = ECharacterType.ResearchScientist;
				break;
			case ERoomType.VirusLab:
				returnValue = ECharacterType.VirusScientist;
				break;
			case ERoomType.AssemblyLine:
				returnValue = ECharacterType.Worker;
				break;
		}
		
		return returnValue;
	}
	
	#endregion
	
	#region ECHARACTERTYPE
	
	public static bool IsZombie(this ECharacterType aType)
	{
		return aType == ECharacterType.Zombie;
	}
	
	public static bool IsWorkingCharacter(this ECharacterType aType)
	{
		bool returnValue = false;
		
		switch(aType)
		{
			case ECharacterType.Janitor:
			case ECharacterType.ResearchScientist:
			case ECharacterType.SecurityGuard:
			case ECharacterType.VirusScientist:
			case ECharacterType.Worker:
				returnValue = true;
			break;
		}
		
		return returnValue;
	}
	
	#endregion
}

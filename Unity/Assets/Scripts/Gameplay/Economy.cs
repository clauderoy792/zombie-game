using UnityEngine;
using System.Collections;

public static class Economy {

	#region ROOM_PRICES
	
	public const float ROOM_SIZE_PRICE_FACTOR = 1.4f;
	
	public const int VIRUS_LAB_PRICE = 400;
	public const int RESEARCH_LAB_PRICE = 400;
	public const int CAFETERIA_PRICE = 400;
	public const int ASSEMBLY_LINE_PRICE = 400;
	public const int RECEPTION_PRICE = 400;
	public const int BATHROOM_PRICE = 400;
	public const int ZOMBIE_WAREHOUSE_PRICE = 400;
	public const int ELEVATOR_PRICE = 400;

	#endregion
	
	#region BASE_STATS
	
	public const int BASE_MONEY = 10000000;
	
	#endregion
	
	#region PUBLIC_METHODS
	
	public static int GetPriceForRoom(ERoomType aType)
	{
		//
		float priceFactor = 1;
		int returnValue = 0;
		
		//
		switch(aType)
		{
			case ERoomType.AssemblyLine:
				returnValue = (int)(priceFactor * Economy.ASSEMBLY_LINE_PRICE);
				break;
			case ERoomType.Bathroom:
				returnValue = (int)(priceFactor * Economy.BATHROOM_PRICE);
				break;
			case ERoomType.Cafeteria:
				returnValue = (int)(priceFactor * Economy.CAFETERIA_PRICE);
				break;
			case ERoomType.Elevator:
				returnValue = (int)(priceFactor * Economy.ELEVATOR_PRICE);
				break;
			case ERoomType.Reception:
				returnValue = (int)(priceFactor * Economy.RECEPTION_PRICE);
				break;
			case ERoomType.ResearchLab:
				returnValue = (int)(priceFactor * Economy.RESEARCH_LAB_PRICE);
				break;
			case ERoomType.VirusLab:
				returnValue = (int)(priceFactor * Economy.VIRUS_LAB_PRICE);
				break;
			case ERoomType.ZombieWarehouse:
				returnValue = (int)(priceFactor * Economy.ZOMBIE_WAREHOUSE_PRICE);
				break;
			default:
				Debug.Log("Cannot set the price on the room type : "+aType.ToString());
				break;
		}
		
		return returnValue;
	}
	
	#endregion
}

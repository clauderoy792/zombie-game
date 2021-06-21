using UnityEngine;
using System.Collections;

public class UIRadialButton2D : UIButton2D 
{
	public System.Action<int> ActionWithParam {get;set;}
	public int ID {get;set;}

	protected override void CallEvent (UIEvent uiEvent)
	{
		if(uiEvent == trigger)
		{
			if(Action != null)
			{
				Action();
			}

			if(ActionWithParam != null)
			{
				ActionWithParam(ID);
			}
			
			if(target != null)
			{
				if(string.IsNullOrEmpty(targetParameter))
				{
					target.SendMessage(targetMessage, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					target.SendMessage(targetMessage, targetParameter, SendMessageOptions.DontRequireReceiver);
				}
			}

			//
			UIManager.Instance.HideRadialMenu();
		}
	}
}

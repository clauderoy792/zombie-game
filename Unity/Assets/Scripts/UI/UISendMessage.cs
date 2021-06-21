using UnityEngine;
using System.Collections;

public class UISendMessage : MonoBehaviour
{
	public GameObject target;
	public string targetMessage;
	public string targetParameter;
	public UIEvent trigger = UIEvent.CLICK;
	private Vector2 mDownPos = new Vector2(-50,-50);
	const int STATIONAY_THRESHOLD = 5;

	//
	void OnTouch2D()
	{
		CallEvent(UIEvent.HOLD);
	}
	
	//
	void OnTouchDown2D()
	{
		mDownPos = InputManager.Instance.Inputs.GetScreenPosition();
		CallEvent(UIEvent.DOWN);
	}
	
	//
	void OnTouchUp2D()
	{
		CallEvent(UIEvent.UP);
	}
	
	//
	void OnClick2D()
	{
		Vector2 currentPos = InputManager.Instance.Inputs.GetScreenPosition();

		if((mDownPos.x != -50 && mDownPos.y != -50) && Vector2.Distance(currentPos, mDownPos) <= STATIONAY_THRESHOLD)
		{
			CallEvent(UIEvent.STATIONARY_CLICK);
		}

		CallEvent(UIEvent.CLICK);
	}
	
	protected virtual void CallEvent(UIEvent uiEvent)
	{
		if(uiEvent == trigger)
		{
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
		}
	}
}

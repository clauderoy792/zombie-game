using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class UIButton2D : AdvancedMonoBehaviour 
{
	public GameObject target;
	public string targetMessage;
	public string targetParameter;
	public UIEvent trigger = UIEvent.CLICK;
	public Vector3 clickScale = Vector3.one;
	public float duration = 0.2f;
	public SpriteRenderer icon;
	public Color disabledColor = Color.gray;
	protected float mBlendValue = 0.0f;
	protected bool mEnabled = true;

	//
	public override void Awake ()
	{
		base.Awake ();
	}

	//
	public System.Action Action{get;set;}

	//
	public bool IsEnabled
	{
		get{return mEnabled;}
		set
		{
			mEnabled = value;

			if(icon != null)
			{
				Color color = mEnabled ? Color.white : disabledColor;
				icon.color = color;
			}
		}
	}

	//
	void OnTouch2D()
	{
		mBlendValue = 1.0f;

		CallEvent(UIEvent.HOLD);
	}
	
	//
	void OnTouchDown2D()
	{
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
		CallEvent(UIEvent.CLICK);
	}

	protected virtual void CallEvent(UIEvent uiEvent)
	{
		if(mEnabled && uiEvent == trigger)
		{
			if(Action != null)
			{
				Action();
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
		}
	}

	//
	void LateUpdate()
	{
		if(mEnabled)
		{
			if(clickScale != Vector3.one)
			{
				if(mBlendValue > 0)
				{
					CachedTransform.localScale = Vector3.Lerp(Vector3.one, clickScale, mBlendValue);
					mBlendValue -= Time.deltaTime / duration;

					if(mBlendValue <= 0)
					{
						CachedTransform.localScale = Vector3.Lerp(Vector3.one, clickScale, mBlendValue);
					}
				}
			}
		}
		else
		{
			if(CachedTransform.localScale != Vector3.one)
			{
				CachedTransform.localScale = Vector3.one;
			}
		}
	}
}

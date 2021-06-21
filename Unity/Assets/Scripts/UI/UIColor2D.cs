using UnityEngine;
using System.Collections;

public class UIColor2D : MonoBehaviour 
{
	public SpriteRenderer target;
	public Color color;
	public UIEvent trigger;
	public float duration = 0.2f;
	private float mBlendValue = 0.0f;
	private Color normalColor;


	void Start()
	{
		if(target != null)
			normalColor = target.color;
	}

	void OnTouch2D()
	{
		ChangeColor(UIEvent.HOLD);
	}

	void OnTouchDown2D()
	{
		ChangeColor(UIEvent.DOWN);
	}

	void OnTouchUp2D()
	{
		ChangeColor(UIEvent.UP);
	}

	void OnClick2D()
	{
		ChangeColor(UIEvent.CLICK);
	}

	void ChangeColor(UIEvent uiEvent)
	{
		if(uiEvent == trigger)
			mBlendValue = 1.0f;
	}

	void LateUpdate()
	{
		if(mBlendValue > 0.0f)
		{
			target.color = Color.Lerp(normalColor, color, mBlendValue);
			mBlendValue -= Time.deltaTime / duration;
		}
	}
}

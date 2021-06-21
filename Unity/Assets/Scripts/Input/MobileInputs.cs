using UnityEngine;
using System.Collections;


public class MobileInputs : IInputs 
{
	Vector2[] mLastFingerPos = new Vector2[]{MIN_VECTOR, MIN_VECTOR};
	static Vector2 MIN_VECTOR = new Vector2(-1000,-1000);
	static float ZOOM_FACTOR;
	
	public MobileInputs()
	{
		ZOOM_FACTOR = Screen.width;
	}
	
	public Vector2 GetWorldPosition()
	{
		//
		Vector2 position = Vector2.zero;
		
		//
		if (Input.touchCount > 0)
		{
			position = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
		}
		
		//
		return position;
	}

	public Vector2 GetScreenPosition()
	{
		//
		Vector2 position = Vector2.zero;
		
		//
		if (Input.touchCount > 0)
		{
			position = Input.touches[0].position;
		}
		
		//
		return position;
	}
	
	public Vector2 GetViewportPosition()
	{
		if (Camera.main == null)
		{
			throw new MissingComponentException("Main Camera is null");
		}
		
		//
		Vector2 position = Vector2.zero;
		
		//
		if (Input.touchCount > 0)
		{
			position = Camera.main.ScreenToViewportPoint(Input.touches[0].position);
		}
		
		//
		return position;
	}
	
	public bool IsTouchDown()
	{
		//
		foreach(Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began)
			{
				return true;
			}
		}
		
		//
		return false;
	}

	public bool IsTouch()
	{
		//
		return Input.touches.Length > 0;
	}
	
	public bool IsTouchUp()
	{
		//
		foreach(Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				return true;
			}
		}
		
		//
		return false;
	}
	
	public bool IsTouchMoved()
	{
		//
		foreach(Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Moved)
			{
				return true;
			}
		}
		
		//
		return false;
	}
	
	public bool IsZooming()
	{	
		if(Input.touchCount < 2)
		{
			mLastFingerPos = new Vector2[]{MIN_VECTOR, MIN_VECTOR};
		}
		
		return(	Input.touchCount >= 2 && 
				(Input.touches[0].phase == TouchPhase.Moved || 
				Input.touches[1].phase == TouchPhase.Moved));
	}
	
	public float GetZoomValue()
	{
		float delta = 0;
		
		if(Input.touchCount >= 2)	
		{
			if(mLastFingerPos[0] == MIN_VECTOR || mLastFingerPos[1] == MIN_VECTOR)
			{
				mLastFingerPos[0] = Input.touches[0].position;
				mLastFingerPos[1] = Input.touches[1].position;
			}
			else
			{
				float lastDistance = Vector2.Distance(mLastFingerPos[0], mLastFingerPos[1]);
				float currentDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
				
				delta = currentDistance - lastDistance;
			}
		}
		
		return delta / ZOOM_FACTOR;
	}
}

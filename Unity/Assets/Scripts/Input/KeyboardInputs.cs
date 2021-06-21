using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class KeyboardInputs : IInputs
{
	private bool mIsPressed = false;
	private Vector3 mTouchPos = Vector3.zero;
	
	public Vector2 GetWorldPosition()
	{
		//
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		//
		return new Vector2(worldPos.x,worldPos.y);
	}

	public Vector2 GetScreenPosition()
	{
		//
		return Input.mousePosition;
	}
	
	public Vector2 GetViewportPosition()
	{
		if (Camera.main == null)
		{
			throw new  MissingComponentException("Main Camera is null");
		}
		
		Vector3 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		
		return new Vector2(viewportPos.x,viewportPos.y);
	}
	
	public bool IsTouchDown()
	{
		//
		bool pressed = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
		
		//
		if (pressed)
		{
			mIsPressed = true;
			mTouchPos = Input.mousePosition;	
		}
		
		return pressed;
	}

	public bool IsTouch()
	{
		//
		bool pressed = Input.GetMouseButton(0) || Input.GetMouseButton(1);
		
		//
		if (pressed)
		{
			//mIsPressed = true;
			mTouchPos = Input.mousePosition;	
		}
		
		return pressed;
	}
	
	public bool IsTouchUp()
	{
		//
		bool released = Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
		
		//
		if (released)
		{
			mIsPressed = false;
		}
		
		return released;
	}
	
	public bool IsTouchMoved()
	{
		//
		bool released = Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
		
		//
		if (released)
		{
			mIsPressed = false;
		}
		
		return mIsPressed && Input.mousePosition != mTouchPos;
	}
	
	public bool IsZooming()
	{	
		return (Input.GetAxis("Mouse ScrollWheel") != 0);
	}
	
	public float GetZoomValue()
	{
		return Input.GetAxis("Mouse ScrollWheel");
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour{
     
    public float dragSpeed = 0.0001f;
    public float minX = -1;
    public float maxX = 101;
    public float minY = -1f;
    public float maxY = 51;
	public bool canPan = true;

  	private Transform mCameraTransform;
    private Vector3 mOriginalTransformPos;
	private Vector3 mOriginalMousePos;
	private Vector3 mMousePos;
	private Vector3 mNextMovePos;
	private Vector3 mNextPosition;
	private bool mResetPan = false;


	#region MONO_METHODS
	
	void Awake()
	{
		mCameraTransform = transform;
		
		Vector3 position = mCameraTransform.position;
		
		//Initialize x position
		if (mCameraTransform.position.x < minX)
		{
			position.Set(minX,position.y,position.z);
		}
		else if (mCameraTransform.position.x > maxX)
		{
			position.Set(maxX,position.y,position.z);
		}
		
		//Initialize y position
		if (mCameraTransform.position.y < minY)
		{
			position.Set(position.x,minY,position.z);
		}
		else if (mCameraTransform.position.y > maxY)
		{
			position.Set(position.x,maxY,position.z);
		}
		
		//
		mCameraTransform.position = position;
	}
	
    void Update () 
	{
		//
		if (canPan /*&& !UIManager.Instance.IsRadialMenuOpen*/)
		{
			if(InputManager.Instance.Inputs.IsTouchUp())
			{
				mResetPan = false;
			}
			
			if(!mResetPan)
			{
				//
				if (InputManager.Instance.Inputs.IsTouchDown())
				{
					mOriginalTransformPos = mCameraTransform.position;
					mOriginalMousePos = InputManager.Instance.Inputs.GetViewportPosition();
				}
				else if (InputManager.Instance.Inputs.IsTouchMoved())
				{
					//
					mMousePos = InputManager.Instance.Inputs.GetViewportPosition();
					
					//
					mNextMovePos = mOriginalTransformPos + (mMousePos-mOriginalMousePos)*dragSpeed;
					
					//
					mNextPosition = mCameraTransform.position;
					
					//Check if we are in bounds.
					if (mNextMovePos.x >= minX && mNextMovePos.x <= maxX)
					{
						//
						mNextPosition.Set(mNextMovePos.x,mNextPosition.y,mNextPosition.z);
					}
					
					if (mNextMovePos.y >= minY && mNextMovePos.y <= maxY)
					{
						//
						mNextPosition.Set(mNextPosition.x,mNextMovePos.y,mNextPosition.z);
					}
					
					//
					mCameraTransform.position = mNextPosition;
				}
			}
		}
    }
	
	#endregion
}
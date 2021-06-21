using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraZoom : MonoBehaviour 
{
	public bool realPixelPerfect = false;
	public int pixelPerUnit = 64;
	public float targetedOthoSize = 1.25f;
	public int maxIteration = 5;
	private Camera mCamera;

	public void Awake()
	{
		//
		mCamera = GetComponent<Camera>();

		//
		InitializeZoom();
	}

	
	void InitializeZoom ()
	{
		int screenHeight = Screen.height;
		float cameraOrthoSize = 0.0f;

		//
		if(realPixelPerfect)
		{
			cameraOrthoSize = screenHeight / pixelPerUnit / 2.0f;
		}
		else
		{
			List<float> powerOfTwo = new List<float>();
			int sum = pixelPerUnit;
			for(int i = 0; i < maxIteration; i++)
			{
				sum = sum*2;
				powerOfTwo.Add(screenHeight/(float)sum);
			}
			
			//
			int closestIndex = 0;
			float closestValue = float.MaxValue;
			
			for(int i = 0; i < powerOfTwo.Count; i++)
			{
				float diff = Mathf.Abs(powerOfTwo[i] - targetedOthoSize);
				if(diff < closestValue )
				{
					closestValue = diff;
					closestIndex = i;
				}
			}

			cameraOrthoSize = powerOfTwo[closestIndex];
		}
		
		//
		mCamera.orthographicSize = cameraOrthoSize;
	}
}

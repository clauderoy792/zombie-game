
using UnityEngine;
using System.Collections;

public class OnTouchDown : MonoBehaviour {
	#if UNITY_IPHONE || UNITY_ANDROID

	RaycastHit mHit;
	Ray mRay;
	
	// Use this for initialization
	void Start () {
		mHit = new RaycastHit();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Code for OnMouseDown in the iPhone. Unquote to test.
		for (int i = 0; i < Input.touchCount; ++i) 
		{
			if (Input.GetTouch(i).phase.Equals(TouchPhase.Began)) 
			{
				// Construct a ray from the current touch coordinates
				mRay = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				if (Physics.Raycast(mRay, out mHit)) 
				{
					mHit.transform.gameObject.SendMessage("OnMouseDown");
			  	}
			}
	   }
	}
	
	#endif
}


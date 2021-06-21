using UnityEngine;
using System.Collections;

public class TimeScaleShortcut : MonoBehaviour {

	void Update()
	{
		//Shift+1
	    if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Alpha1))
	    {
	       	Time.timeScale = 1;
			Time.fixedDeltaTime = 1;
	    }
		//Shift+2
		else if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Alpha2))
	    {
	        Time.timeScale = 2;
			Time.fixedDeltaTime = 2;
	    }
		//Shift+3
		else if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Alpha3))
	    {
	        Time.timeScale = 5;
			Time.fixedDeltaTime = 5;
	    }
		//Shift+4
		else if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Alpha4))
	    {
	        Time.timeScale = 10;
			Time.fixedDeltaTime = 10;
	    }
		//Shift+5
		else if ((Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) && Input.GetKeyDown(KeyCode.Alpha5))
	    {
	        Time.timeScale = 50;
			Time.fixedDeltaTime = 50;
	    }
	}
}

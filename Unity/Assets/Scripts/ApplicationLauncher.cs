using UnityEngine;
using System.Collections;

public class ApplicationLauncher : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		//
		Random.seed = System.DateTime.Now.Millisecond*System.DateTime.Now.Second;
		
		//
		ScenesManager.Instance.LoadScreen(ScenesManager.EScene.UIScene);
	}
}

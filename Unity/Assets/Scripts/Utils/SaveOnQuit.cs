using UnityEngine;
using System.Collections;

public class SaveOnQuit : MonoBehaviour {

	void OnApplicationQuit()
	{
		//SaveGameManager.SaveGame();

		SaveGameManager.DeleteTempFiles();
	}
}

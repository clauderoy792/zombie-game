using UnityEngine;
using System.Collections;

public class TestContract : MonoBehaviour 
{
	int mSelection = 0;
	string[] mDifficulty = new string[]{ "Easy", "Normal", "Intermediate", "Hard", "VeryHard" };
	string mResult;

	// Update is called once per frame
	void OnGUI ()
	{
		mSelection = GUILayout.SelectionGrid(mSelection, mDifficulty, mDifficulty.Length);

		if(GUILayout.Button("Generate Contract"))
		{
			EContractDifficulty difficulty;

			switch(mSelection)
			{
			case 0: difficulty = EContractDifficulty.Easy; break;
			case 1: difficulty = EContractDifficulty.Normal; break;
			case 2: difficulty = EContractDifficulty.Intermediate; break;
			case 3: difficulty = EContractDifficulty.Hard; break;
			case 4: difficulty = EContractDifficulty.VeryHard; break;
			default : difficulty = EContractDifficulty.Normal; break;
			}

			var contract = ContractGenerator.Instance.GetRandomContract( difficulty );
			mResult = contract.ToString();
		}

		GUILayout.Space(30);

		GUILayout.Label(mResult);
	}
}

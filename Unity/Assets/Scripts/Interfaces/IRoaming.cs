using UnityEngine;
using System.Collections;

public interface IRoaming 
{
	//
	bool HasTarget();
	bool IsAtTarget();
	
	//
	void GoToTarget();
	void DoTargetAction();
}

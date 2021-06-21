using UnityEngine;
using System.Collections;

public static class HCMath {

	public static bool AlmostEqual(float n1,float n2, float interval = 0.06f)
	{
		return (Mathf.Abs(n1-n2) <= interval);
	}
}

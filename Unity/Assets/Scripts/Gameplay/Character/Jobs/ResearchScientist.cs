using UnityEngine;
using System.Collections;

public class ResearchScientist : Human
{
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new HumanBehavior(this);
	}
}

using UnityEngine;
using System.Collections;

public class VirusScientist : Human
{
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new HumanBehavior(this);
	}
}

using UnityEngine;
using System.Collections;

public class Receptionist : Human
{
	protected override void Awake ()
	{
		base.Awake ();
		Behavior = new HumanBehavior(this);
	}
}

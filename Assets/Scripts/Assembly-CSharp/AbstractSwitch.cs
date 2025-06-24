using System;
using UnityEngine;

public abstract class AbstractSwitch : AbstractCollidableObject
{
	public event Action OnActivate;

	protected void DispatchEvent()
	{
		Debug.Log("DispatchEvent ! " + base.name);
		if (this.OnActivate != null)
		{
			this.OnActivate();
		}
	}
}

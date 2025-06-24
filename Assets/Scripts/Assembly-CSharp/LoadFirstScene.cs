using System.Collections;
using UnityEngine;

public class LoadFirstScene : AbstractMonoBehaviour
{
	protected override void Start()
	{
		base.Start();
		Application.LoadLevel(0);
	}

	private IEnumerator load_cr()
	{
		yield return null;
	}
}

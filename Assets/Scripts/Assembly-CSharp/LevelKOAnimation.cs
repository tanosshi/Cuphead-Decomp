using System.Collections;
using UnityEngine;

public class LevelKOAnimation : AbstractLevelHUDComponent
{
	public enum State
	{
		Animating = 0,
		Complete = 1
	}

	private const float FRAME_DELAY = 5f;

	private State state;

	private static bool isMausoleum;

	public static LevelKOAnimation Create(bool isMaus)
	{
		isMausoleum = isMaus;
		return Object.Instantiate(Level.Current.LevelResources.levelKO);
	}

	protected override void Awake()
	{
		base.Awake();
		_parentToHudCanvas = true;
	}

	private void OnAnimComplete()
	{
		state = State.Complete;
	}

	public IEnumerator anim_cr()
	{
		GetComponent<Animator>().SetTrigger(isMausoleum ? "StartMaus" : "Start");
		while (state == State.Animating)
		{
			yield return null;
		}
	}
}

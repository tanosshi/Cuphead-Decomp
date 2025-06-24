using System;
using System.Collections;
using UnityEngine;

public class AbstractPausableComponent : AbstractMonoBehaviour
{
	[NonSerialized]
	public bool preEnabled;

	protected SoundEmitter emitAudioFromObject;

	protected virtual Transform emitTransform
	{
		get
		{
			return base.transform;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		PauseManager.AddChild(this);
		preEnabled = base.enabled;
		emitAudioFromObject = new SoundEmitter(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PauseManager.RemoveChild(this);
	}

	public virtual void OnPause()
	{
	}

	public virtual void OnUnpause()
	{
	}

	protected IEnumerator WaitForPause_CR()
	{
		while (PauseManager.state == PauseManager.State.Paused)
		{
			yield return null;
		}
	}

	public virtual void OnLevelEnd()
	{
		if (this != null)
		{
			StopAllCoroutines();
			base.enabled = false;
		}
	}

	public void EmitSound(string key)
	{
		AudioManager.FollowObject(key, emitTransform);
	}
}

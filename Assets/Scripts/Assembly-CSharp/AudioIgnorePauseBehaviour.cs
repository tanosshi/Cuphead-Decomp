using UnityEngine;

public class AudioIgnorePauseBehaviour : AbstractMonoBehaviour
{
	private AudioSource audio;

	protected override void Awake()
	{
		base.Awake();
		audio = GetComponent<AudioSource>();
		if (audio != null)
		{
			audio.ignoreListenerPause = true;
		}
	}
}

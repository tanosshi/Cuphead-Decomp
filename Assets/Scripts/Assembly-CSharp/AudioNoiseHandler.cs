using UnityEngine;

public class AudioNoiseHandler : AbstractMonoBehaviour
{
	private static AudioNoiseHandler noiseHandler;

	private const string PATH = "Audio/AudioNoiseHandler";

	public static AudioNoiseHandler Instance
	{
		get
		{
			if (noiseHandler == null)
			{
				AudioNoiseHandler audioNoiseHandler = Object.Instantiate(Resources.Load("Audio/AudioNoiseHandler")) as AudioNoiseHandler;
				audioNoiseHandler.name = "NoiseHandler";
			}
			return noiseHandler;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		noiseHandler = this;
		GetComponent<AudioSource>().ignoreListenerPause = true;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OpticalSound()
	{
		AudioManager.Play("optical_start");
	}
}

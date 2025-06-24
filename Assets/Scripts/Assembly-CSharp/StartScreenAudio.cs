using UnityEngine;

public class StartScreenAudio : AbstractMonoBehaviour
{
	private static StartScreenAudio startScreenAudio;

	public static StartScreenAudio Instance
	{
		get
		{
			return startScreenAudio;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		startScreenAudio = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}

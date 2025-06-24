using System.Collections;
using UnityEngine;

public class MapNPCMusic : MonoBehaviour
{
	private void Start()
	{
		Dialoguer.events.onMessageEvent += OnDialoguerMessageEvent;
	}

	private void OnDestroy()
	{
		Dialoguer.events.onMessageEvent -= OnDialoguerMessageEvent;
	}

	private void OnDialoguerMessageEvent(string message, string metadata)
	{
		if (message == "MinimalistMusic")
		{
			PlayerData.Data.pianoAudioEnabled = true;
			PlayerData.SaveCurrentFile();
			StartCoroutine(change_bgm_cr());
		}
		else if (message == "RegularMusic")
		{
			PlayerData.Data.pianoAudioEnabled = false;
			PlayerData.SaveCurrentFile();
			StartCoroutine(change_bgm_cr());
		}
	}

	private IEnumerator change_bgm_cr()
	{
		PlayerData.SaveCurrentFile();
		AudioManager.FadeBGMVolume(0f, 0.2f, true);
		yield return new WaitForSeconds(0.2f);
		AudioManager.StopBGM();
		AudioManager.StopBGMPlaylistManually();
		if (PlayerData.Data.pianoAudioEnabled)
		{
			AudioManager.PlayBGMPlaylistManually(false);
		}
		else
		{
			AudioManager.PlayBGM();
		}
		AudioManager.bgmOptionsVolume = 0f;
		AudioManager.FadeBGMVolume(1f, 0.3f, false);
	}
}

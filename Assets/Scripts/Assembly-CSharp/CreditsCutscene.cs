using System.Collections;
using UnityEngine;

public class CreditsCutscene : Cutscene
{
	[SerializeField]
	private AudioSource bgm;

	protected override void Start()
	{
		base.Start();
		CutsceneGUI.Current.pause.pauseAllowed = false;
		StartCoroutine(music_cr());
	}

	private IEnumerator music_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		if (CreditsScreen.goodEnding)
		{
			bgm.Play();
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "GoodEnding");
		}
		else
		{
			AudioManager.PlayBGMPlaylistManually(true);
			OnlineManager.Instance.Interface.UnlockAchievement(PlayerId.Any, "BadEnding");
		}
	}
}

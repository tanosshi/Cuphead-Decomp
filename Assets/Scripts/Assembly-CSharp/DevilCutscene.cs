using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DevilCutscene : Cutscene
{
	private CupheadInput.AnyPlayerInput input;

	[SerializeField]
	private Image arrow;

	[SerializeField]
	private DevilCutsceneOptionSelector optionSelector;

	private float arrowTransparency;

	private bool arrowVisible;

	protected override void Start()
	{
		base.Start();
		input = new CupheadInput.AnyPlayerInput();
		CutsceneGUI.Current.pause.pauseAllowed = false;
		StartCoroutine(main_cr());
	}

	private IEnumerator main_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		arrowVisible = true;
		while (!input.GetAnyButtonDown())
		{
			yield return null;
		}
		arrowVisible = false;
		base.animator.SetTrigger("Continue");
		yield return CupheadTime.WaitForSeconds(this, 1.25f);
		optionSelector.Show();
	}

	public void RefuseDevil()
	{
		ConfirmSFX();
		StartCoroutine(refuse_devil_cr());
	}

	public void JoinDevil()
	{
		ConfirmSFX();
		StartCoroutine(join_devil_cr());
	}

	private IEnumerator join_devil_cr()
	{
		AudioManager.FadeBGMVolume(0f, 0.5f, true);
		AudioManager.PlayBGMPlaylistManually(false);
		base.animator.SetTrigger("joinedDevil");
		yield return CupheadTime.WaitForSeconds(this, 1.25f);
		arrowVisible = true;
		while (!input.GetAnyButtonDown())
		{
			yield return null;
		}
		arrowVisible = false;
		base.animator.SetTrigger("fadeOut");
		yield return base.animator.WaitForAnimationToEnd(this, "Fade_Out", 1);
		base.animator.SetTrigger("Continue");
		base.animator.SetTrigger("fadeIn");
		DevilEvilSFX();
		StartCoroutine(blink_cr());
		yield return CupheadTime.WaitForSeconds(this, 10f);
		KillSFX();
		CreditsScreen.goodEnding = false;
		Cutscene.Load(Scenes.scene_title, Scenes.scene_cutscene_credits, SceneLoader.Transition.Iris, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
	}

	private IEnumerator blink_cr()
	{
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, Random.Range(3f, 5f));
			base.animator.SetTrigger("Blink");
		}
	}

	private IEnumerator refuse_devil_cr()
	{
		base.animator.SetTrigger("refusedDevil");
		DevilAngrySFX();
		yield return CupheadTime.WaitForSeconds(this, 1.25f);
		arrowVisible = true;
		while (!input.GetAnyButtonDown())
		{
			yield return null;
		}
		arrowVisible = false;
		KillSFX();
		SceneLoader.LoadLevel(Levels.Devil, SceneLoader.Transition.Iris);
	}

	protected override void Update()
	{
		base.Update();
		if (arrowVisible)
		{
			arrowTransparency = Mathf.Clamp01(arrowTransparency + Time.deltaTime / 0.25f);
		}
		else
		{
			arrowTransparency = 0f;
		}
		arrow.color = new Color(1f, 1f, 1f, arrowTransparency);
	}

	private void ConfirmSFX()
	{
		AudioManager.Play("ui_confirm");
	}

	private void DevilEvilSFX()
	{
		AudioManager.PlayLoop("sfx_hell_fire");
		AudioManager.Play("devil_laugh");
	}

	private void DevilAngrySFX()
	{
		AudioManager.PlayLoop("sfx_hell_fire");
	}

	private void KillSFX()
	{
		AudioManager.FadeSFXVolume("sfx_hell_fire", 0f, 4f);
	}
}

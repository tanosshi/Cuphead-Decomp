using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OutroCutscene : Cutscene
{
	private CupheadInput.AnyPlayerInput input;

	[SerializeField]
	private Image arrow;

	private float arrowTransparency;

	private bool arrowVisible;

	protected override void Start()
	{
		base.Start();
		CreditsScreen.goodEnding = true;
		input = new CupheadInput.AnyPlayerInput();
		CutsceneGUI.Current.pause.pauseAllowed = false;
		StartCoroutine(main_cr());
		StartCoroutine(skip_cr());
	}

	private IEnumerator main_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 0.33f);
		int numScreens = 6;
		for (int i = 0; i < numScreens; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, 1.75f);
			arrowVisible = true;
			while (!input.GetAnyButtonDown())
			{
				yield return null;
			}
			arrowVisible = false;
			base.animator.SetTrigger("Continue");
			if (i != 5)
			{
				NextPageSFX();
			}
			if (i == 0)
			{
				FireWhooshSFX();
			}
			if (i == 4)
			{
				Cheering();
			}
		}
		CreditsScreen.goodEnding = true;
		yield return CupheadTime.WaitForSeconds(this, 6.25f);
		AudioManager.FadeBGMVolume(0f, 3f, true);
		yield return CupheadTime.WaitForSeconds(this, 3f);
		Cutscene.Load(Scenes.scene_title, Scenes.scene_cutscene_credits, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
	}

	private IEnumerator skip_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			if (input.GetButtonDown(CupheadButton.Pause))
			{
				Cutscene.Load(Scenes.scene_title, Scenes.scene_cutscene_credits, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
			}
			yield return null;
		}
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
		arrow.color = new Color(1f, 1f, 1f, arrowTransparency * 0.35f);
	}

	private void NextPageSFX()
	{
		AudioManager.Play("ui_confirm");
		AudioManager.Play("ui_pageturn");
	}

	private void FireWhooshSFX()
	{
		AudioManager.Play("firewhoosh");
	}

	private void Cheering()
	{
		AudioManager.Play("cheering");
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KingDiceCutscene : Cutscene
{
	private CupheadInput.AnyPlayerInput input;

	[SerializeField]
	private Image arrow;

	private float arrowTransparency;

	private bool arrowVisible;

	protected override void Awake()
	{
		base.Awake();
		input = new CupheadInput.AnyPlayerInput();
		CutsceneGUI.Current.pause.pauseAllowed = false;
		if (PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world1BossLevels, Level.Mode.Normal) && PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world2BossLevels, Level.Mode.Normal) && PlayerData.Data.CheckLevelsHaveMinDifficulty(Level.world3BossLevels, Level.Mode.Normal))
		{
			StartCoroutine(have_all_contracts_cr());
		}
		else
		{
			StartCoroutine(missing_contracts_cr());
		}
	}

	private IEnumerator have_all_contracts_cr()
	{
		base.animator.Play("All_Contracts");
		int numScreens = 2;
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		for (int i = 0; i < numScreens; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, 1.25f);
			arrowVisible = true;
			while (!input.GetAnyButtonDown())
			{
				yield return null;
			}
			arrowVisible = false;
			base.animator.SetTrigger("Continue");
		}
		SceneLoader.LoadScene(Scenes.scene_level_dice_palace_main, SceneLoader.Transition.Fade, SceneLoader.Transition.Iris);
	}

	private IEnumerator missing_contracts_cr()
	{
		base.animator.Play("Missing_Contracts");
		int numScreens = 3;
		yield return CupheadTime.WaitForSeconds(this, 0.25f);
		for (int i = 0; i < numScreens; i++)
		{
			yield return CupheadTime.WaitForSeconds(this, 1.25f);
			arrowVisible = true;
			while (!input.GetAnyButtonDown())
			{
				yield return null;
			}
			arrowVisible = false;
			base.animator.SetTrigger("Continue");
		}
		SceneLoader.LoadLastMap();
		yield return null;
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
}

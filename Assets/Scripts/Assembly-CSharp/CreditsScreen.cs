using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : AbstractMonoBehaviour
{
	public static bool goodEnding;

	[SerializeField]
	private RectTransform content;

	private VerticalLayoutGroup verticalLayoutGroup;

	[SerializeField]
	private float introDuration;

	[SerializeField]
	private float scrollSpeed;

	[SerializeField]
	private float outroDuration;

	private CupheadInput.AnyPlayerInput input;

	private bool doneScrolling;

	private float timeMultiplier = 1f;

	protected override void Start()
	{
		base.Start();
		Init(false);
	}

	public void Init(bool checkIfDead)
	{
		input = new CupheadInput.AnyPlayerInput();
		verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
		StartCoroutine(credits_cr());
		StartCoroutine(skip_cr());
		StartCoroutine(fastForward_cr());
	}

	private IEnumerator credits_cr()
	{
		float wait = introDuration;
		while (wait > 0f)
		{
			wait -= (float)CupheadTime.Delta * timeMultiplier;
			yield return null;
		}
		float accumulator = 0f;
		while (content.anchoredPosition.y < verticalLayoutGroup.preferredHeight - base.rectTransform.sizeDelta.y)
		{
			accumulator += (float)CupheadTime.Delta * timeMultiplier;
			while (accumulator > 1f / 24f)
			{
				accumulator -= 1f / 24f;
				content.anchoredPosition = new Vector2(0f, content.anchoredPosition.y + scrollSpeed * (1f / 24f));
			}
			yield return null;
		}
		doneScrolling = true;
		wait = outroDuration;
		while (wait > 0f)
		{
			wait -= (float)CupheadTime.Delta * timeMultiplier;
			yield return null;
		}
		PlayerManager.ResetPlayers();
		SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Iris, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
	}

	private IEnumerator fastForward_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			if (input.GetAnyButtonHeld() && !input.GetButtonDown(CupheadButton.Pause) && !doneScrolling)
			{
				if (timeMultiplier == 1f)
				{
					timeMultiplier = 8f;
					AudioManager.ChangeBGMPitch(8f, 0.125f);
				}
			}
			else if (timeMultiplier > 1f)
			{
				timeMultiplier = 1f;
				AudioManager.ChangeBGMPitch(1f, 0.125f);
			}
			yield return null;
		}
	}

	private IEnumerator skip_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 1f);
		while (true)
		{
			if (input.GetButtonDown(CupheadButton.Pause))
			{
				PlayerManager.ResetPlayers();
				SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Iris, SceneLoader.Transition.Fade, SceneLoader.Icon.None);
			}
			yield return null;
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!(CupheadMapCamera.Current == null))
		{
			base.transform.position = CupheadMapCamera.Current.transform.position;
		}
	}

	protected bool GetButtonDown(CupheadButton button)
	{
		if (input.GetButtonDown(button))
		{
			return true;
		}
		return false;
	}
}

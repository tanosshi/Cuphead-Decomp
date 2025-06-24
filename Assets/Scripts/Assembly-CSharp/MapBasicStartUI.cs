using TMPro;
using UnityEngine;

public class MapBasicStartUI : AbstractMapSceneStartUI
{
	public Animator Animator;

	public TMP_Text Title;

	public static MapBasicStartUI Current { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		Current = this;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Current == this)
		{
			Current = null;
		}
	}

	protected override void Update()
	{
		base.Update();
		if (base.CurrentState == State.Active)
		{
			CheckInput();
		}
	}

	private void CheckInput()
	{
		if (base.Able)
		{
			if (GetButtonDown(CupheadButton.Cancel))
			{
				Out();
			}
			if (GetButtonDown(CupheadButton.Accept))
			{
				LoadLevel();
			}
		}
	}

	public new void In(MapPlayerController playerController)
	{
		base.In(playerController);
		if (Animator != null)
		{
			Animator.SetTrigger("ZoomIn");
			AudioManager.Play("world_map_level_menu_open");
		}
		InitUI(level);
	}

	public void InitUI(string level)
	{
		TranslationElement translationElement = Localization.Find(level.ToString());
		if (translationElement != null)
		{
			Title.text = translationElement.translation.text;
		}
	}
}

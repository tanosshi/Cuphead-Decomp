using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapConfirmStartUI : AbstractMapSceneStartUI
{
	public Animator Animator;

	public TMP_Text Title;

	[Header("Coins")]
	public Image EmptyCoin1;

	public Image Coin1;

	public Image EmptyCoin2;

	public Image Coin2;

	public Image EmptyCoin3;

	public Image Coin3;

	public Image EmptyCoin4;

	public Image Coin4;

	public Image EmptyCoin5;

	public Image Coin5;

	public static MapConfirmStartUI Current { get; protected set; }

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

	public void InitUI(string level)
	{
		TranslationElement translationElement = Localization.Find(level);
		if (translationElement != null)
		{
			Title.text = translationElement.translation.text;
		}
		else
		{
			Title.text = "Area";
		}
		EmptyCoin2.enabled = true;
		Coin2.enabled = false;
		EmptyCoin3.enabled = true;
		Coin3.enabled = false;
		EmptyCoin4.enabled = true;
		Coin4.enabled = false;
		EmptyCoin5.enabled = true;
		Coin5.enabled = false;
		List<PlayerData.PlayerCoinManager.LevelAndCoins> levelsAndCoins = PlayerData.Data.coinManager.LevelsAndCoins;
		for (int i = 0; i < levelsAndCoins.Count; i++)
		{
			if (levelsAndCoins[i].level.ToString() == level)
			{
				if (levelsAndCoins[i].Coin1Collected)
				{
					EmptyCoin1.enabled = false;
					Coin1.enabled = true;
				}
				else
				{
					EmptyCoin1.enabled = true;
					Coin1.enabled = false;
				}
				if (levelsAndCoins[i].Coin2Collected)
				{
					EmptyCoin2.enabled = false;
					Coin2.enabled = true;
				}
				else
				{
					EmptyCoin2.enabled = true;
					Coin2.enabled = false;
				}
				if (levelsAndCoins[i].Coin3Collected)
				{
					EmptyCoin3.enabled = false;
					Coin3.enabled = true;
				}
				else
				{
					EmptyCoin3.enabled = true;
					Coin3.enabled = false;
				}
				if (levelsAndCoins[i].Coin4Collected)
				{
					EmptyCoin4.enabled = false;
					Coin4.enabled = true;
				}
				else
				{
					EmptyCoin4.enabled = true;
					Coin4.enabled = false;
				}
				if (levelsAndCoins[i].Coin5Collected)
				{
					EmptyCoin5.enabled = false;
					Coin5.enabled = true;
				}
				else
				{
					EmptyCoin5.enabled = true;
					Coin5.enabled = false;
				}
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
}

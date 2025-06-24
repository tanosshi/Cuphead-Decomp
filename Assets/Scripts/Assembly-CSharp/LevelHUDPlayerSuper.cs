using UnityEngine;

public class LevelHUDPlayerSuper : AbstractLevelHUDComponent
{
	private const float SPACE = 18f;

	[SerializeField]
	private LevelHUDPlayerSuperCard cardTemplate;

	private LevelHUDPlayerSuperCard[] cards;

	private float lastSuper;

	private bool superToReady;

	public override void Init(LevelHUDPlayer hud)
	{
		base.Init(hud);
		cardTemplate.Init(base._player.id, base._player.stats.ExCost);
		cards = new LevelHUDPlayerSuperCard[5];
		cards[0] = cardTemplate;
		for (int i = 1; i < cards.Length; i++)
		{
			Vector3 localPosition = cardTemplate.transform.localPosition + new Vector3(18f * (float)i, 0f, 0f);
			LevelHUDPlayerSuperCard levelHUDPlayerSuperCard = Object.Instantiate(cardTemplate);
			levelHUDPlayerSuperCard.rectTransform.SetParent(cardTemplate.rectTransform.parent, false);
			levelHUDPlayerSuperCard.rectTransform.localPosition = localPosition;
			levelHUDPlayerSuperCard.Init(base._player.id, base._player.stats.ExCost);
			cards[i] = levelHUDPlayerSuperCard;
		}
		OnSuperChanged(base._player.stats.SuperMeter);
	}

	public void OnSuperChanged(float super)
	{
		for (int i = 0; i < cards.Length; i++)
		{
			float num = base._player.stats.SuperMeterMax / (float)cards.Length;
			float num2 = num * (float)i;
			cards[i].SetAmount(super - num2);
		}
		if (super >= base._player.stats.SuperMeterMax)
		{
			LevelHUDPlayerSuperCard[] array = cards;
			foreach (LevelHUDPlayerSuperCard levelHUDPlayerSuperCard in array)
			{
				if (!levelHUDPlayerSuperCard.animator.GetBool("Super"))
				{
					superToReady = true;
				}
				else
				{
					superToReady = false;
				}
			}
			if (superToReady)
			{
				if (base._player.id == PlayerId.PlayerOne)
				{
					if (!AudioManager.CheckIfPlaying("player_parry_power_increment_cuphead"))
					{
						AudioManager.Play("player_parry_power_increment_cuphead");
					}
				}
				else if (!AudioManager.CheckIfPlaying("player_parry_power_increment_mugman"))
				{
					AudioManager.Play("player_parry_power_increment_mugman");
				}
			}
			LevelHUDPlayerSuperCard[] array2 = cards;
			foreach (LevelHUDPlayerSuperCard levelHUDPlayerSuperCard2 in array2)
			{
				levelHUDPlayerSuperCard2.SetSuper(true);
				if (lastSuper != super)
				{
					switch (base._player.id)
					{
					case PlayerId.PlayerOne:
						levelHUDPlayerSuperCard2.animator.Play("Cuphead_Idle_B", 0, 0f);
						break;
					case PlayerId.PlayerTwo:
						levelHUDPlayerSuperCard2.animator.Play("Mugman_Idle_B", 0, 0f);
						break;
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < cards.Length; l++)
			{
				cards[l].SetSuper(false);
				float num3 = base._player.stats.SuperMeterMax / (float)cards.Length;
				if (super >= num3 + num3 * (float)l)
				{
					cards[l].SetEx(true);
					if (cards[l].animator.GetCurrentAnimatorStateInfo(0).IsName("Cuphead_Spin_A"))
					{
						if (!AudioManager.CheckIfPlaying("player_parry_power_increment_cuphead"))
						{
							AudioManager.Play("player_parry_power_increment_cuphead");
						}
					}
					else if (cards[l].animator.GetCurrentAnimatorStateInfo(0).IsName("Mugman_Spin_A") && !AudioManager.CheckIfPlaying("player_parry_power_increment_mugman"))
					{
						AudioManager.Play("player_parry_power_increment_mugman");
					}
				}
				else
				{
					cards[l].SetEx(false);
				}
			}
		}
		superToReady = false;
		lastSuper = super;
	}
}

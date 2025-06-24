using System;
using RektTransform;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectList : AbstractMonoBehaviour
{
	public enum Mode
	{
		Primary = 0,
		Secondary = 1,
		Super = 2,
		Charm = 3,
		Difficulty = 4
	}

	[SerializeField]
	public Mode mode;

	public Button button;

	public Button selectedButton;

	public RectTransform contentPanel;

	protected override void Awake()
	{
		base.Awake();
		SetupList();
	}

	private void SetupList()
	{
		Array array = null;
		switch (mode)
		{
		case Mode.Primary:
		case Mode.Secondary:
			array = Enum.GetValues(typeof(Weapon));
			break;
		case Mode.Super:
			array = Enum.GetValues(typeof(Super));
			break;
		case Mode.Charm:
			array = Enum.GetValues(typeof(Charm));
			break;
		case Mode.Difficulty:
			array = Enum.GetValues(typeof(Level.Mode));
			break;
		}
		int num = 0;
		foreach (object value in array)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(button.gameObject);
			Button b = gameObject.GetComponent<Button>();
			string text = value.ToString().Replace("level_", string.Empty).Replace("weapon_", string.Empty)
				.Replace("super_", string.Empty)
				.Replace("charm_", string.Empty);
			b.name = value.ToString();
			gameObject.GetComponentInChildren<Text>().text = text;
			b.onClick.AddListener(delegate
			{
				PlayerId[] array2 = new PlayerId[2]
				{
					PlayerId.PlayerOne,
					PlayerId.PlayerTwo
				};
				PlayerId[] array3 = array2;
				foreach (PlayerId player in array3)
				{
					PlayerData.PlayerLoadouts.PlayerLoadout playerLoadout = PlayerData.Data.Loadouts.GetPlayerLoadout(player);
					switch (mode)
					{
					case Mode.Primary:
						playerLoadout.primaryWeapon = (Weapon)value;
						break;
					case Mode.Secondary:
						playerLoadout.secondaryWeapon = (Weapon)value;
						break;
					case Mode.Super:
						playerLoadout.super = (Super)value;
						break;
					case Mode.Charm:
						playerLoadout.charm = (Charm)value;
						break;
					case Mode.Difficulty:
						Level.SetCurrentMode((Level.Mode)value);
						break;
					}
				}
				if (selectedButton != null)
				{
					ColorBlock colors = selectedButton.colors;
					colors.colorMultiplier = 2f;
					selectedButton.colors = colors;
				}
				selectedButton = b;
				ColorBlock colors2 = selectedButton.colors;
				colors2.colorMultiplier = 4f;
				selectedButton.colors = colors2;
			});
			b.transform.SetParent(button.transform.parent);
			b.transform.ResetLocalTransforms();
			num++;
		}
		button.gameObject.SetActive(false);
		contentPanel.SetHeight(30f * (float)num);
	}
}

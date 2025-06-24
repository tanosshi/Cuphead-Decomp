using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapEquipUICardBackSelect : AbstractMapEquipUICardSide
{
	private static readonly Weapon[] WEAPONS = new Weapon[6]
	{
		Weapon.level_weapon_peashot,
		Weapon.level_weapon_spreadshot,
		Weapon.level_weapon_homing,
		Weapon.level_weapon_bouncer,
		Weapon.level_weapon_charge,
		Weapon.level_weapon_boomerang
	};

	private static readonly Super[] SUPERS = new Super[3]
	{
		Super.level_super_beam,
		Super.level_super_invincible,
		Super.level_super_ghost
	};

	private static readonly Charm[] CHARMS = new Charm[6]
	{
		Charm.charm_health_up_1,
		Charm.charm_super_builder,
		Charm.charm_smoke_dash,
		Charm.charm_parry_plus,
		Charm.charm_health_up_2,
		Charm.charm_parry_attack
	};

	[Header("Text")]
	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Text titleText;

	[SerializeField]
	private Text exText;

	[SerializeField]
	private Text descriptionText;

	[Header("Cursors")]
	[SerializeField]
	private MapEquipUICursor cursor;

	[SerializeField]
	private MapEquipUICardBackSelectSelectionCursor selectionCursor;

	[Header("Backs")]
	[SerializeField]
	private Image iconsBack;

	[SerializeField]
	private Image superIconsBack;

	[Header("Icons")]
	[SerializeField]
	private MapEquipUICardBackSelectIcon[] normalIcons;

	[Header("Super Icons")]
	[SerializeField]
	private MapEquipUICardBackSelectIcon[] superIcons;

	private int index;

	private int lastIndex;

	private MapEquipUICard.Slot slot;

	private MapEquipUICard.Slot lastSlot;

	private MapEquipUICardBackSelectIcon[] selectedIcons;

	private bool noneUnlocked;

	private bool itemSelected;

	public void ChangeSelection(Trilean2 direction)
	{
		index = selectedIcons[index].GetIndexOfNeighbor(direction);
		SetCursorPosition(index);
		UpdateText();
	}

	public void ChangeSlot(int direction)
	{
		cursor.Show();
		int num = (int)slot;
		slot = (MapEquipUICard.Slot)Mathf.Repeat(num + direction, EnumUtils.GetCount<MapEquipUICard.Slot>());
		Setup(slot);
	}

	private void UpdateText()
	{
		switch (slot)
		{
		case MapEquipUICard.Slot.SHOT_A:
		case MapEquipUICard.Slot.SHOT_B:
			titleText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index])) ? "LOCKED" : WeaponProperties.GetDisplayName(WEAPONS[index]).ToUpper());
			exText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index])) ? "? ? ? ? ? ? ? ? ?" : WeaponProperties.GetSubtext(WEAPONS[index]));
			descriptionText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index])) ? "? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ?" : WeaponProperties.GetDescription(WEAPONS[index]));
			break;
		case MapEquipUICard.Slot.SUPER:
			titleText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, SUPERS[index])) ? "LOCKED" : WeaponProperties.GetDisplayName(SUPERS[index]).ToUpper());
			exText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, SUPERS[index])) ? "? ? ? ? ? ? ? ? ?" : WeaponProperties.GetSubtext(SUPERS[index]));
			descriptionText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, SUPERS[index])) ? "? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ?" : WeaponProperties.GetDescription(SUPERS[index]));
			break;
		case MapEquipUICard.Slot.CHARM:
			titleText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, CHARMS[index])) ? "LOCKED" : WeaponProperties.GetDisplayName(CHARMS[index]).ToUpper());
			exText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, CHARMS[index])) ? "? ? ? ? ? ? ? ? ?" : WeaponProperties.GetSubtext(CHARMS[index]));
			descriptionText.text = ((!PlayerData.Data.IsUnlocked(base.playerID, CHARMS[index])) ? "? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ?" : WeaponProperties.GetDescription(CHARMS[index]));
			break;
		default:
			Debug.LogWarning(string.Concat("Slot '", slot, "' not configured"));
			break;
		}
	}

	private void SetCursorPosition(int index)
	{
		if (lastIndex != index)
		{
			AudioManager.Play("menu_equipment_move");
			lastIndex = index;
		}
		cursor.SetPosition(selectedIcons[index].transform.position);
		if (!noneUnlocked && itemSelected)
		{
			selectionCursor.Show();
		}
		else
		{
			selectionCursor.Hide();
		}
	}

	public void Setup(MapEquipUICard.Slot slot)
	{
		this.slot = slot;
		headerText.text = slot.ToString().Replace("_", "-");
		bool flag = slot == MapEquipUICard.Slot.SUPER;
		selectedIcons = ((!flag) ? normalIcons : superIcons);
		MapEquipUICardBackSelectIcon[] array = superIcons;
		foreach (MapEquipUICardBackSelectIcon mapEquipUICardBackSelectIcon in array)
		{
			mapEquipUICardBackSelectIcon.gameObject.SetActive(flag);
		}
		MapEquipUICardBackSelectIcon[] array2 = normalIcons;
		foreach (MapEquipUICardBackSelectIcon mapEquipUICardBackSelectIcon2 in array2)
		{
			mapEquipUICardBackSelectIcon2.gameObject.SetActive(!flag);
		}
		superIconsBack.enabled = flag;
		iconsBack.enabled = !flag;
		PlayerData.PlayerLoadouts.PlayerLoadout playerLoadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID);
		selectionCursor.Hide();
		noneUnlocked = true;
		index = -1;
		bool isGrey = false;
		itemSelected = false;
		for (int k = 0; k < selectedIcons.Length; k++)
		{
			selectedIcons[k].Index = k;
			string text = "_000" + (k + 1);
			string iconPath = Weapon.None.ToString();
			switch (slot)
			{
			case MapEquipUICard.Slot.SHOT_A:
				if (PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[k]))
				{
					isGrey = ((WEAPONS[k] == playerLoadout.secondaryWeapon) ? true : false);
					iconPath = WeaponProperties.GetIconPath(WEAPONS[k]);
					noneUnlocked = false;
				}
				else
				{
					iconPath = WeaponProperties.GetIconPath(Weapon.None) + text;
				}
				if (WEAPONS[k] == playerLoadout.primaryWeapon && playerLoadout.primaryWeapon != Weapon.None)
				{
					index = k;
					itemSelected = true;
				}
				break;
			case MapEquipUICard.Slot.SHOT_B:
				if (PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[k]))
				{
					isGrey = ((WEAPONS[k] == playerLoadout.primaryWeapon) ? true : false);
					iconPath = WeaponProperties.GetIconPath(WEAPONS[k]);
					noneUnlocked = false;
				}
				else
				{
					iconPath = WeaponProperties.GetIconPath(Weapon.None) + text;
				}
				if (WEAPONS[k] == playerLoadout.secondaryWeapon && playerLoadout.secondaryWeapon != Weapon.None)
				{
					index = k;
					itemSelected = true;
				}
				break;
			case MapEquipUICard.Slot.SUPER:
				if (PlayerData.Data.IsUnlocked(base.playerID, SUPERS[k]))
				{
					iconPath = WeaponProperties.GetIconPath(SUPERS[k]);
					noneUnlocked = false;
				}
				else
				{
					iconPath = WeaponProperties.GetIconPath(Super.None) + text;
				}
				if (SUPERS[k] == playerLoadout.super && playerLoadout.super != Super.None)
				{
					index = k;
					itemSelected = true;
				}
				break;
			case MapEquipUICard.Slot.CHARM:
				if (PlayerData.Data.IsUnlocked(base.playerID, CHARMS[k]))
				{
					iconPath = WeaponProperties.GetIconPath(CHARMS[k]);
					noneUnlocked = false;
				}
				else
				{
					iconPath = WeaponProperties.GetIconPath(Charm.None) + text;
				}
				if (CHARMS[k] == playerLoadout.charm && playerLoadout.charm != Charm.None)
				{
					index = k;
					itemSelected = true;
				}
				break;
			default:
				Debug.LogWarning(string.Concat("Slot '", slot, "' not yet configured"));
				break;
			}
			selectedIcons[k].SetIcons(iconPath, isGrey);
			if (index == -1)
			{
				index = 0;
			}
			cursor.SetPosition(selectedIcons[index].transform.position);
			UpdateText();
		}
		if (!noneUnlocked && itemSelected)
		{
			if (slot != lastSlot)
			{
				selectionCursor.Show();
				selectionCursor.SetPosition(selectedIcons[index].transform.position);
			}
			else
			{
				StartCoroutine(set_selection_cursor());
			}
			cursor.SelectIcon(true);
		}
		lastSlot = slot;
	}

	private IEnumerator set_selection_cursor()
	{
		while (!selectionCursor.animator.GetCurrentAnimatorStateInfo(0).IsName("Off"))
		{
			yield return null;
		}
		selectionCursor.Show();
		selectionCursor.SetPosition(selectedIcons[index].transform.position);
		yield return null;
	}

	public void Accept()
	{
		AudioManager.Play("menu_equipment_equip");
		PlayerData.PlayerLoadouts.PlayerLoadout playerLoadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID);
		switch (slot)
		{
		case MapEquipUICard.Slot.SHOT_A:
			if (PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index]))
			{
				Selection();
			}
			break;
		case MapEquipUICard.Slot.SHOT_B:
			if (PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index]) && (playerLoadout.primaryWeapon != WEAPONS[index] || playerLoadout.secondaryWeapon != Weapon.None))
			{
				Selection();
			}
			break;
		case MapEquipUICard.Slot.SUPER:
			if (PlayerData.Data.IsUnlocked(base.playerID, SUPERS[index]))
			{
				Selection();
			}
			break;
		case MapEquipUICard.Slot.CHARM:
			if (PlayerData.Data.IsUnlocked(base.playerID, CHARMS[index]))
			{
				Selection();
			}
			break;
		}
		switch (slot)
		{
		case MapEquipUICard.Slot.SHOT_A:
			if (WEAPONS[index] == Weapon.None || !PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index]))
			{
				AudioManager.Play("menu_locked");
				return;
			}
			if (PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).secondaryWeapon == WEAPONS[index])
			{
				PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).secondaryWeapon = PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).primaryWeapon;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).primaryWeapon = WEAPONS[index];
			break;
		case MapEquipUICard.Slot.SHOT_B:
			if (WEAPONS[index] == Weapon.None || !PlayerData.Data.IsUnlocked(base.playerID, WEAPONS[index]) || (playerLoadout.primaryWeapon == WEAPONS[index] && playerLoadout.secondaryWeapon == Weapon.None))
			{
				AudioManager.Play("menu_locked");
				return;
			}
			if (PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).primaryWeapon == WEAPONS[index])
			{
				PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).primaryWeapon = PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).secondaryWeapon;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).secondaryWeapon = WEAPONS[index];
			if (!PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).HasEquippedSecondaryRegularWeapon)
			{
				PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).MustNotifySwitchRegularWeapon = true;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).HasEquippedSecondaryRegularWeapon = true;
			break;
		case MapEquipUICard.Slot.SUPER:
			if (SUPERS[index] == Super.None || !PlayerData.Data.IsUnlocked(base.playerID, SUPERS[index]))
			{
				AudioManager.Play("menu_locked");
				return;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).super = SUPERS[index];
			break;
		case MapEquipUICard.Slot.CHARM:
			if (CHARMS[index] == Charm.None || !PlayerData.Data.IsUnlocked(base.playerID, CHARMS[index]))
			{
				AudioManager.Play("menu_locked");
				return;
			}
			PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID).charm = CHARMS[index];
			break;
		default:
			Debug.LogWarning(string.Concat("Slot '", slot, "' not yet configured"));
			break;
		}
		Setup(slot);
	}

	private void Selection()
	{
		selectedIcons[index].SelectIcon();
		if (cursor.transform.position != selectionCursor.transform.position)
		{
			selectionCursor.Select();
			cursor.SelectIcon(false);
		}
		else
		{
			cursor.SelectIcon(true);
		}
	}
}

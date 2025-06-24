using UnityEngine;
using UnityEngine.UI;

public class MapEquipUICardFront : AbstractMapEquipUICardSide
{
	public MapEquipUICardFrontIcon weaponA;

	public MapEquipUICardFrontIcon weaponB;

	public MapEquipUICardFrontIcon super;

	public MapEquipUICardFrontIcon item;

	public MapEquipUICardFrontIcon checklist;

	public bool checkListSelected;

	[Space(10f)]
	public MapEquipUICursor cursor;

	private int index;

	private MapEquipUICardFrontIcon[] icons;

	public Text title;

	private PlayerData.PlayerLoadouts.PlayerLoadout loadout;

	public MapEquipUICard.Slot Slot
	{
		get
		{
			return (MapEquipUICard.Slot)index;
		}
	}

	protected override void Update()
	{
		base.Update();
		SetCursorPosition(index);
	}

	public override void Init(PlayerId playerID)
	{
		base.Init(playerID);
		icons = new MapEquipUICardFrontIcon[5] { weaponA, weaponB, super, item, checklist };
		checklist.SetIcons("Icons/equip_icon_list", false);
		checkListSelected = false;
		Refresh();
		ChangeSelection(0);
	}

	public void Refresh()
	{
		loadout = PlayerData.Data.Loadouts.GetPlayerLoadout(base.playerID);
		weaponA.SetIcons(WeaponProperties.GetIconPath(loadout.primaryWeapon), false);
		weaponB.SetIcons(WeaponProperties.GetIconPath(loadout.secondaryWeapon), false);
		super.SetIcons(WeaponProperties.GetIconPath(loadout.super), false);
		item.SetIcons(WeaponProperties.GetIconPath(loadout.charm), false);
	}

	public void ChangeSelection(int direction)
	{
		if ((index != icons.Length - 1 && direction != -1) || (index != 0 && direction != 1))
		{
			AudioManager.Play("menu_equipment_move");
		}
		index = Mathf.Clamp(index + direction, 0, icons.Length - 1);
		SetCursorPosition(index);
		checkListSelected = ((index == icons.Length - 1) ? true : false);
		string empty = string.Empty;
		if (icons[index] == weaponA)
		{
			empty = WeaponProperties.GetDisplayName(loadout.primaryWeapon);
			if (empty.ToUpper() == "ERROR")
			{
				empty = Localization.Translate("level_weapon_none_name").text;
			}
			title.text = empty;
		}
		else if (icons[index] == weaponB)
		{
			empty = WeaponProperties.GetDisplayName(loadout.secondaryWeapon);
			if (empty.ToUpper() == "ERROR")
			{
				empty = Localization.Translate("level_weapon_none_name").text;
			}
			title.text = empty;
		}
		else if (icons[index] == super)
		{
			empty = WeaponProperties.GetDisplayName(loadout.super);
			if (empty.ToUpper() == "ERROR")
			{
				empty = Localization.Translate("level_super_none_name").text;
			}
			title.text = empty;
		}
		else if (icons[index] == item)
		{
			empty = WeaponProperties.GetDisplayName(loadout.charm);
			if (empty.ToUpper() == "ERROR")
			{
				empty = Localization.Translate("charm_none_name").text;
			}
			title.text = empty;
		}
		else
		{
			title.text = Localization.Translate("list_name").text;
		}
	}

	private void SetCursorPosition(int index)
	{
		cursor.SetPosition(icons[index].transform.position);
	}
}

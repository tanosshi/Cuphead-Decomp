using System.Collections.Generic;
using UnityEngine;

public class MapEquipUIChecklist : AbstractMapEquipUICardSide
{
	private readonly string[] worldPaths = new string[4] { "equip_checklist_world_1", "equip_checklist_world_2", "equip_checklist_world_3", "equip_checklist_finale" };

	private readonly Levels[] world1Levels = new Levels[7]
	{
		Levels.Veggies,
		Levels.Slime,
		Levels.FlyingBlimp,
		Levels.Flower,
		Levels.Frogs,
		Levels.Platforming_Level_1_1,
		Levels.Platforming_Level_1_2
	};

	private readonly Levels[] world2Levels = new Levels[7]
	{
		Levels.Baroness,
		Levels.Clown,
		Levels.FlyingGenie,
		Levels.Dragon,
		Levels.FlyingBird,
		Levels.Platforming_Level_2_1,
		Levels.Platforming_Level_2_2
	};

	private readonly Levels[] world3Levels = new Levels[9]
	{
		Levels.Bee,
		Levels.Pirate,
		Levels.SallyStagePlay,
		Levels.Mouse,
		Levels.Robot,
		Levels.FlyingMermaid,
		Levels.Train,
		Levels.Platforming_Level_3_1,
		Levels.Platforming_Level_3_2
	};

	private readonly Levels[] finaleLevels = new Levels[11]
	{
		Levels.DicePalaceBooze,
		Levels.DicePalaceChips,
		Levels.DicePalaceCigar,
		Levels.DicePalaceDomino,
		Levels.DicePalaceEightBall,
		Levels.DicePalaceFlyingHorse,
		Levels.DicePalaceFlyingMemory,
		Levels.DicePalaceRabbit,
		Levels.DicePalaceRoulette,
		Levels.DicePalaceMain,
		Levels.Devil
	};

	[Header("Cursors")]
	[SerializeField]
	private MapEquipUICursor cursor;

	[Header("Icons")]
	[SerializeField]
	private MapEquipUICardChecklistIcon[] worldSelectionIcons;

	[Header("FinaleImages")]
	[SerializeField]
	private GameObject[] finaleImages;

	[Header("Bosses + Platforming items")]
	[SerializeField]
	private List<MapEquipUIChecklistItem> checklistItems;

	[SerializeField]
	private List<MapEquipUIChecklistItem> finaleItems;

	private int index;

	private int lastIndex;

	private bool selectedFinale;

	private Color darkText;

	private Color lightText;

	private Color disabledText;

	private int selectableLength;

	public override void Init(PlayerId playerID)
	{
		base.Init(playerID);
		darkText = new Color(0.2f, 0.188f, 0.188f);
		lightText = new Color(0.827f, 0.765f, 0.702f);
		disabledText = new Color(0.537f, 0.498f, 0.463f);
		selectableLength = worldSelectionIcons.Length;
		for (int i = 0; i < worldSelectionIcons.Length; i++)
		{
			worldSelectionIcons[i].SetIcons("Icons/" + worldPaths[i] + "_dark");
			worldSelectionIcons[i].SetTextColor(darkText);
		}
		worldSelectionIcons[index].SetIcons("Icons/" + worldPaths[index] + "_light");
		worldSelectionIcons[index].SetTextColor(lightText);
		if (!PlayerData.Data.CheckLevelsCompleted(Level.world1BossLevels))
		{
			worldSelectionIcons[worldSelectionIcons.Length - 1].SetTextColor(disabledText);
			worldSelectionIcons[worldSelectionIcons.Length - 2].SetTextColor(disabledText);
			worldSelectionIcons[worldSelectionIcons.Length - 3].SetTextColor(disabledText);
			selectableLength -= 3;
		}
		else if (!PlayerData.Data.CheckLevelsCompleted(Level.world2BossLevels))
		{
			worldSelectionIcons[worldSelectionIcons.Length - 1].SetTextColor(disabledText);
			worldSelectionIcons[worldSelectionIcons.Length - 2].SetTextColor(disabledText);
			selectableLength -= 2;
		}
		else if (!PlayerData.Data.CheckLevelsCompleted(Level.world3BossLevels))
		{
			worldSelectionIcons[worldSelectionIcons.Length - 1].SetTextColor(disabledText);
			selectableLength--;
		}
		UpdateList();
	}

	public void ChangeSelection(int direction)
	{
		index = Mathf.Clamp(index + direction, 0, selectableLength - 1);
		SetCursorPosition(index);
	}

	public void SetCursorPosition(int index)
	{
		this.index = index;
		if (lastIndex != index)
		{
			worldSelectionIcons[index].SetIcons("Icons/" + worldPaths[index] + "_light");
			worldSelectionIcons[index].SetTextColor(new Color(0.827f, 0.765f, 0.702f));
			worldSelectionIcons[lastIndex].SetIcons("Icons/" + worldPaths[lastIndex] + "_dark");
			worldSelectionIcons[lastIndex].SetTextColor(new Color(0.2f, 0.188f, 0.188f));
			AudioManager.Play("menu_equipment_move");
			lastIndex = index;
		}
		cursor.SetPosition(worldSelectionIcons[index].transform.position);
		UpdateList();
	}

	private void UpdateList()
	{
		List<Levels> list = new List<Levels>();
		List<string> list2 = new List<string>();
		list.Clear();
		list2.Clear();
		for (int i = 0; i < checklistItems.Count; i++)
		{
			checklistItems[i].gameObject.SetActive(false);
			checklistItems[i].ClearDescription(selectedFinale);
		}
		for (int j = 0; j < finaleItems.Count; j++)
		{
			finaleItems[j].gameObject.SetActive(false);
			if (finaleItems[j].checkMark != null)
			{
				finaleItems[j].checkMark.enabled = false;
				finaleItems[j].ClearDescription(selectedFinale);
			}
		}
		switch (index)
		{
		case 0:
			list.AddRange(world1Levels);
			selectedFinale = false;
			break;
		case 1:
			list.AddRange(world2Levels);
			selectedFinale = false;
			break;
		case 2:
			list.AddRange(world3Levels);
			selectedFinale = false;
			break;
		case 3:
			list.AddRange(finaleLevels);
			selectedFinale = true;
			break;
		default:
			Debug.LogWarning("Invalid index");
			break;
		}
		foreach (Levels item in list)
		{
			list2.Add(Level.GetLevelName(item).Replace("\\n", " "));
		}
		GameObject[] array = finaleImages;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(selectedFinale);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			if (!selectedFinale)
			{
				checklistItems[l].gameObject.SetActive(true);
				checklistItems[l].EnableCheckbox((l < list2.Count - 2) ? true : false);
				checklistItems[l].SetDescription(list[l], list2[l], selectedFinale);
			}
			else
			{
				finaleItems[l].gameObject.SetActive(true);
				finaleItems[l].SetDescription(list[l], list2[l], selectedFinale);
			}
		}
	}
}

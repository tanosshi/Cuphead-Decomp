using UnityEngine;
using UnityEngine.UI;

public class SlotSelectScreenSlot : AbstractMonoBehaviour
{
	[SerializeField]
	private RectTransform emptyChild;

	[SerializeField]
	private RectTransform mainChild;

	[SerializeField]
	private Text worldMapText;

	[SerializeField]
	private Image boxImage;

	[SerializeField]
	private Image starImage;

	[SerializeField]
	private Image noiseImage;

	[SerializeField]
	private Sprite unselectedBoxSprite;

	[SerializeField]
	private Sprite unselectedBoxSpriteExpert;

	[SerializeField]
	private Sprite unselectedBoxSpriteComplete;

	[SerializeField]
	private Sprite unselectedNoise;

	[SerializeField]
	private Sprite selectedBoxSprite;

	[SerializeField]
	private Sprite selectedBoxSpriteExpert;

	[SerializeField]
	private Sprite selectedBoxSpriteComplete;

	[SerializeField]
	private Sprite selectedNoise;

	[SerializeField]
	private Text slotText;

	[SerializeField]
	private Text emptyText;

	[SerializeField]
	private Color selectedTextColor;

	[SerializeField]
	private Color unselectedTextColor;

	private bool isExpert;

	private bool isComplete;

	public bool IsEmpty { get; private set; }

	public void Init(int slotNumber)
	{
		PlayerData dataForSlot = PlayerData.GetDataForSlot(slotNumber);
		if (!dataForSlot.GetMapData(Scenes.scene_map_world_1).sessionStarted && !dataForSlot.IsTutorialCompleted)
		{
			emptyChild.gameObject.SetActive(true);
			mainChild.gameObject.SetActive(false);
			IsEmpty = true;
			return;
		}
		IsEmpty = false;
		emptyChild.gameObject.SetActive(false);
		mainChild.gameObject.SetActive(true);
		if (slotNumber == 0)
		{
			slotText.text = "Cuphead A - ";
		}
		if (slotNumber == 1)
		{
			slotText.text = "Cuphead B - ";
		}
		if (slotNumber == 2)
		{
			slotText.text = "Cuphead C - ";
		}
		isExpert = dataForSlot.IsHardModeAvailable;
		int num = Mathf.RoundToInt(dataForSlot.GetCompletionPercentage());
		isComplete = num == 200;
		Text text = slotText;
		text.text = text.text + num + "%";
		switch (dataForSlot.CurrentMap)
		{
		case Scenes.scene_map_world_1:
			worldMapText.text = "Inkwell Isle One";
			break;
		case Scenes.scene_map_world_2:
			worldMapText.text = "Inkwell Isle Two";
			break;
		case Scenes.scene_map_world_3:
			worldMapText.text = "Inkwell Isle Three";
			break;
		case Scenes.scene_map_world_4:
			worldMapText.text = "Inkwell Isle Four";
			break;
		}
	}

	public void SetSelected(bool selected)
	{
		slotText.color = ((!selected) ? unselectedTextColor : selectedTextColor);
		worldMapText.color = ((!selected) ? unselectedTextColor : selectedTextColor);
		emptyText.color = ((!selected) ? unselectedTextColor : selectedTextColor);
		boxImage.sprite = ((!selected) ? unselectedBoxSprite : selectedBoxSprite);
		if (!IsEmpty && isComplete)
		{
			starImage.sprite = ((!selected) ? unselectedBoxSpriteComplete : selectedBoxSpriteComplete);
			starImage.gameObject.SetActive(true);
		}
		else if (!IsEmpty && isExpert)
		{
			starImage.sprite = ((!selected) ? unselectedBoxSpriteExpert : selectedBoxSpriteExpert);
			starImage.gameObject.SetActive(true);
		}
		else
		{
			starImage.gameObject.SetActive(false);
		}
		noiseImage.sprite = ((!selected) ? unselectedNoise : selectedNoise);
	}

	public string getSlotText()
	{
		return slotText.text;
	}
}

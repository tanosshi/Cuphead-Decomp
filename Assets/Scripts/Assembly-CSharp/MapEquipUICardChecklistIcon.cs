using UnityEngine;
using UnityEngine.UI;

public class MapEquipUICardChecklistIcon : AbstractMapCardIcon
{
	public Text iconText;

	public void SetTextColor(Color color)
	{
		iconText.color = color;
	}
}

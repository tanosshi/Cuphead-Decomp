using UnityEngine;

public class MapEquipUICardBackSelectIcon : AbstractMapCardIcon
{
	[Header("Directions")]
	[SerializeField]
	private MapEquipUICardBackSelectIcon up;

	[SerializeField]
	private MapEquipUICardBackSelectIcon down;

	[SerializeField]
	private MapEquipUICardBackSelectIcon left;

	[SerializeField]
	private MapEquipUICardBackSelectIcon right;

	public int Index { get; set; }

	public int GetIndexOfNeighbor(Trilean2 direction)
	{
		MapEquipUICardBackSelectIcon mapEquipUICardBackSelectIcon = null;
		if ((int)direction.x < 0)
		{
			mapEquipUICardBackSelectIcon = left;
		}
		if ((int)direction.x > 0)
		{
			mapEquipUICardBackSelectIcon = right;
		}
		if ((int)direction.y > 0)
		{
			mapEquipUICardBackSelectIcon = up;
		}
		if ((int)direction.y < 0)
		{
			mapEquipUICardBackSelectIcon = down;
		}
		if (mapEquipUICardBackSelectIcon == null)
		{
			return Index;
		}
		return mapEquipUICardBackSelectIcon.Index;
	}
}

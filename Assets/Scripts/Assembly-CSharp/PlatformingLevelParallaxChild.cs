using UnityEngine;

public class PlatformingLevelParallaxChild : AbstractMonoBehaviour
{
	[SerializeField]
	private int _sortingOrderOffset;

	public int SortingOrderOffset
	{
		get
		{
			return _sortingOrderOffset;
		}
	}
}

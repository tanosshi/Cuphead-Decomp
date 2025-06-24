using UnityEngine;

public class DicePalaceMainLevelBoardSpace : MonoBehaviour
{
	[SerializeField]
	private GameObject space;

	[SerializeField]
	private GameObject clearSpace;

	[SerializeField]
	private GameObject odds;

	[SerializeField]
	private GameObject heartSpace;

	[SerializeField]
	private Transform pivot;

	public bool HasHeart
	{
		get
		{
			if (heartSpace == null || odds == null)
			{
				return false;
			}
			return heartSpace.activeSelf;
		}
		set
		{
			if (heartSpace != null && odds != null)
			{
				odds.SetActive(!value);
				heartSpace.SetActive(value);
			}
		}
	}

	public Transform Pivot
	{
		get
		{
			return pivot;
		}
	}

	public bool Clear
	{
		set
		{
			space.SetActive(!value);
			clearSpace.SetActive(value);
		}
	}

	public Vector3 HeartSpacePosition
	{
		get
		{
			if (heartSpace != null)
			{
				return heartSpace.transform.position;
			}
			return Vector3.zero;
		}
	}
}

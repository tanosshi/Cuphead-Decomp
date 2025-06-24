using System.Collections.Generic;
using UnityEngine;

public class BeeLevelPlatforms : AbstractMonoBehaviour
{
	private const float OFFSET = -230f;

	[SerializeField]
	private Transform[] rows;

	private static int lastPlatform;

	public void Init()
	{
		List<Transform> list = new List<Transform>(rows);
		for (int i = 0; i < rows.Length; i++)
		{
			Transform transform = Object.Instantiate(rows[i]);
			list.Add(transform);
			transform.transform.SetParent(base.transform);
			transform.transform.AddLocalPosition(0f, -230f);
		}
		rows = list.ToArray();
	}

	public void Randomize(int missingCount)
	{
		Transform[] array = rows;
		foreach (Transform transform in array)
		{
			List<Transform> list = new List<Transform>(transform.GetChildTransforms());
			foreach (Transform item in list)
			{
				item.gameObject.SetActive(true);
			}
			for (int j = 0; j < missingCount; j++)
			{
				if (list.Count <= 1)
				{
					break;
				}
				int num = Random.Range(0, list.Count);
				if ((num == 0 && lastPlatform == 0) || (num == 3 && lastPlatform == 2))
				{
					break;
				}
				list[num].gameObject.SetActive(false);
				lastPlatform = num;
				list.RemoveAt(num);
			}
		}
	}
}

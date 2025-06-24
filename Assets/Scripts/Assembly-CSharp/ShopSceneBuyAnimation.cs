using System.Collections.Generic;
using UnityEngine;

public class ShopSceneBuyAnimation : MonoBehaviour
{
	public GameObject[] coins;

	private List<GameObject> spawnedCoins;

	private List<int> indexes;

	private List<int> rightIndex;

	private void Start()
	{
		rightIndex = new List<int>();
		indexes = new List<int> { 0, 1, 2, 3, 4, 5 };
		int index = Random.Range(0, 6);
		rightIndex.Add(indexes[index]);
		indexes.RemoveAt(index);
		int index2 = Random.Range(0, 5);
		rightIndex.Add(indexes[index2]);
		indexes.RemoveAt(index2);
		int index3 = Random.Range(0, 4);
		rightIndex.Add(indexes[index3]);
		indexes.RemoveAt(index3);
		for (int i = 0; i < rightIndex.Count; i++)
		{
			coins[rightIndex[i]].gameObject.SetActive(true);
		}
	}

	private void Update()
	{
	}
}

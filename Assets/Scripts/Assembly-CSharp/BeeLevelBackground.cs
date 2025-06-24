using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeLevelBackground : LevelProperties.Bee.Entity
{
	public const float GROUP_OFFSET = 455f;

	[SerializeField]
	private BeeLevelPlatforms platformGroup;

	[SerializeField]
	private BeeLevelBackgroundGroup[] groups;

	[SerializeField]
	private Transform[] middleGroups;

	[SerializeField]
	private ScrollingSprite back;

	private BeeLevel level;

	protected override void Start()
	{
		base.Start();
		level = Level.Current as BeeLevel;
		StartCoroutine(middle_cr());
	}

	protected override void Update()
	{
		base.Update();
		back.speed = (0f - level.Speed) * 0.35f;
	}

	public override void LevelInit(LevelProperties.Bee properties)
	{
		base.LevelInit(properties);
		int[] array = new int[groups.Length];
		List<int> list = new List<int>();
		for (int i = 0; i < groups.Length; i++)
		{
			list.Add(i);
		}
		for (int j = 0; j < groups.Length; j++)
		{
			int index = Random.Range(0, list.Count);
			array[j] = list[index];
			list.RemoveAt(index);
			groups[array[j]].Init(platformGroup, groups.Length);
			groups[array[j]].SetY(-455f * (float)j);
		}
		platformGroup.gameObject.SetActive(false);
	}

	private IEnumerator middle_cr()
	{
		SpriteRenderer[] sprites = new SpriteRenderer[middleGroups.Length];
		for (int i = 0; i < middleGroups.Length; i++)
		{
			sprites[i] = middleGroups[i].GetComponentInChildren<SpriteRenderer>();
			middleGroups[i].gameObject.SetActive(false);
		}
		int scale = ((Random.value > 0.5f) ? 1 : (-1));
		while (true)
		{
			int i2 = Random.Range(0, middleGroups.Length);
			float height = (int)sprites[i2].sprite.bounds.size.y;
			float y = (720f + height) / 2f;
			middleGroups[i2].gameObject.SetActive(true);
			middleGroups[i2].SetPosition(0f, y, 0f);
			middleGroups[i2].SetScale(scale, 1f, 1f);
			while (middleGroups[i2].position.y >= 0f - y)
			{
				middleGroups[i2].AddPosition(0f, level.Speed * 0.75f * (float)CupheadTime.Delta);
				yield return null;
			}
			middleGroups[i2].gameObject.SetActive(false);
			yield return null;
		}
	}
}
